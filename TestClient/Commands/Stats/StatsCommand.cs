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

using System;
using System.Collections.Generic;
using System.Text;
using OpenMetaverse;
using OpenMetaverse.Packets;

namespace OpenMetaverse.TestClient
{
    public class StatsCommand : Command
    {
        public StatsCommand(TestClient testClient)
        {
            Name = "stats";
            Description = "Provide connection figures and statistics";
            Category = CommandCategory.Simulator;
        }

        public override string Execute(string[] args, UUID fromAgentID)
        {
            StringBuilder output = new StringBuilder();

            lock (Client.Network.Simulators)
            {
                for (int i = 0; i < Client.Network.Simulators.Count; i++)
                {
                    Simulator sim = Client.Network.Simulators[i];

                    output.AppendLine(String.Format(
                        "[{0}] Dilation: {1} InBPS: {2} OutBPS: {3} ResentOut: {4}  ResentIn: {5}",
                        sim.ToString(), sim.Stats.Dilation, sim.Stats.IncomingBPS, sim.Stats.OutgoingBPS, 
                        sim.Stats.ResentPackets, sim.Stats.ReceivedResends));
                }
            }

            Simulator csim = Client.Network.CurrentSim;

            output.Append("Packets in the queue: " + Client.Network.InboxCount);
			output.AppendLine(String.Format("FPS : {0} PhysicsFPS : {1} AgentUpdates : {2} Objects : {3} Scripted Objects : {4}",
                csim.Stats.FPS, csim.Stats.PhysicsFPS, csim.Stats.AgentUpdates, csim.Stats.Objects, csim.Stats.ScriptedObjects));
			output.AppendLine(String.Format("Frame Time : {0} Net Time : {1} Image Time : {2} Physics Time : {3} Script Time : {4} Other Time : {5}",
                csim.Stats.FrameTime, csim.Stats.NetTime, csim.Stats.ImageTime, csim.Stats.PhysicsTime, csim.Stats.ScriptTime, csim.Stats.OtherTime));
			output.AppendLine(String.Format("Agents : {0} Child Agents : {1} Active Scripts : {2}",
                csim.Stats.Agents, csim.Stats.ChildAgents, csim.Stats.ActiveScripts));

            return output.ToString();
        }
    }
}