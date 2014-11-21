/***************************************************************************
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

using Aurora.Framework.ConsoleFramework;
using Aurora.Framework.Modules;
using Aurora.Framework.SceneInfo;
using Aurora.Framework.Serialization;
using Aurora.Framework.Services;
using Aurora.Framework.Utilities;
using Nini.Config;
using OpenMetaverse.StructuredData;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Aurora.Region.Components
{
    public class ComponentManager : IService, ISOPSerializerModule
    {
        #region IComponentManager Members

        /// <summary>
        ///     Take the serialized string and set up the Components for this object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="serialized"></param>
        public void DeserializeComponents(ISceneChildEntity obj, string serialized)
        {
            //Pull the OSDMap out for components
            OSDMap map;
            try
            {
                if (serialized == "")
                    map = new OSDMap();
                else
                    map = (OSDMap) OSDParser.DeserializeJson(serialized);
            }
            catch
            {
                //Bad JSON? Just return
                return;
            }

            //Now check against the list of components we have loaded
            foreach (KeyValuePair<string, OSD> kvp in map)
            {
                PropertyInfo property = obj.GetType().GetProperty(kvp.Key);
                if (property != null)
                {
                    property.SetValue(obj, Util.OSDToObject(kvp.Value, property.PropertyType), null);
                }
            }
            map.Clear();
            map = null;
        }

        #endregion

        #region ISOPSerializerModule Members

        public void Deserialization(ISceneChildEntity obj, XmlTextReader reader)
        {
            string components = reader.ReadElementContentAsString("Components", String.Empty);
            if (components != "")
            {
                try
                {
                    DeserializeComponents(obj, components);
                }
                catch (Exception ex)
                {
                    MainConsole.Instance.Warn("[COMPONENTMANAGER]: Error on deserializing Components! " + ex);
                }
            }
        }

        public string Serialization(ISceneChildEntity part)
        {
            return null;
        }

        #endregion

        #region IService Members

        public void Initialize(IConfigSource config, IRegistryCore registry)
        {
            SceneEntitySerializer.SceneObjectSerializer.AddSerializer("Components", this);
        }

        public void Start(IConfigSource config, IRegistryCore registry)
        {
        }

        public void FinishedStartup()
        {
        }

        #endregion
    }
}