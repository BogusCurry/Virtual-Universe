﻿/*
 * Software license for this code will be added after the code is completed.
 * This code is in the early stages of development so should not be utilized 
 * at this time.
 * 
 * Thanks
 * Virtual Universe Development Team
 */

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using VirtualUniverse.ScriptEngine.VirtualScript.MiniModule;
using Microsoft.CSharp;
using OpenMetaverse;

//using Microsoft.JScript;

namespace VirtualUniverse.ScriptEngine.VirtualScript.CompilerTools
{
    public class MRMConverter : IScriptConverter
    {
        private readonly CSharpCodeProvider CScodeProvider = new CSharpCodeProvider();

        #region IScriptConverter Members

        public string DefaultState
        {
            get { return ""; }
        }

        public void Initialise(Compiler compiler)
        {
        }

        public void Convert(string Script, out string CompiledScript,
                            out object PositionMap)
        {
            CompiledScript = CreateCompilerScript(Script);
            PositionMap = null;
        }

        public string Name
        {
            get { return "MRM:C#"; }
        }

        public CompilerResults Compile(CompilerParameters parameters, bool isFile, string Script)
        {
            string[] lines = Script.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            List<string> libraries = new List<string>();
            foreach (string s in lines)
                if (s.StartsWith("//@DEPENDS:"))
                    libraries.Add(s.Replace("//@DEPENDS:", ""));

            string rootPath =
                Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            foreach (string library in libraries)
            {
                if (rootPath != null) parameters.ReferencedAssemblies.Add(Path.Combine(rootPath, library));
            }

            libraries.Add("OpenMetaverseTypes.dll");


            bool complete = false;
            bool retried = false;

            CompilerResults results;
            do
            {
                lock (CScodeProvider)
                {
                    if (isFile)
                        results = CScodeProvider.CompileAssemblyFromFile(
                            parameters, Script);
                    else
                        results = CScodeProvider.CompileAssemblyFromSource(
                            parameters, Script);
                }
                // Deal with an occasional segv in the compiler.
                // Rarely, if ever, occurs twice in succession.
                // Line # == 0 and no file name are indications that
                // this is a native stack trace rather than a normal
                // error log.
                if (results.Errors.Count > 0)
                {
                    if (!retried && string.IsNullOrEmpty(results.Errors[0].FileName) &&
                        results.Errors[0].Line == 0)
                    {
                        // System.Console.WriteLine("retrying failed compilation");
                        retried = true;
                    }
                    else
                    {
                        complete = true;
                    }
                }
                else
                {
                    complete = true;
                }
            } while (!complete);
            return results;
        }

        public void FinishCompile(IScriptModulePlugin plugin, ScriptData data, IScript Script)
        {
            MRMBase mmb = (MRMBase)Script;
            if (mmb == null)
                return;

            InitializeMRM(plugin, data, mmb, data.Part.LocalId, data.ItemID);
            mmb.Start();
        }

        public void FindErrorLine(CompilerError CompErr, object PositionMap, string script, out int LineN, out int CharN)
        {
            LineN = CompErr.Line;
            CharN = CompErr.Column;
        }

        #endregion

        public void Dispose()
        {
        }

        private string CreateCompilerScript(string compileScript)
        {
            return ConvertMRMKeywords(compileScript);
        }

        private string ConvertMRMKeywords(string script)
        {
            script = script.Replace("microthreaded void", "IEnumerable");
            script = script.Replace("relax;", "yield return null;");

            return script;
        }

        public void GetGlobalEnvironment(IScriptModulePlugin plugin, ScriptData data, uint localID, out IWorld world,
                                         out IHost host)
        {
            // UUID should be changed to object owner.
            UUID owner = data.World.RegionInfo.EstateSettings.EstateOwner;
            SEUser securityUser = new SEUser(owner, "Name Unassigned");
            SecurityCredential creds = new SecurityCredential(securityUser, data.World);

            world = new World(data.World, creds);
            host = new Host(new SOPObject(data.World, localID, creds), data.World,
                            new ExtensionHandler(plugin.Extensions));
        }

        public void InitializeMRM(IScriptModulePlugin plugin, ScriptData data, MRMBase mmb, uint localID, UUID itemID)
        {
            IWorld world;
            IHost host;

            GetGlobalEnvironment(plugin, data, localID, out world, out host);

            mmb.InitMiniModule(world, host, itemID);
        }
    }
}