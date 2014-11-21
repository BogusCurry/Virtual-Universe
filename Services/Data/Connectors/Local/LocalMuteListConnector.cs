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

using Aurora.Framework.ClientInterfaces;
using Aurora.Framework.DatabaseInterfaces;
using Aurora.Framework.Modules;
using Aurora.Framework.Services;
using Aurora.Framework.Utilities;
using Nini.Config;
using OpenMetaverse;
using System.Collections.Generic;

namespace Aurora.Services.DataService
{
    public class LocalMuteListConnector : ConnectorBase, IMuteListConnector, 
	#region IMuteListConnector Members
	#endregion
	System.IDisposable
	{
		IGenericData GD;
		public void Initialize (IGenericData GenericData, IConfigSource source, IRegistryCore simBase, string defaultConnectionString)
		{
			GD = GenericData;
			if (source.Configs [Name] != null)
				defaultConnectionString = source.Configs [Name].GetString ("ConnectionString", defaultConnectionString);
			if (GD != null)
				GD.ConnectToDatabase (defaultConnectionString, "Generics", source.Configs ["Connectors"].GetBoolean ("ValidateTables", true));
			Framework.Utilities.DataManager.RegisterPlugin (Name + "Local", this);
			if (source.Configs ["Connectors"].GetString ("MuteListConnector", "LocalConnector") == "LocalConnector") {
				Framework.Utilities.DataManager.RegisterPlugin (this);
			}
			Init (simBase, Name);
		}
		public string Name {
			get {
				return "IMuteListConnector";
			}
		}
		/// <summary>
		///     Gets the full mute list for the given agent.
		/// </summary>
		/// <param name="AgentID"></param>
		/// <returns></returns>
		[CanBeReflected (ThreatLevel = ThreatLevel.Low)]
		public List<MuteList> GetMuteList (UUID AgentID)
		{
			object remoteValue = DoRemote (AgentID);
			if (remoteValue != null || m_doRemoteOnly)
				return (List<MuteList>)remoteValue;
			return GenericUtils.GetGenerics<MuteList> (AgentID, "MuteList", GD);
		}
		/// <summary>
		///     Updates or adds a mute for the given agent
		/// </summary>
		/// <param name="mute"></param>
		/// <param name="AgentID"></param>
		[CanBeReflected (ThreatLevel = ThreatLevel.Low)]
		public void UpdateMute (MuteList mute, UUID AgentID)
		{
			object remoteValue = DoRemote (mute, AgentID);
			if (remoteValue != null || m_doRemoteOnly)
				return;
			GenericUtils.AddGeneric (AgentID, "MuteList", mute.MuteID.ToString (), mute.ToOSD (), GD);
		}
		/// <summary>
		///     Deletes a mute for the given agent
		/// </summary>
		/// <param name="muteID"></param>
		/// <param name="AgentID"></param>
		[CanBeReflected (ThreatLevel = ThreatLevel.Low)]
		public void DeleteMute (UUID muteID, UUID AgentID)
		{
			object remoteValue = DoRemote (muteID, AgentID);
			if (remoteValue != null || m_doRemoteOnly)
				return;
			GenericUtils.RemoveGenericByKeyAndType (AgentID, "MuteList", muteID.ToString (), GD);
		}
		/// <summary>
		///     Checks to see if PossibleMuteID is muted by AgentID
		/// </summary>
		/// <param name="AgentID"></param>
		/// <param name="PossibleMuteID"></param>
		/// <returns></returns>
		[CanBeReflected (ThreatLevel = ThreatLevel.Low)]
		public bool IsMuted (UUID AgentID, UUID PossibleMuteID)
		{
			object remoteValue = DoRemote (AgentID, PossibleMuteID);
			if (remoteValue != null || m_doRemoteOnly)
				return remoteValue != null && (bool)remoteValue;
			return GenericUtils.GetGeneric<MuteList> (AgentID, "MuteList", PossibleMuteID.ToString (), GD) != null;
		}
		public void Dispose ()
		{
		}
	}
}