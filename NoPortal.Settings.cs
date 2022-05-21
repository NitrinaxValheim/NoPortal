namespace NoPortal.Settings
{ // start tag

    class ModData
    { // class start

        #region[ModData]

        public const string Guid = Company + "." + Namespace;

        public const string Namespace = "NoPortal";

        public const string ModName = "NoPortal";

        public const string Version = "0.0.1.1";

        public const string Description = "Remove portal from _HammerPieceTable. Admins can still spawn it. Set noportals global key.";

#if (DEBUG)

        public const string Configuration = "Debug";

#else

        public const string Configuration = "Release";

#endif

        public const string Company = "Nitrinax";

        public const string Copyright = "MIT License © 2022";

        public const string Trademark = "";

        public const string Culture = "";

        public const string Language = "";

        #endregion

        #region[ConfigData]

        //default language for this mod. Possible options are English, German
        public const string ModDefaultLanguage = "English";

        //default state of this mod
        public const bool ModDefaultState = true;

        #endregion

    }

} // end tag