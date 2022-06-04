// System
using System;
using System.IO;
using System.Collections.Generic;

// UnityEngine
using UnityEngine;

// BepInEx
using BepInEx;
using BepInEx.Configuration;

// Jotunn
using Jotunn.Managers;
using Jotunn.Utils;
using Logger = Jotunn.Logger;

// Settings
using PluginSettings;

namespace NoPortal
{

    // setup plugin data
    [BepInPlugin(PluginData.Guid, PluginData.ModName, PluginData.Version)]

    // check for running valheim process
    [BepInProcess("valheim.exe")]

    // check for jotunn dependency
    [BepInDependency(Jotunn.Main.ModGuid, BepInDependency.DependencyFlags.HardDependency)]

    // check compatibility level
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Patch)]

    internal class NoPortal : BaseUnityPlugin
    {

        // public values

        public static bool isModded = true;

        // private values

        // plugin
        private const Boolean showPluginInfo = true;
        private const Boolean showPluginDetailedInfo = false;
        private const Boolean showPluginErrorInfo = true;

        // event
        private const Boolean showEventInfo = false;
        private const Boolean showEventDetailedInfo = false;
        private const Boolean showEventErrorInfo = true;

        // sub
        private const Boolean showSubInfo = false;
        private const Boolean showSubDetailedInfo = false;
        private const Boolean showSubErrorInfo = true;

        // debug
        private const Boolean showDebugInfo = false;

        // config values
        // enum for AllowPortals
        [Flags]
        private enum AllowPortalOptions
        {
            None = 0,
            Vanilla = 1,
            Custom = 2,
            All = 4
        };

        //portal category
        private const string ConfigCategoryPortal = "Portal";

        private const string ConfigEntryBrowseAllPieceTables = "BrowseAllPieceTables";
        private const bool ConfigEntryBrowseAllPieceTablesDefaultState = true;
        private const string ConfigEntryBrowseAllPieceTablesDescription = "Browse all known piece tables for portal pieces.";

        private const string ConfigEntryAllowPortals = "AllowPortals";
        private const AllowPortalOptions ConfigEntryAllowPortalsDefaultState = AllowPortalOptions.None;
        private const string ConfigEntryAllowPortalsDescription = "Allow 'None', only 'Vanilla', only 'Custom' or 'All' portals.";

        /* temporary disabled
        private const string ConfigEntrySetNoportalsGlobalKey = "SetGlobalKey";
        private const bool ConfigEntrySetNoportalsGlobalKeyDefaultState = true;
        private const string ConfigEntrySetNoportalsGlobalKeyDescription = "If AllowPortals is set to 'None', the global key 'noportals' is set.";

        private const string ConfigEntryForceNoportalsGlobalKey = "ForceGlobalKey";
        private const bool ConfigEntryForceNoportalsGlobalKeyDefaultState = false;
        private const string ConfigEntryForceNoportalsGlobalKeyDescription = "Force the global key 'noportals' every time.";
        */

        // config values
        private ConfigEntry<bool> configModEnabled;
        private ConfigEntry<int> configNexusID;
        private ConfigEntry<bool> configShowChangesAtStartup;
        private ConfigEntry<bool> configBrowseAllPieceTables;
        private ConfigEntry<AllowPortalOptions> configAllowPortals;

        /* temporary disabled
        private ConfigEntry<bool> configSetNoportalsGlobalKey;
        private ConfigEntry<bool> configForceNoportalsGlobalKey;
        */

        #region[Awake]
        private void Awake()
        {

            CreateConfigValues();

            // check if incompatible mods installed
            if (CheckForOtherPortalMods() == false)
            {

                // ##### plugin startup logic #####

                if (showPluginDetailedInfo == true) { Jotunn.Logger.LogInfo("Loading start"); }

                // ItemManager
                ItemManager.OnItemsRegistered += OnItemsRegistered;

                // PieceManager
                PieceManager.OnPiecesRegistered += OnPiecesRegistered;

                if (showPluginDetailedInfo == true) { Jotunn.Logger.LogInfo("Loading done"); }

                // ##### info functions #####

                // Game data
                if (showPluginInfo == true) { Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} is active."); }

            }
            else
            {

                if (showPluginErrorInfo == true) { Logger.LogWarning($"Other portal mod found. {PluginInfo.PLUGIN_GUID} is inactive."); }

            }

        }
        #endregion

        #region[CreateConfigValues]
        private void CreateConfigValues()
        {

            if (showSubInfo == true) { Logger.LogInfo("CreateConfigValues"); }

            Config.SaveOnConfigSet = true;

            // Add client config which can be edited in every local instance independently
            configModEnabled = Config.Bind(
                PluginData.ConfigCategoryGeneral,
                PluginData.ConfigEntryEnabled,
                PluginData.ConfigEntryEnabledDefaultState,
                new ConfigDescription(PluginData.ConfigEntryEnabledDescription,
                    null,
                    new ConfigurationManagerAttributes
                    {
                        Order = 0,
                    })
            );

            configNexusID = Config.Bind(
                PluginData.ConfigCategoryGeneral,
                PluginData.ConfigEntryNexusID,
                PluginData.ConfigEntryNexusIDID,
                new ConfigDescription(PluginData.ConfigEntryNexusIDDescription,
                    null,
                    new ConfigurationManagerAttributes
                    {
                        Order = 1,
                        Browsable = false,
                        ReadOnly = true
                    }
                )
            );

            configShowChangesAtStartup = Config.Bind(
                PluginData.ConfigCategoryPlugin,
                PluginData.ConfigEntryShowChangesAtStartup,
                PluginData.ConfigEntryShowChangesAtStartupDefaultState,
                new ConfigDescription(PluginData.ConfigEntryShowChangesAtStartupDescription,
                    null,
                    new ConfigurationManagerAttributes
                    {
                        DefaultValue = PluginData.ConfigEntryShowChangesAtStartupDefaultState,
                        Order = 2
                    }
                )
            );

            configBrowseAllPieceTables = Config.Bind(
                ConfigCategoryPortal,
                ConfigEntryBrowseAllPieceTables,
                ConfigEntryBrowseAllPieceTablesDefaultState,
                new ConfigDescription(ConfigEntryBrowseAllPieceTablesDescription,
                    null,
                    new ConfigurationManagerAttributes
                    {
                        DefaultValue = ConfigEntryBrowseAllPieceTablesDefaultState,
                        Order = 3
                    }
                )
            );

            configAllowPortals = Config.Bind(
                ConfigCategoryPortal,
                ConfigEntryAllowPortals,
                ConfigEntryAllowPortalsDefaultState,
                new ConfigDescription(
                    ConfigEntryAllowPortalsDescription,
                    null,
                    new ConfigurationManagerAttributes
                    {
                        DefaultValue = ConfigEntryAllowPortalsDefaultState,
                        Order = 4
                    }
                )
            );

            /* temporary disabled
            configSetNoportalsGlobalKey = Config.Bind(
                ConfigCategoryPortal,
                ConfigEntrySetNoportalsGlobalKey,
                ConfigEntrySetNoportalsGlobalKeyDefaultState,
                new ConfigDescription(
                    ConfigEntrySetNoportalsGlobalKeyDescription,
                    null,
                    new ConfigurationManagerAttributes
                    {
                        DefaultValue = ConfigEntrySetNoportalsGlobalKeyDefaultState,
                        Order = 5
                    }
                )
            );

            configForceNoportalsGlobalKey = Config.Bind(
                ConfigCategoryPortal,
                ConfigEntryForceNoportalsGlobalKey,
                ConfigEntryForceNoportalsGlobalKeyDefaultState,
                new ConfigDescription(
                    ConfigEntryForceNoportalsGlobalKeyDescription,
                    null,
                    new ConfigurationManagerAttributes
                    {
                        DefaultValue = ConfigEntryForceNoportalsGlobalKeyDefaultState,
                        Order = 6
                    }
                )
            );
            */

            // You can subscribe to a global event when config got synced initially and on changes
            SynchronizationManager.OnConfigurationSynchronized += (obj, attr) =>
            {

                if (attr.InitialSynchronization)
                {

                    Jotunn.Logger.LogMessage("Initial Config sync event received");

                }
                else
                {

                    Jotunn.Logger.LogMessage("Config sync event received");

                }

            };

        }
        #endregion

        #region[ReadConfigValues]
        private void ReadConfigValues()
        {

            if (showSubInfo == true) { Logger.LogInfo("ReadConfigValues"); }

            bool pluginEnabled = (bool)Config[PluginData.ConfigCategoryGeneral, PluginData.ConfigEntryEnabled].BoxedValue;
            if (showDebugInfo == true) { Logger.LogInfo("pluginEnabled " + pluginEnabled); }

            if (pluginEnabled == true)
            {

                browsePieceTables();

            }

        }
        #endregion

        #region[browsePieceTables]
        private void browsePieceTables()
        {

            if (showSubInfo == true) { Logger.LogInfo("browsePieceTables"); }

            // get config vars
            bool browseAllPieceTables = (bool)Config[ConfigCategoryPortal, ConfigEntryBrowseAllPieceTables].BoxedValue;
            if (showDebugInfo == true) { Logger.LogInfo("browseAllPieceTables " + browseAllPieceTables); }

            // check piece tables
            if (browseAllPieceTables == true)
            {

                // get handle for all registered PieceTables
                List<PieceTable> pieceTables = PieceManager.Instance.GetPieceTables();

                // check all piece tables
                foreach (PieceTable pieceTable in pieceTables)
                {

                    if (showDebugInfo == true) { Logger.LogInfo("pieceTable " + pieceTable.name); }

                    CheckPieceTableForPortalPieces(pieceTable);

                }

            }
            else
            {

                // get handle for _HammerPieceTable
                PieceTable pieceTable = PieceManager.Instance.GetPieceTable("_HammerPieceTable");

                if (showDebugInfo == true) { Logger.LogInfo("pieceTable " + pieceTable.name); }

                CheckPieceTableForPortalPieces(pieceTable);

            }

            /* temporary disabled
            // get forceNoPortalsGlobalKey
            bool forceNoPortalsGlobalKey = (bool)Config[PluginData.ConfigCategoryPlugin, nameOfForceNoportalsGlobalKeyConfigEntry].BoxedValue;
            if (showDebugInfo == true) { Logger.LogInfo("forceNoPortalsGlobalKey " + forceNoPortalsGlobalKey); }

            // get AllowPortalOptions
            AllowPortalOptions allowPortals = (AllowPortalOptions)Config[PluginData.ConfigCategoryPlugin, nameOfAllowPortalsConfigEntry].BoxedValue;
            if (showDebugInfo == true) { Logger.LogInfo("allowPortals " + allowPortals); }

            if (forceNoPortalsGlobalKey == true)
            {
                if (GetGlobalKey() == false) { SetGlobalKey(); }
            }
            else
            {

                switch (allowPortals)
                {

                    case AllowPortalOptions.None:
                        if (GetGlobalKey() == false) { SetGlobalKey(); }
                        break;

                    case AllowPortalOptions.Vanilla:
                        if (GetGlobalKey() == true) { RemoveGlobalKey(); }
                        break;

                    case AllowPortalOptions.Custom:
                        if (GetGlobalKey() == true) { RemoveGlobalKey(); }
                        break;

                    case AllowPortalOptions.All:
                        if (GetGlobalKey() == true) { RemoveGlobalKey(); }
                        break;

                    default:
                        break;

                }

            }
            */

        }
        #endregion

        #region[CheckPieceTableForPortalPieces]
        private void CheckPieceTableForPortalPieces(PieceTable pieceTable)
        {

            if (showSubInfo == true) { Logger.LogInfo("CheckPieceTableForPortalPieces"); }

            // get AllowPortalOptions
            AllowPortalOptions allowPortals = (AllowPortalOptions)Config[ConfigCategoryPortal, ConfigEntryAllowPortals].BoxedValue;
            if (showDebugInfo == true) { Logger.LogInfo("allowPortals " + allowPortals); }

            bool showChangesAtStartup = (bool)Config[PluginData.ConfigCategoryPlugin, PluginData.ConfigEntryShowChangesAtStartup].BoxedValue;
            if (showDebugInfo == true) { Logger.LogInfo("showChangesAtStartup " + showChangesAtStartup); }

            switch (allowPortals)
            {

                case AllowPortalOptions.None:

                    if (showDebugInfo == true) { Logger.LogInfo("'None' option selected"); }

                    // remove vanilla portal piece
                    RemoveVanillaPortalPieceFromHammerPieceTable();

                    // remove all custom portal pieces 
                    RemoveCustomPortalPiecesFromPieceTables(pieceTable);

                    break;

                case AllowPortalOptions.Vanilla:

                    if (showDebugInfo == true) { Logger.LogInfo("'Vanilla' option selected"); }

                    // add vanilla portal piece
                    // workaround if portal_wood is not available in hammerpiecetable as it was previously removed,
                    // but it should be available again after changing config accordingly and re-logging in
                    AddVanillaPortalPieceToHammerPieceTable();

                    // remove all custom portal pieces
                    RemoveCustomPortalPiecesFromPieceTables(pieceTable);

                    break;

                case AllowPortalOptions.Custom:

                    if (showDebugInfo == true) { Logger.LogInfo("'Custom' option selected"); }

                    // remove vanilla portal piece, allow all custom portal pieces
                    RemoveVanillaPortalPieceFromHammerPieceTable();

                    break;

                case AllowPortalOptions.All:

                    if (showDebugInfo == true) { Logger.LogInfo("'All' option selected"); }

                    // add vanilla portal piece
                    // workaround if portal_wood is not available in hammerpiecetable as it was previously removed,
                    // but it should be available again after changing config accordingly and re-logging in
                    AddVanillaPortalPieceToHammerPieceTable();

                    // allow all portal pieces, do nothing
                    if (showChangesAtStartup == true) { Logger.LogWarning("AllowPortals is set to 'All'. No changes are made for " + pieceTable.name + "."); }

                    break;

                default:

                    Logger.LogWarning("unknown option found " + allowPortals.GetType().ToString());

                    break;

            }

        }
        #endregion

        #region[AddVanillaPortalPieceToPieceTable]
        private void AddVanillaPortalPieceToHammerPieceTable()
        {

            if (showSubInfo == true) { Logger.LogInfo("AddVanillaPortalPieceToHammerPieceTable"); }

            // set name of piece table
            string portalPieceTable = "_HammerPieceTable";

            // set name of portal_wood in hammerpiecetable
            string portalPieceName = "portal_wood";

            // set index of portal_wood in hammerpiecetable
            int portalPieceIndex = 129;

            PieceTable pieceTable = PieceManager.Instance.GetPieceTable(portalPieceTable);

            // check if prefab in piece table
            if (!pieceTable.m_pieces.Contains(PrefabManager.Instance.GetPrefab(portalPieceName)))
            {

                // add piece
                AddPieceToPieceTable(pieceTable, portalPieceName, portalPieceIndex);

            }

        }
        #endregion

        #region[RemoveVanillaPortalPieceFromHammerPieceTable]
        private void RemoveVanillaPortalPieceFromHammerPieceTable()
        {

            if (showSubInfo == true) { Logger.LogInfo("RemoveVanillaPortalPiecesFromPieceTables"); }

            // set name of piece table
            string portalPieceTable = "_HammerPieceTable";

            // set name of portal_wood in hammerpiecetable
            string portalPieceName = "portal_wood";

            PieceTable pieceTable = PieceManager.Instance.GetPieceTable(portalPieceTable);

            // check if prefab in piece table
            if (pieceTable.m_pieces.Contains(PrefabManager.Instance.GetPrefab(portalPieceName)))
            {

                // remove all vanilla portal pieces
                RemovePieceFromPieceTable(pieceTable, portalPieceName);

            }

        }
        #endregion

        #region[RemoveCustomPortalPiecesFromPieceTables]
        private void RemoveCustomPortalPiecesFromPieceTables(PieceTable pieceTable)
        {

            if (showSubInfo == true) { Logger.LogInfo("RemoveCustomPortalPiecesFromPieceTables"); }

            // get handle for portal prefab names
            string[] customPieceNames = GetCustomPortalPieceNames();

            // check alls custom portal prefabs
            foreach (string customPieceName in customPieceNames)
            {

                // check if prefab in piece table
                if (pieceTable.m_pieces.Contains(PrefabManager.Instance.GetPrefab(customPieceName)))
                {

                    // remove all portal pieces
                    RemovePieceFromPieceTable(pieceTable, customPieceName);

                }

            }

        }
        #endregion

        #region[AddPieceToPieceTable]
        private void AddPieceToPieceTable(PieceTable pieceTable, string pieceName, int pieceIndex)
        {

            // get handle for portal prefab
            GameObject piecePrefab = PrefabManager.Instance.GetPrefab(pieceName);

            pieceTable.m_pieces.Insert(pieceIndex, piecePrefab);

        }
        #endregion

        #region[RemovePieceFromPieceTable]
        private void RemovePieceFromPieceTable(PieceTable pieceTable, string pieceName)
        {

            if (showSubInfo == true) { Logger.LogInfo("RemovePieceFromPieceTable"); }

            bool showChangesAtStartup = (bool)Config[PluginData.ConfigCategoryPlugin, PluginData.ConfigEntryShowChangesAtStartup].BoxedValue;
            if (showDebugInfo == true) { Logger.LogInfo("showChangesAtStartup " + showChangesAtStartup); }

            // get handle for portal prefab
            GameObject piecePrefab = PrefabManager.Instance.GetPrefab(pieceName);

            // get index of portal prefab in piece table
            int pieceIndex = pieceTable.m_pieces.IndexOf(piecePrefab);
            if (showDebugInfo == true) { Logger.LogInfo("prefabIndex " + pieceIndex); }

            if (pieceIndex > -1)
            {

                // remove piece at index of piece table
                // pieceTable.m_pieces.RemoveAt(pieceIndex);
                pieceTable.m_pieces.Remove(piecePrefab);

                if (showChangesAtStartup == true) { Logger.LogWarning(pieceName + " removed from " + pieceTable.name + " at position " + pieceIndex); }

            }
            else
            {

                Logger.LogWarning(pieceName + " not found in " + pieceTable.name);

            }

        }
        #endregion

        #region[SetGlobalKey]
        private void SetGlobalKey(string keyName = "")
        {

            if (showSubInfo == true) { Logger.LogInfo("setGlobalKey"); }

            bool showChangesAtStartup = (bool)Config[PluginData.ConfigCategoryPlugin, PluginData.ConfigEntryShowChangesAtStartup].BoxedValue;
            if (showDebugInfo == true) { Logger.LogInfo("showChangesAtStartup " + showChangesAtStartup); }

            ZoneSystem.instance.SetGlobalKey("noportals");
            //ZoneSystem.instance.m_globalKeys.Add("noportals");

            if (GetGlobalKey() == true)
            {

                if (showChangesAtStartup == true) { Logger.LogWarning("Global key 'noportals' set."); }

            }
            else
            {

                if (showChangesAtStartup == true) { Logger.LogWarning("Global key 'noportals' cant set."); }
            }

        }
        #endregion

        #region[RemoveGlobalKey]
        private void RemoveGlobalKey(string keyName = "")
        {

            if (showSubInfo == true) { Logger.LogInfo("removeGlobalKey"); }

            bool showChangesAtStartup = (bool)Config[PluginData.ConfigCategoryPlugin, PluginData.ConfigEntryShowChangesAtStartup].BoxedValue;
            if (showDebugInfo == true) { Logger.LogInfo("showChangesAtStartup " + showChangesAtStartup); }

            ZoneSystem.instance.RemoveGlobalKey("noportals");
            //ZoneSystem.instance.m_globalKeys.Remove("noportals");

            if (GetGlobalKey() == false)
            {

                if (showChangesAtStartup == true) { Logger.LogWarning("Global key 'noportals' removed."); }

            }
            else
            {

                if (showChangesAtStartup == true) { Logger.LogWarning("Global key 'noportals' cant removed."); }
            }


        }
        #endregion

        #region[GetGlobalKey]
        private bool GetGlobalKey(string keyName = "")
        {

            bool globalKey = ZoneSystem.instance.GetGlobalKey("noportals");

            return globalKey;

        }
        #endregion

        private void debugGlobalKeys()
        {

            bool globalKey = ZoneSystem.instance.GetGlobalKey("noportals");
            Logger.LogInfo("ZoneSystem.instance.GetGlobalKey " + globalKey);

            List<string> globalKeyList = ZoneSystem.instance.GetGlobalKeys();

            foreach (string globalKeyEntry in globalKeyList)
            {
                Logger.LogInfo(globalKeyEntry);
            }

            bool globalKeyA = ZoneSystem.instance.m_globalKeys.Contains("noportals");
            Logger.LogInfo("ZoneSystem.instance.m_globalKeys.Contains " + globalKeyA);

            HashSet<string> globalKeyListA = ZoneSystem.instance.m_globalKeys;

            foreach (string globalKeyEntry in globalKeyListA)
            {
                Logger.LogInfo(globalKeyEntry);
            }

        }
        #region[EventSystem]

        // ItemManager
        #region[OnItemsRegistered]
        private void OnItemsRegistered()
        {

            try
            {

                if (showEventInfo == true) { Logger.LogInfo("OnItemsRegistered"); }

                ReadConfigValues();

                /* temporary disabled
                debugGlobalKeys();

                // debug 'noportals' global key
                if (GetGlobalKey() == true)
                {

                    Logger.LogInfo("noportals set");

                }
                else
                {

                    Logger.LogInfo("noportals not set");

                }
                */

            }
            catch (Exception ex)
            {
                if (showEventErrorInfo == true) { Logger.LogError($"Error OnItemsRegistered : {ex.Message}"); }
            }
            finally
            {
                PrefabManager.OnPrefabsRegistered -= OnItemsRegistered;
            }

        }
        #endregion

        // PieceManager
        #region[OnPiecesRegistered]
        private void OnPiecesRegistered()
        {

            try
            {

                if (showEventInfo == true) { Logger.LogInfo("OnPiecesRegistered"); }

            }
            catch (Exception ex)
            {

                if (showEventErrorInfo == true) { Logger.LogError($"Error OnPiecesRegistered : {ex.Message}"); }

            }
            finally
            {

                PieceManager.OnPiecesRegistered -= OnPiecesRegistered;

            }

        }
        #endregion

        #endregion

        // check if other portal mods installed
        #region[CheckForOtherPortalMods]
        private Boolean CheckForOtherPortalMods()
        {

            if (showSubInfo == true) { Logger.LogInfo("CheckForOtherPortalMods"); }

            string[] outdatedModList = GetOutdatedModList();

            // check for outdated plugins
            foreach (string outdatedMod in outdatedModList)
            {

                // check for other portal mod, if found return true
                if (LookForModInPluginDir(outdatedMod + ".dll") == true)
                {

                    Logger.LogWarning("Outdated mod found : " + outdatedMod + ". Please remove it.");

                    return true;

                }

            }

            string[] incompatibleModList = GetIncompatibleModList();

            // check for incompatible plugins
            foreach (string incompatibleMod in incompatibleModList)
            {

                // check for other portal mod, if found return true
                if (LookForModInPluginDir(incompatibleMod + ".dll") == true)
                {

                    Logger.LogWarning("Incompatible mod found : " + incompatibleMod + ". Please remove it or you cant use NoPortal.");

                    return true;

                }

            }

            return false;

        }
        #endregion

        // look for installed mods in bepinex plugin dir
        #region[LookForModInPluginDir]
        private Boolean LookForModInPluginDir(string modName)
        {

            if (showSubInfo == true) { Logger.LogInfo("LookForModInPluginDir"); }

            string[] modFound = Directory.GetFiles(@BepInEx.Paths.PluginPath, modName, SearchOption.AllDirectories);

            foreach (string mod in modFound)
            {

                // check if mod contains modname, if found return true
                if (mod.Contains(modName)) { return true; }

            }

            return false;

        }
        #endregion

        // define portal piece names of custom portals
        #region[GetCustomPortalPieceNames]
        private string[] GetCustomPortalPieceNames()
        {

            if (showSubInfo == true) { Logger.LogInfo("GetCustomPortalPieceNames"); }

            string[] portalPieceNames =
            {

                "structure_asgard_portal",      // Asgard Portal - Bifrost

            };

            return portalPieceNames;

        }
        #endregion

        // defines a list of incompatible mods that do not work with noportal and/or generate errors when noportal is installed
        #region[GetIncompatibleModList]
        private string[] GetIncompatibleModList()
        {

            if (showSubInfo == true) { Logger.LogInfo("GetIncompatibleModList"); }

            string[] incompatibleModList =
            {

                "MapTeleporter",                    // allows teleportation without portals
                "MapTeleport",                      // allows teleportation without portals

            };

            return incompatibleModList;

        }
        #endregion

        // // defines outdated list of mods that not work with current valheim version
        #region[GetOutdatedModList]
        private string[] GetOutdatedModList()
        {

            if (showSubInfo == true) { Logger.LogInfo("GetOutdatedModList"); }

            string[] outdatedModList =
            {

                "TPWolves",

            };

            return outdatedModList;

        }
        #endregion

    }

}