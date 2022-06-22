﻿namespace Plugin
{ // start tag

    class Data
    { // class start

        #region[Data]

        /// <summary>
        ///     The namespace of the mod.
        /// </summary>
        public const string Namespace = "NoPortal";

        /// <summary>
        ///     The name of the mod.
        /// </summary>
        public const string ModName = "NoPortal";

        /// <summary>
        ///     The company of the mod.
        /// </summary>
        public const string Company = "Nitrinax";

        /// <summary>
        ///     The current version of the mod.
        /// </summary>
        public const string Version = "0.0.20";

        /// <summary>
        ///     The BepInEx plugin Mod GUID being used.
        /// </summary>
        public const string ModGuid = Company + "." + Namespace + "." + ModName;

        /// <summary>
        ///     The description of the mod.
        /// </summary>
        public const string Description = "Remove portal from _HammerPieceTable. Admins can still spawn it. Set noportals global key.";

        /// <summary>
        ///     Set the deploy channel.
        /// </summary>
#if (DEBUG)

        public const string Configuration = "Debug";

#else

        public const string Configuration = "Release";

#endif

        /// <summary>
        ///     The license of the mod.
        /// </summary>
        public const string Copyright = "GNU GPL V3";

        /// <summary>
        ///     The trademark of the mod.
        /// </summary>
        public const string Trademark = "";

        /// <summary>
        ///     The culture of the mod.
        /// </summary>
        public const string Culture = "";

        /// <summary>
        ///     The language of the mod.
        /// </summary>
        public const string Language = "English";

        /// <summary>
        ///     The default language of the mod.
        /// </summary>
        //default language for this mod. Possible options are English, German
        public const string PluginDefaultLanguage = "English";

        #endregion

        #region[ConfigData]

        /// <summary>
        ///     Name of Config_General category.
        /// </summary>
        public const string ConfigCategoryGeneral = "General";

        /// <summary>
        ///     Name of Enabled entry.
        /// </summary>
        public const string ConfigEntryEnabled = "Enabled";

        /// <summary>
        ///     Description of Enabled.
        /// </summary>
        public const string ConfigEntryEnabledDescription = "Enable this mod";

        /// <summary>
        ///     The default state of the mod.
        /// </summary>
        //default state of this mod
        public const bool ConfigEntryEnabledDefaultState = true;

        /// <summary>
        ///     Name of NexusID entry.
        /// </summary>
        public const string ConfigEntryNexusID = "NexusID";

        /// <summary>
        ///     Description of NexusID.
        /// </summary>
        public const string ConfigEntryNexusIDDescription = "Nexus mod ID for updates";

        /// <summary>
        ///     The NexusID.
        /// </summary>
        //default state of this mod
        public const int ConfigEntryNexusIDID = 1888;

        /// <summary>
        ///     Name of Plugin category.
        /// </summary>
        public const string ConfigCategoryPlugin = "Plugin";

        /// <summary>
        ///     Name of ShowChangesAtStartup entry.
        /// </summary>
        public const string ConfigEntryShowChangesAtStartup = "ShowChangesAtStartup";

        /// <summary>
        ///     The default ShowChangesAtStartup.
        /// </summary>
        //default state of this mod
        public const bool ConfigEntryShowChangesAtStartupDefaultState = true;

        /// <summary>
        ///     Description of NexusID.
        /// </summary>
        public const string ConfigEntryShowChangesAtStartupDescription = "If this option is set, the changes made by this mod are displayed in the BepinEx log.";

        #endregion

    }

} // end tag