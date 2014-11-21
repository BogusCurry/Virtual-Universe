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
using Aurora.Framework.ModuleLoader;
using Aurora.Framework.Modules;
using Aurora.Framework.Services;
using Aurora.Framework.Services.ClassHelpers.Inventory;
using Nini.Ini;
using Nini.Config;
using OpenMetaverse;
using System;
using System.IO;
using System.Collections.Generic;
using Aurora.Framework.SceneInfo;
using Aurora.Framework.Utilities;

namespace Aurora.Services.SQLServices.InventoryService
{
    /// <summary>
    ///     Basically a hack to give us a Inventory library while we don't have a inventory server
    ///     once the server is fully implemented then should read the data from that
    /// </summary>
    public class LibraryService : ILibraryService, IService
    {
        // moved to Constants to allow for easier comparision from the WebUI
        //private readonly UUID libOwner = new UUID("11111111-1111-0000-0000-000100bba000");
        readonly UUID libOwner = new UUID (Constants.LibraryOwner);

        public UUID LibraryRootFolderID
        {
            // similarly placed in Constants
            //get { return new UUID("00000112-000f-0000-0000-000100bba000"); }
            get { return new UUID(Constants.LibraryRootFolderID); }
        }

        string libOwnerName = "Central 46";
        bool m_enabled;
        IRegistryCore m_registry;
        string pLibName = "Welcome Package";
        protected IInventoryService m_inventoryService;

        #region ILibraryService Members

        public UUID LibraryOwner
        {
            get { return libOwner; }
        }

        public string LibraryOwnerName
        {
            get { return libOwnerName; }
        }

        public string LibraryName
        {
            get { return pLibName; }
        }

        #endregion

        #region IService Members

        public void Initialize(IConfigSource config, IRegistryCore registry)
        {
            string pLibOwnerName = "Central 46";

            IConfig libConfig = config.Configs["LibraryService"];
            if (libConfig != null)
            {
                m_enabled = true;
                pLibName = libConfig.GetString("LibraryName", pLibName);
                libOwnerName = libConfig.GetString("LibraryOwnerName", pLibOwnerName);
            }

            //MainConsole.Instance.Debug("[LIBRARY]: Starting library service...");

            registry.RegisterModuleInterface<ILibraryService>(this);
            m_registry = registry;
        }

        public void Start(IConfigSource config, IRegistryCore registry)
        {
            if (m_enabled)
            {
                if (MainConsole.Instance != null)
                    MainConsole.Instance.Commands.AddCommand("Empty Welcome Package", "empty welcome package",
                                                             "Empties the Welcome Package",
                                                             EmptyWelcomePackage, false, true);
            }
        }

        public void FinishedStartup()
        {
            m_inventoryService = m_registry.RequestModuleInterface<IInventoryService>();
            LoadLibraries();
        }

        #endregion

        public void LoadLibraries()
		{
			if (!m_enabled) {
				return;
			}
			if (!File.Exists ("Settings/WelcomePaclage/WelcomePackage.ini") &&
				!File.Exists ("Settings/WelcomePackage/WelcomePackage.ini.example")) {
				MainConsole.Instance.Error (
					"Could not find WelcomePackage/WelcomePackage.ini or WelcomePackage/WelcomePackage.ini.example");
				return;
			}
			List<IDefaultLibraryLoader> Loaders = ModuleLoader.PickupModules<IDefaultLibraryLoader> ();
			try {
				if (!File.Exists ("Settings/WelcomePackage/WelcomePackage.ini")) {
					File.Copy ("Settings/WelcomePackage/WelcomePackage.ini.example", "Settings/WelcomePackage/WelcomePackage.ini");
				}
				IniConfigSource iniSource = new IniConfigSource ("Settings/WelcomePackage/WelcomePackage.ini",
				                                                            IniFileType.AuroraStyle);
				if (iniSource != null) {
					foreach (IDefaultLibraryLoader loader in Loaders) {
						loader.LoadLibrary (this, iniSource, m_registry);
					}
				}
			} catch {
			}
		}

        void EmptyWelcomePackage(IScene scene, string[] cmd)
        {
            string sure = MainConsole.Instance.Prompt("Are you sure you want to empty the Welcome Package? (yes/no)", "no");
            if (!sure.Equals("yes", StringComparison.CurrentCultureIgnoreCase))
                return;
            EmptyWelcomePackage();
        }

        public void EmptyWelcomePackage()
        {
            //Delete the root folders
            InventoryFolderBase root = m_inventoryService.GetRootFolder(LibraryOwner);
            while (root != null)
            {
                MainConsole.Instance.Info("Removing folder " + root.Name);
                m_inventoryService.ForcePurgeFolder(root);
                root = m_inventoryService.GetRootFolder(LibraryOwner);
            }
            List<InventoryFolderBase> rootFolders = m_inventoryService.GetRootFolders(LibraryOwner);
            foreach (InventoryFolderBase rFolder in rootFolders)
            {
                MainConsole.Instance.Info("Removing folder " + rFolder.Name);
                m_inventoryService.ForcePurgeFolder(rFolder);
            }
            MainConsole.Instance.Info("Finished removing emptying welcome package");
        }
    }
}