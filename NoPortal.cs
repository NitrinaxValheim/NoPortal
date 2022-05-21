// System
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

// UnityEngine
using UnityEngine;

// BepInEx
using BepInEx;

// Jotunn
using Jotunn.Managers;
using Jotunn.Utils;

using Logger = Jotunn.Logger;

////  functions
using NoPortal.Settings;

namespace NoPortal
{

    // setup plugin data
    [BepInPlugin(ModData.Guid, ModData.ModName, ModData.Version)]

    // check for running valheim process
    [BepInProcess("valheim.exe")]

    // check for jotunn dependency
    [BepInDependency(Jotunn.Main.ModGuid, BepInDependency.DependencyFlags.HardDependency)]

    // check compatibility level
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Patch)]

    internal class NoPortal : BaseUnityPlugin
    {

        private static Boolean showModInfo = true;
        private static Boolean showModDetailedInfo = false;
        private static Boolean showModErrorInfo = true;

        private static Boolean showEventInfo = false;
        private static Boolean showEventDetailedInfo = false;
        private static Boolean showEventErrorInfo = true;

        #region[Awake]
        private void Awake()
        {

            // check if incompatible mods installed
            if (CheckForOtherPortalMods() == false)
            {

                // ##### plugin startup logic #####

                if (showModDetailedInfo == true) { Jotunn.Logger.LogInfo("Loading start"); }

                // PieceManager
                PieceManager.OnPiecesRegistered += OnPiecesRegistered;

                if (showModDetailedInfo == true) { Jotunn.Logger.LogInfo("Loading done"); }

                // ##### info functions #####

                // Game data
                if (showModInfo == true) { Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} is active."); }
            
            }
            else
            {

                if (showModErrorInfo == true) { Logger.LogWarning($"Other portal mod found. {PluginInfo.PLUGIN_GUID} is inactive."); }

            }

        }
        #endregion

        #region[EventSystem]

        // PieceManager
        #region[OnPiecesRegistered]
        private void OnPiecesRegistered()
        {

            try
            {

                if (showEventInfo == true) { Logger.LogInfo("OnPiecesRegistered"); }

                // remove known portal pieces from all PieceTables

                // get handle for all registered PieceTables
                List<PieceTable> pieceTables = PieceManager.Instance.GetPieceTables();
                //Dictionary<int, GameObject> prefabList = ZNetScene.instance.m_namedPrefabs;

                // check all piece tables
                foreach (PieceTable pieceTable in pieceTables)
                {

                    if (showEventDetailedInfo == true) { Logger.LogInfo("pieceTable " + pieceTable.name); }

                    // get handle for portal prefab names
                    string[] portalPrefabNames = GetPortalPrefabNames();

                    // check alls portal prefabs
                    foreach (string portalPrefabName in portalPrefabNames)
                    {

                        // check if prefab in piece table
                        if (pieceTable.m_pieces.Contains(PrefabManager.Instance.GetPrefab(portalPrefabName)))
                        {

                            if (showEventDetailedInfo == true) { Logger.LogInfo("found piecePrefab " + portalPrefabName); }

                            // get handle for portal prefab
                            GameObject piecePrefab = PrefabManager.Instance.GetPrefab(portalPrefabName);

                            // get index of portal prefab in piece table
                            int prefabIndex = pieceTable.m_pieces.IndexOf(piecePrefab);
                            if (showEventDetailedInfo == true) { Logger.LogInfo("prefabIndex " + prefabIndex); }

                            // remove piece at index of piece table
                            pieceTable.m_pieces.RemoveAt(prefabIndex);
                            //pieceTable.m_pieces.Remove(piecePrefab);

                            Logger.LogWarning(portalPrefabName + " removed from " + pieceTable.name + " at position " + prefabIndex);

                        }
                        else
                        {
                            if (showEventDetailedInfo == true) { Logger.LogInfo("not found " + portalPrefabName); }

                        }                       

                    }

                }

                // set globalkey 'noportals'

                bool globalKey = ZoneSystem.instance.GetGlobalKey("noportals");
                
                if (globalKey == true)
                {

                    Logger.LogInfo("GlobalKey noportals already set");

                }
                else
                {

                    ZoneSystem.instance.SetGlobalKey("noportals");

                    Logger.LogWarning("GlobalKey 'noportals' set");

                }

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

            string[] outdatedModList = GetOutdatedModList();

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

            foreach (string incompatibleMod in incompatibleModList)
            {

                // check for other portal mod, if found return true
                if (LookForModInPluginDir(incompatibleMod + ".dll") == true) {

                    Logger.LogWarning("Incompatible mod found : " + incompatibleMod + ". Please remove it or you cant use NoPortal.");

                    return true;
                
                }

            }

            return false;

        }
        #endregion

        // look for installed mods in bepinex plugin dir
        #region[LookForModInPluginDir]
        private static Boolean LookForModInPluginDir(string modName)
        {

            string[] modFound = Directory.GetFiles(@BepInEx.Paths.PluginPath, modName, SearchOption.AllDirectories);

            foreach (string mod in modFound)
            {

                // check if mod contains modname, if found return true
                if (mod.Contains(modName)) { return true; }

            }

            return false;

        }
        #endregion

        // define portal prefab names for vanilla and custom portals
        #region[GetPortalPrefabNames]
        private static string[] GetPortalPrefabNames()
        {

            string[] portalPrefabNames =
            {

                "portal",                       // ingame, but currently unused because bugged
                "portal_wood",                  // _HammerPieceTable, portal_wood, 129
                "structure_asgard_portal",      // Asgard Portal - Bifrost

            };

            return portalPrefabNames;

        }
        #endregion

        // defines a list of incompatible mods that do not work with noportal and/or generate errors when noportal is installed
        #region[GetIncompatibleModList]
        private static string[] GetIncompatibleModList()
        {

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
        private static string[] GetOutdatedModList()
        {

            string[] outdatedModList =
            {

                "TPWolves",

            };

            return outdatedModList;

        }
        #endregion

    }

}