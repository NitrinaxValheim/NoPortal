// System
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

// BepInEx
using BepInEx;

using Plugin;

namespace Common
{
    public class DependencyOperations
    {

        public static bool CheckForDependencyErrors(string pluginName) {

            bool dependenciesError = false;

            //PluginDependencies.Dependencies.EngineName;
            //PluginDependencies.Dependencies.EngineVersion;
            if (VersionOperations.CheckVersionWithOutput(
                pluginName,
                Dependencies.EngineName,
                VersionOperations.GetEngineVersion(),
                Dependencies.EngineVersion
                ) == false) { dependenciesError = true;  }

            //PluginDependencies.Dependencies.GameName
            //PluginDependencies.Dependencies.GameVersion
            if (VersionOperations.CheckVersionWithOutput(
                pluginName,
                Dependencies.GameName,
                VersionOperations.GetGameVersion(),
                Dependencies.GameVersion
                ) == false) { dependenciesError = true; }

            //PluginDependencies.Dependencies.BepInExName
            //PluginDependencies.Dependencies.BepInExVersion
            if (VersionOperations.CheckVersionWithOutput(
                pluginName,
                Dependencies.BepInExName,
                VersionOperations.GetBepinExVersion(),
                Dependencies.BepInExVersion
                ) == false) { dependenciesError = true; }

            //PluginDependencies.Dependencies.IsJotunnRequired
            //PluginDependencies.Dependencies.JotunnName
            //PluginDependencies.Dependencies.JotunnVersion
            if (Dependencies.IsJotunnRequired == true)
            {
                if (VersionOperations.CheckVersionWithOutput(
                    pluginName,
                    Dependencies.JotunnName,
                    VersionOperations.GetJotunnVersion(),
                    Dependencies.JotunnVersion
                    ) == false) { dependenciesError = true; }
            }

            //PluginDependencies.Dependencies.IsOtherPluginsRequired
            //PluginDependencies.Dependencies.RequiredPlugins
            if (Dependencies.IsOtherPluginsRequired == true)
            {

                if (FileOperations.checkPluginDependencies(
                    Dependencies.RequiredPlugins
                    ) == false) { dependenciesError = true; }

            }

            //PluginDependencies.Dependencies.IsFilesRequired
            //PluginDependencies.Dependencies.RequiredFiles
            if (Dependencies.IsFilesRequired == true)
            {
                if (FileOperations.checkPluginDependencies(
                    Dependencies.RequiredFiles
                    ) == false) { dependenciesError = true; }
            }

            return dependenciesError;
        
        }

    }

}
