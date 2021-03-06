/*
 * Copyright (c) Contributors, http://virtual-planets.org/, http://whitecore-sim.org/, http://aurora-sim.org, http://opensimulator.org//
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the Aurora-Sim Project nor the
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Nini.Config;
using OpenMetaverse;
using Universe.Framework.Modules;
using Universe.Framework.SceneInfo;
using Universe.Framework.Servers.HttpServer.Interfaces;
using Universe.Framework.ConsoleFramework;
using Universe.Framework.Utilities;
using System.Collections.Generic;

namespace Universe.Modules.WorldView
{
    public class WorldViewModule : INonSharedRegionModule
    {

        bool m_Enabled = true;
        IMapImageGenerator m_Generator;
        string m_assetCacheDir = Constants.DEFAULT_ASSETCACHE_DIR;
        string m_worldviewCacheDir;
        bool m_cacheEnabled = true;
        float m_cacheExpires = 24;

        public void Initialise(IConfigSource config)
        {
         
            IConfig moduleConfig = config.Configs ["WorldViewModule"];
            if (moduleConfig != null)
            {
                // enabled by default but allow disabling
                m_Enabled = moduleConfig.GetBoolean ("Enabled", m_Enabled);
                m_cacheEnabled = moduleConfig.GetBoolean ("EnableCache", true);
                m_cacheExpires = moduleConfig.GetFloat("CacheExpires", m_cacheExpires);
            }
             
            if (m_Enabled)
            {
                MainConsole.Instance.Commands.AddCommand (
                    "save worldview",
                    "save worldview [filename]< --fov degrees >",
                    "Save a view of the region to a file",
                    HandleSaveWorldview, true, true);

                MainConsole.Instance.Commands.AddCommand (
                    "save worldmaptile",
                    "save worldmaptile [filename] < --size pixels >",
                    "Save a maptile view of the region to a file",
                    HandleSaveWorldTile, true, true);


                if (m_cacheEnabled)
                {
                    m_assetCacheDir = config.Configs ["AssetCache"].GetString ("CacheDirectory",m_assetCacheDir);
                    CreateCacheDirectories (m_assetCacheDir);
                }

            }
        }

        public void AddRegion (IScene scene)
        {
        }

        public void RegionLoaded (IScene scene)
        {
            if (!m_Enabled)
                return;
            m_Generator = scene.RequestModuleInterface<IMapImageGenerator>();
            if (m_Generator == null)
            {
                m_Enabled = false;
                return;
            }

            ISimulationBase simulationBase = scene.RequestModuleInterface<ISimulationBase>();
            if (simulationBase != null)
            {
                IHttpServer server = simulationBase.GetHttpServer(0);
                server.AddStreamHandler(new WorldViewRequestHandler(this,
                        scene.RegionInfo.RegionID.ToString()));
                MainConsole.Instance.Info("[WORLDVIEW]: Configured and enabled for " + scene.RegionInfo.RegionName);
                MainConsole.Instance.Info("[WORLDVIEW]: RegionID " + scene.RegionInfo.RegionID);
            }
        }

        public void RemoveRegion (IScene scene)
        {
        }

        public string Name
        {
            get { return "WorldViewModule"; }
        }

        public Type ReplaceableInterface
        {
            get { return null; }
        }

        public void Close()
        {
        }

        public bool CacheEnabled
        {
            get { return m_cacheEnabled; }
        }

        public float CacheExpires
        {
            get { return m_cacheExpires; }
        }

        public string CacheDir
        {
            get { return m_worldviewCacheDir; }
        }

        void CreateCacheDirectories(string cacheDir)
        {
            if (!Directory.Exists(cacheDir))
                Directory.CreateDirectory(cacheDir);

            m_worldviewCacheDir = cacheDir + "/Worldview";
            if (!Directory.Exists (m_worldviewCacheDir))
                Directory.CreateDirectory (m_worldviewCacheDir);
        }

        public byte[] GenerateWorldView(Vector3 pos, Vector3 rot, float fov,
                int width, int height, bool usetex)
        {
            if (!m_Enabled)
                return new Byte[0];

            Bitmap bmp = m_Generator.CreateViewImage(pos, rot, fov, width, height, usetex);

            MemoryStream str = new MemoryStream();

            bmp.Save(str, ImageFormat.Jpeg);

            return str.ToArray();
        }

        public void SaveRegionWorldView (IScene scene, string fileName, float fieldOfView)
        {
            m_Generator = scene.RequestModuleInterface<IMapImageGenerator>();
            if (m_Generator == null)
                return;

            // set some basic defaults
            Vector3 camPos = new Vector3 ();

            camPos.X = 1.25f;
            camPos.Y = 1.25f;
            camPos.Z = 61.0f;

            Vector3 camDir = new Vector3 ();
            camDir.X = .687462f;                        // -1  -< y/x > 1
            camDir.Y = .687462f;
            camDir.Z = -0.23536f;                       // -1 (up) < Z > (down) 1

            float fov = 89f;                            // degrees
            if (fieldOfView > 0)
                fov = fieldOfView;

            int width = 1280;           
            int height = 720;  

            byte[] jpeg = ExportWorldView(camPos, camDir, fov, width, height, true); 

            // save image
            var savePath = fileName;
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = scene.RegionInfo.RegionName + ".jpg";
                savePath = PathHelpers.VerifyWriteFile (fileName, ".jpg", Constants.DEFAULT_DATA_DIR + "/Worldview", true);
            }
            File.WriteAllBytes(savePath, jpeg);

        }

        public void SaveRegionWorldMapTile (IScene scene, string fileName, int size)
        {
            m_Generator = scene.RequestModuleInterface<IMapImageGenerator>();
            if (m_Generator == null)
                return;


            byte[] jpeg = ExportWorldMapTile(size); 

            // save image
            var savePath = fileName;
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = scene.RegionInfo.RegionName + "_maptile.jpg";
                savePath = PathHelpers.VerifyWriteFile (fileName, ".jpg", Constants.DEFAULT_DATA_DIR + "/Worldview", true);
            }
            File.WriteAllBytes(savePath, jpeg);

        }

        public byte[] ExportWorldView(Vector3 camPos, Vector3 camDir, float fov,
            int width, int height, bool usetex)
        {
            Bitmap bmp = m_Generator.CreateViewImage(camPos, camDir, fov, width, height, usetex);

            if (bmp != null)
            {
                MemoryStream str = new MemoryStream ();

                bmp.Save (str, ImageFormat.Jpeg);

                return str.ToArray ();
            } 

            return null;
        }

        public byte[] ExportWorldMapTile(int size)
        {
            Bitmap bmp = m_Generator.CreateViewTileImage(size);

            if (bmp != null)
            {
                MemoryStream str = new MemoryStream ();

                bmp.Save (str, ImageFormat.Jpeg);

                return str.ToArray ();
            } else
                return null;
        }

        protected void HandleSaveWorldview(IScene scene, string[] cmdparams)
        {
            string fileName = "";
            float fieldOfView = 0f;

            // check for switch options
            var cmds = new List <string>();
            for (int i = 2; i < cmdparams.Length;)
            {
                if (cmdparams [i].StartsWith ("--fov"))
                {
                    fieldOfView = float.Parse(cmdparams [i + 1]);
                    i +=2;
                } else
                {
                    cmds.Add (cmdparams [i]);
                    i++;
                }
            }

            if (cmds.Count > 0)
                fileName = cmds [0];
            else
            {
                fileName = scene.RegionInfo.RegionName;
                fileName = MainConsole.Instance.Prompt (" Worldview filename", fileName);
                if (fileName == "")
                    return;
            }

            //some file sanity checks
            var savePath = PathHelpers.VerifyWriteFile (fileName, ".jpg", Constants.DEFAULT_DATA_DIR + "/Worldview", true);

            MainConsole.Instance.InfoFormat (
                "[Worldview]: Saving worldview for {0} to {1}", scene.RegionInfo.RegionName, savePath);
        
            SaveRegionWorldView (scene, savePath, fieldOfView);
        }

        protected void HandleSaveWorldTile(IScene scene, string[] cmdparams)
        {
            string fileName = "";
            int size = 256;

            // check for switch options
            var cmds = new List <string>();
            for (int i = 2; i < cmdparams.Length;)
            {
                if (cmdparams [i].StartsWith ("--size"))
                {
                    size = int.Parse(cmdparams [i + 1]);
                    if (size > 4096)
                    {
                    	MainConsole.Instance.Warn("[Worldview]: Size can not be large then 4096");
                    	size = int.Parse(MainConsole.Instance.Prompt (" World maptile size", "4096"));
                    }
                    i +=2;
                } 
                else
                {
                    cmds.Add (cmdparams [i]);
                    i++;
                }
            }

            if (cmds.Count > 0)
                fileName = cmds [0];
            else
            {
                fileName = scene.RegionInfo.RegionName;
                fileName = MainConsole.Instance.Prompt (" World maptile filename", fileName);
                if (fileName == "")
                    return;
            }

            //some file sanity checks
            var savePath = PathHelpers.VerifyWriteFile (fileName+"_maptile", ".jpg", Constants.DEFAULT_DATA_DIR + "/Worldview", true);

            MainConsole.Instance.InfoFormat (
                "[Worldview]: Saving world maptile for {0} to {1}", scene.RegionInfo.RegionName, savePath);

            SaveRegionWorldMapTile (scene, savePath, size);

        }
    }
}