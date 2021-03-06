/*
 * Copyright (c) Contributors, http://virtual-planets.org/, http://whitecore-sim.org/, http://aurora-sim.org, http://opensimulator.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the Virtual-Universe Project nor the
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
 */

using System;
using System.Collections.Generic;
using OpenMetaverse;
using Universe.Framework.Services;

namespace Universe.Modules.Ban
{
    public class PresenceInfo
    {
        public UUID AgentID;

        public string LastKnownIP = "";
        public string LastKnownViewer = "";
        public string LastKnownID0 = "";
        public string LastKnownMac = "";
        public string Platform = "";

        public List<string> KnownIPs = new List<string>();
        public List<string> KnownViewers = new List<string>();
        public List<string> KnownMacs = new List<string>();
        public List<string> KnownID0s = new List<string>();
        public List<string> KnownAlts = new List<string>();

        public PresenceInfoFlags Flags;

        [Flags]
        public enum PresenceInfoFlags : int
        {
            Clean = 1 << 1,
            Suspected = 1 << 2,
            Known = 1 << 3,
            SuspectedAltAccount = 1 << 4,
            SuspectedAltAccountOfKnown = 1 << 5,
            SuspectedAltAccountOfSuspected = 1 << 6,
            KnownAltAccountOfKnown = 1 << 7,
            KnownAltAccountOfSuspected = 1 << 8,
            Banned = 1 << 9
        }
    }

    public interface IPresenceInfo : IUniverseDataPlugin
    {
        PresenceInfo GetPresenceInfo(UUID agentID);
        void UpdatePresenceInfo(PresenceInfo agent);
        void Check(PresenceInfo info, List<string> viewers, bool includeList);
        void Check(List<string> viewers, bool includeList);
    }
}