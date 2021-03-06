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
using Universe.Framework.Services;

namespace Universe.Framework.Utilities
{
    /// <summary>
    ///     Plugin manager that deals with retrieving IDataPlugins
    /// </summary>
    public static class DataManager
    {
        private static readonly Dictionary<string, IUniverseDataPlugin> Plugins =
            new Dictionary<string, IUniverseDataPlugin>();

        public static List<IUniverseDataPlugin> GetPlugins()
        {
            return new List<IUniverseDataPlugin>(Plugins.Values);
        }

        /// <summary>
        ///     Request a data plugin from the registry
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RequestPlugin<T>() where T : IUniverseDataPlugin
        {
            if (Plugins.ContainsKey(typeof (T).Name))
            {
                IUniverseDataPlugin Plugin;
                Plugins.TryGetValue(typeof (T).Name, out Plugin);
                return (T) Plugin;
            }
            //Return null if we can't find it
            return default(T);
        }

        /// <summary>
        ///     Request a data plugin from the registry
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RequestPlugin<T>(string name) where T : IUniverseDataPlugin
        {
            if (Plugins.ContainsKey(name))
            {
                IUniverseDataPlugin Plugin;
                Plugins.TryGetValue(name, out Plugin);
                return (T) Plugin;
            }
            //Return null if we can't find it
            return default(T);
        }

        /// <summary>
        ///     Register a new plugin to the registry
        /// </summary>
        /// <param name="plugin"></param>
        public static void RegisterPlugin(IUniverseDataPlugin plugin)
        {
            RegisterPlugin(plugin.Name, plugin);
        }

        /// <summary>
        ///     Register a new plugin to the registry
        /// </summary>
        /// <param name="name"></param>
        /// <param name="plugin"></param>
        public static void RegisterPlugin(string name, IUniverseDataPlugin plugin)
        {
            if (!Plugins.ContainsKey(name))
                Plugins.Add(name, plugin);
        }
    }
}