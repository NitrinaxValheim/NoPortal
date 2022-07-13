# Changelog

* Version 0.0.22
   + reworked configuration to adapt to Jotunn 2.7.0 changes and to use BepInExConfigurationManager
  (please delete your current Nitrinax.CustomizedBronze.cfg to create a new config with the current strings)
   + The description text for BrowseAllPieceTables and AllowPortals has been extended to better describe the options.

* Version 0.0.21
   + revert changes of ModGuid and changed it from Company.Namespace.ModName to Company.ModName because otherwise the name of the config file becomes too long

* Version 0.0.20
   + changed Guid to ModGuid
   + added ModName to ModGuid
   + changed CompatibilityLevel from EveryoneMustHaveMod to ClientMustHaveMod
   + adjusted Version string from pattern 0.0.0.0 to 0.0.0

* Version 0.0.1.9
   + updated for Valheim 0.209.8 (Unity 2020.3.33f1)
   + updated for BepInEX 5.4.1901
   + updated for Jotunn 2.6.10
   + renamed class PluginDependencies to Dependencies and moved it into folder Plugin
   + renamed class PluginSettings to Data and moved it into folder Plugin
   + removed unused code parts
   + updated publish.bat

* Version 0.0.1.8
   + added bool isModded
   + split README in single documents for README, INSTALLATION and CHANGELOG

*  Version 0.0.1.7
   + ported code to new template format
   + moved config options BrowseAllPieceTables and AllowPortals to Portal category

* Version 0.0.1.6
   + Temporary disabling of the possibility to set the *noportals* global key, because I haven't found a reliable method for it yet. It sometimes just doesn't work or reports that the key has been set although it is not present in the global key list ingame.

* Version 0.0.1.5
   + added config for
      - force noportals global key (toggle)

* Version 0.0.1.4
   + added config for
      - plugin enabled (toggle)
      - show changes at startup (toggle)
      - browse all piece tables (toggle)
      - allow portals (dropdown) (None, Vanilla, Custom, All)
      - set noportals global key (toggle) (if *allow portals* set to *None*)

* Version 0.0.1.3
   + updated code to new template based format

* Version 0.0.1.2
   + updated to use Jotunn 2.6.7

* Version 0.0.1.1
   + unused code parts removed
   + checked against a bunch of portal and teleport mods
   + updated README.md

* Version 0.0.1.0
   + code for supporting all registered PieceTables reworked

* Version 0.0.0.9
   + List for easier extensibility added for
      - hammer piece tables to support custom hammers
      - portal prefab names to support custom portals
      - obsolete portal mods that create an error with the current Valheim version

* Version 0.0.0.8
   + added toggle showModErrorInfo
   + toggle showDetailedModInfo renamed to showModDetailedInfo
   + global key *noportals* is set automatically

* Version 0.0.0.7
   + Added check for other installed (incompatible) portal mods, if found, this mod will be disabled

* Version 0.0.0.6
   + set showModInfo to true

* Version 0.0.0.5
   + fix comments, no code changes

* Version 0.0.0.4
   + set showEventErrorInfo to true

* Version 0.0.0.3
   + Initial release.