﻿/***************************************************************************
 *	                VIRTUAL REALITY PUBLIC SOURCE LICENSE
 * 
 * Date				: Sun January 1, 2006
 * Copyright		: (c) 2006-2014 by Virtual Reality Development Team. 
 *                    All Rights Reserved.
 * Website			: http://www.syndarveruleiki.is
 *
 * Product Name		: Virtual Reality
 * License Text     : packages/docs/VRLICENSE.txt
 * 
 * Planetary Info   : Information about the Planetary code
 * 
 * Copyright        : (c) 2014-2024 by Second Galaxy Development Team
 *                    All Rights Reserved.
 * 
 * Website          : http://www.secondgalaxy.com
 * 
 * Product Name     : Virtual Reality
 * License Text     : packages/docs/SGLICENSE.txt
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the WhiteCore-Sim Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
***************************************************************************/

using Aurora.Framework.Modules;
using Aurora.Framework.Servers;
using Aurora.Framework.Services;
using Aurora.Framework.Utilities;
using Nini.Config;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GridRegion = Aurora.Framework.Services.GridRegion;

namespace Aurora.Services.GenericServices.CapsService
{
    public class ExternalCapsHandler : ConnectorBase, IExternalCapsHandler, IService
    {
        List<string> m_allowedCapsModules = new List<string>();
        Dictionary<UUID, List<IExternalCapsRequestHandler>> m_caps =
            new Dictionary<UUID, List<IExternalCapsRequestHandler>>();
        ISyncMessagePosterService m_syncPoster;
        List<string> m_servers = new List<string>();

        public void Initialize(IConfigSource config, IRegistryCore registry)
        {
        }

        public void Start(IConfigSource config, IRegistryCore registry)
        {
            IConfig externalConfig = config.Configs["ExternalCaps"];
            if (externalConfig == null) return;
            m_allowedCapsModules = Util.ConvertToList(externalConfig.GetString("CapsHandlers"), true);
            
            ISyncMessageRecievedService service = registry.RequestModuleInterface<ISyncMessageRecievedService>();
            service.OnMessageReceived += service_OnMessageReceived;
            m_syncPoster = registry.RequestModuleInterface<ISyncMessagePosterService>();
            m_registry = registry;
            registry.RegisterModuleInterface<IExternalCapsHandler>(this);

            Init(registry, GetType().Name);
        }

        public void FinishedStartup()
        {
        }

        public OSDMap GetExternalCaps(UUID agentID, GridRegion region)
        {
            if (m_registry == null) return new OSDMap();
            OSDMap resp = new OSDMap();
            if (m_registry.RequestModuleInterface<IGridServerInfoService>() != null)
            {
                m_servers = m_registry.RequestModuleInterface<IGridServerInfoService>().GetGridURIs("SyncMessageServerURI");
                OSDMap req = new OSDMap();
                req["AgentID"] = agentID;
                req["Region"] = region.ToOSD();
                req["Method"] = "GetCaps";

                List<ManualResetEvent> events = new List<ManualResetEvent>();
                foreach (string uri in m_servers.Where((u)=>(!u.Contains(MainServer.Instance.Port.ToString()))))
                {
                    ManualResetEvent even = new ManualResetEvent(false);
                    m_syncPoster.Get(uri, req, (r) =>
                    {
                        if (r == null)
                            return;
                        foreach (KeyValuePair<string, OSD> kvp in r)
                            resp.Add(kvp.Key, kvp.Value);
                        even.Set();
                    });
                    events.Add(even);
                }
                if(events.Count > 0)
                    WaitHandle.WaitAll(events.ToArray());
            }
            foreach (var h in GetHandlers(agentID, region.RegionID))
            {
                if (m_allowedCapsModules.Contains(h.Name))
                    h.IncomingCapsRequest(agentID, region, m_registry.RequestModuleInterface<ISimulatorBase>(), ref resp);
            }
            return resp;
        }

        public void RemoveExternalCaps(UUID agentID, GridRegion region)
        {
            OSDMap req = new OSDMap();
            req["AgentID"] = agentID;
            req["Region"] = region.ToOSD();
            req["Method"] = "RemoveCaps";

            foreach (string uri in m_servers)
                m_syncPoster.Post(uri, req);

            foreach (var h in GetHandlers(agentID, region.RegionID))
            {
                if (m_allowedCapsModules.Contains(h.Name))
                    h.IncomingCapsDestruction();
            }
        }

        OSDMap service_OnMessageReceived(IDictionary<string, OSD> message)
        {
            string method = message["Method"];
            if (method != "GetCaps" && method != "RemoveCaps")
                return null;
            UUID AgentID = message["AgentID"];
            GridRegion region = new GridRegion();
            region.FromOSD((OSDMap)message["Region"]);
            OSDMap map = new OSDMap();
            switch (method)
            {
                case "GetCaps":
                    foreach (var h in GetHandlers(AgentID, region.RegionID))
                    {
                        if (m_allowedCapsModules.Contains(h.Name))
                            h.IncomingCapsRequest(AgentID, region, m_registry.RequestModuleInterface<ISimulatorBase>(), ref map);
                    }
                    return map;
                case "RemoveCaps":
                    foreach (var h in GetHandlers(AgentID, region.RegionID))
                    {
                        if (m_allowedCapsModules.Contains(h.Name))
                            h.IncomingCapsDestruction();
                    }
                    return map;
            }
            return null;
        }

        List<IExternalCapsRequestHandler> GetHandlers(UUID agentID, UUID regionID)
        {
            lock (m_caps)
            {
                List<IExternalCapsRequestHandler> caps;
                if (!m_caps.TryGetValue(agentID ^ regionID, out caps))
                {
                    caps = Aurora.Framework.ModuleLoader.ModuleLoader.PickupModules<IExternalCapsRequestHandler>();
                    m_caps.Add(agentID ^ regionID, caps);
                }
                return caps;
            }
        }
    }
}