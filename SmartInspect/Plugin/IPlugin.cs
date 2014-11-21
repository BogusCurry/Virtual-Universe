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

using SmartInspect.Repository;

namespace SmartInspect.Plugin
{
	/// <summary>
	/// Interface implemented by logger repository plugins.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Plugins define additional behavior that can be associated
	/// with a <see cref="SmartInspect.Repository.ILoggerRepository"/>.
	/// The <see cref="PluginMap"/> held by the <see cref="SmartInspect.Repository.ILoggerRepository.PluginMap"/>
	/// property is used to store the plugins for a repository.
	/// </para>
	/// <para>
	/// The <c>SmartInspect.Config.PluginAttribute</c> can be used to
	/// attach plugins to repositories created using configuration
	/// attributes.
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	/// <author>Gert Driesen</author>
	public interface IPlugin
	{
		/// <summary>
		/// Gets the name of the plugin.
		/// </summary>
		/// <value>
		/// The name of the plugin.
		/// </value>
		/// <remarks>
		/// <para>
		/// Plugins are stored in the <see cref="PluginMap"/>
		/// keyed by name. Each plugin instance attached to a
		/// repository must be a unique name.
		/// </para>
		/// </remarks>
		string Name { get; }

		/// <summary>
		/// Attaches the plugin to the specified <see cref="ILoggerRepository"/>.
		/// </summary>
		/// <param name="repository">The <see cref="ILoggerRepository"/> that this plugin should be attached to.</param>
		/// <remarks>
		/// <para>
		/// A plugin may only be attached to a single repository.
		/// </para>
		/// <para>
		/// This method is called when the plugin is attached to the repository.
		/// </para>
		/// </remarks>
		void Attach(ILoggerRepository repository);

		/// <summary>
		/// Is called when the plugin is to shutdown.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method is called to notify the plugin that 
		/// it should stop operating and should detach from
		/// the repository.
		/// </para>
		/// </remarks>
		void Shutdown();
	}
}