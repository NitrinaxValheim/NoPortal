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

using Plugin;

using Common;

namespace NoPortal
{

    // setup plugin data
    [BepInPlugin(Data.Guid, Data.ModName, Data.Version)]

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

        // config values
        private ConfigEntry<bool> configModEnabled;
        private ConfigEntry<int> configNexusID;
        private ConfigEntry<bool> configShowChangesAtStartup;
        private ConfigEntry<bool> configBrowseAllPieceTables;
        private ConfigEntry<AllowPortalOptions> configAllowPortals;

        #region[Awake]
        private void Awake()
        {

            if (DependencyOperations.CheckForDependencyErrors(PluginInfo.PLUGIN_GUID) == false)
            {

                CreateConfigValues();

                bool modEnabled = (bool)Config[Data.ConfigCategoryGeneral, Data.ConfigEntryEnabled].BoxedValue;

                if (modEnabled == true)
                {

                    // check if incompatible mods installed
                    if (CheckForOtherPortalMods() == false)
                    {
                        // ##### plugin startup logic #####
#if (DEBUG)
                        Logger.LogInfo("Loading start");
#endif

                        // ItemManager
                        ItemManager.OnItemsRegistered += OnItemsRegistered;

                        // ##### info functions #####

#if (DEBUG)
                        Logger.LogInfo("Loading done");
#endif

                        // Game data
#if (DEBUG)
                        Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} is active.");
#endif

                    }
                    else
                    {

                        Logger.LogWarning($"Other portal mod found. {PluginInfo.PLUGIN_GUID} is inactive.");

                    }


                }
                else
                {

                    Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} is disabled by config.");

                }
            }
            else
            {

                Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} is disabled because needed dependencies are missing.");

            }

        }
        #endregion

        #region[CreateConfigValues]
        private void CreateConfigValues()
        {

#if (DEBUG)
            Logger.LogInfo("CreateConfigValues");
#endif

            Config.SaveOnConfigSet = true;

            // Add client config which can be edited in every local instance independently
            configModEnabled = Config.Bind(
                Data.ConfigCategoryGeneral,
                Data.ConfigEntryEnabled,
                Data.ConfigEntryEnabledDefaultState,
                new ConfigDescription(Data.ConfigEntryEnabledDescription,
                    null,
                    new ConfigurationManagerAttributes
                    {
                        Order = 0,
                    })
            );

            configNexusID = Config.Bind(
                Data.ConfigCategoryGeneral,
                Data.ConfigEntryNexusID,
                Data.ConfigEntryNexusIDID,
                new ConfigDescription(Data.ConfigEntryNexusIDDescription,
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
                Data.ConfigCategoryPlugin,
                Data.ConfigEntryShowChangesAtStartup,
                Data.ConfigEntryShowChangesAtStartupDefaultState,
                new ConfigDescription(Data.ConfigEntryShowChangesAtStartupDescription,
                    null,
                    new ConfigurationManagerAttributes
                    {
                        DefaultValue = Data.ConfigEntryShowChangesAtStartupDefaultState,
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

#if (DEBUG)
            Logger.LogInfo("ReadConfigValues");
#endif

            bool pluginEnabled = (bool)Config[Data.ConfigCategoryGeneral, Data.ConfigEntryEnabled].BoxedValue;

#if (DEBUG)
            Logger.LogInfo("pluginEnabled " + pluginEnabled);
#endif

            if (pluginEnabled == true)
            {

                browsePieceTables();

            }

        }
        #endregion

        #region[browsePieceTables]
        private void browsePieceTables()
        {

#if (DEBUG)
            Logger.LogInfo("browsePieceTables");
#endif

            // get config vars
            bool browseAllPieceTables = (bool)Config[ConfigCategoryPortal, ConfigEntryBrowseAllPieceTables].BoxedValue;

            // check piece tables
            if (browseAllPieceTables == true)
            {

                // get handle for all registered PieceTables
                List<PieceTable> pieceTables = PieceManager.Instance.GetPieceTables();

                // check all piece tables
                foreach (PieceTable pieceTable in pieceTables)
                {

#if (DEBUG)
                    Logger.LogInfo("pieceTable " + pieceTable.name);
#endif

                    CheckPieceTableForPortalPieces(pieceTable);

                }

            }
            else
            {

                // get handle for _HammerPieceTable
                PieceTable pieceTable = PieceManager.Instance.GetPieceTable("_HammerPieceTable");

#if (DEBUG)
                Logger.LogInfo("pieceTable " + pieceTable.name);
#endif

                CheckPieceTableForPortalPieces(pieceTable);

            }

        }
        #endregion

        #region[CheckPieceTableForPortalPieces]
        private void CheckPieceTableForPortalPieces(PieceTable pieceTable)
        {

#if (DEBUG)
            Logger.LogInfo("CheckPieceTableForPortalPieces");
#endif

            // get AllowPortalOptions
            AllowPortalOptions allowPortals = (AllowPortalOptions)Config[ConfigCategoryPortal, ConfigEntryAllowPortals].BoxedValue;

#if (DEBUG)
            Logger.LogInfo("allowPortals " + allowPortals);
#endif

            bool showChangesAtStartup = (bool)Config[Data.ConfigCategoryPlugin, Data.ConfigEntryShowChangesAtStartup].BoxedValue;

            switch (allowPortals)
            {

                case AllowPortalOptions.None:

#if (DEBUG)
                    Logger.LogInfo("'None' option selected");
#endif

                    // remove vanilla portal piece
                    RemoveVanillaPortalPieceFromHammerPieceTable();

                    // remove all custom portal pieces 
                    RemoveCustomPortalPiecesFromPieceTables(pieceTable);

                    break;

                case AllowPortalOptions.Vanilla:

#if (DEBUG)
                    Logger.LogInfo("'Vanilla' option selected");
#endif

                    // add vanilla portal piece
                    // workaround if portal_wood is not available in hammerpiecetable as it was previously removed,
                    // but it should be available again after changing config accordingly and re-logging in
                    AddVanillaPortalPieceToHammerPieceTable();

                    // remove all custom portal pieces
                    RemoveCustomPortalPiecesFromPieceTables(pieceTable);

                    break;

                case AllowPortalOptions.Custom:

#if (DEBUG)
                    Logger.LogInfo("'Custom' option selected");
#endif

                    // remove vanilla portal piece, allow all custom portal pieces
                    RemoveVanillaPortalPieceFromHammerPieceTable();

                    break;

                case AllowPortalOptions.All:

#if (DEBUG)
                    Logger.LogInfo("'All' option selected");
#endif

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

#if (DEBUG)
            Logger.LogInfo("AddVanillaPortalPieceToHammerPieceTable");
#endif

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

#if (DEBUG)
            Logger.LogInfo("RemoveVanillaPortalPiecesFromPieceTables");
#endif

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

#if (DEBUG)
            Logger.LogInfo("RemoveCustomPortalPiecesFromPieceTables");
#endif

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

#if (DEBUG)
            Logger.LogInfo("RemovePieceFromPieceTable");
#endif

            bool showChangesAtStartup = (bool)Config[Data.ConfigCategoryPlugin, Data.ConfigEntryShowChangesAtStartup].BoxedValue;

            // get handle for portal prefab
            GameObject piecePrefab = PrefabManager.Instance.GetPrefab(pieceName);

            // get index of portal prefab in piece table
            int pieceIndex = pieceTable.m_pieces.IndexOf(piecePrefab);

#if (DEBUG)
            Logger.LogInfo("prefabIndex " + pieceIndex);
#endif

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

        #region[EventSystem]

        // ItemManager
        #region[OnItemsRegistered]
        private void OnItemsRegistered()
        {

            try
            {

#if (DEBUG)
                Logger.LogInfo("OnItemsRegistered");
#endif

                ReadConfigValues();

            }
            catch (Exception ex)
            {
                Logger.LogError($"Error OnItemsRegistered : {ex.Message}");
            }
            finally
            {
                PrefabManager.OnPrefabsRegistered -= OnItemsRegistered;
            }

        }
        #endregion

        #endregion

        // check if other portal mods installed
        #region[CheckForOtherPortalMods]
        private Boolean CheckForOtherPortalMods()
        {

#if (DEBUG)
            Logger.LogInfo("CheckForOtherPortalMods");
#endif

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

#if (DEBUG)
            Logger.LogInfo("LookForModInPluginDir");
#endif

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

#if (DEBUG)
            Logger.LogInfo("GetCustomPortalPieceNames");
#endif

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

#if (DEBUG)
            Logger.LogInfo("GetIncompatibleModList");
#endif

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

#if (DEBUG)
            Logger.LogInfo("GetOutdatedModList");
#endif

            string[] outdatedModList =
            {

                "TPWolves",

            };

            return outdatedModList;

        }
        #endregion

    }

}