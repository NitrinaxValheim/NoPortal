# NoPortal

- The mod automatically removes all known portal pieces from all registered PieceTables.
(Admins can still spawn it.)
- The mod automatically sets the global key noportals.

# Usage

No manual interaction is required.

# Installation

1. Install [BepInEx for Valheim](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/)
2. Install [Jotunn](https://valheim.thunderstore.io/package/ValheimModding/Jotunn/)
3. Place the complete folder `NoPortal` or the `NoPortal.dll` in your BepInEx plugins directory, like this: `Steam\steamapps\common\Valheim\BepInEx\plugins\NoPortal.dll`.

# Miscellaneous

This mod needs to be installed on each client, not the server.

# Configuration

No configuration added currently.

# Unaffected Mods

* list of mods that are not affected by NoPortal

* HookGenPatcher created
    - MMHOOK_assembly_googleanalytics
    - MMHOOK_assembly_guiutils
    - MMHOOK_assembly_lux
    - MMHOOK_assembly_postprocessing
    - MMHOOK_assembly_simplemeshcombine
    - MMHOOK_assembly_steamworks
    - MMHOOK_assembly_sunshafts
    - MMHOOK_assembly_utils
    - MMHOOK_assembly_valheim

 * sinai-dev-UnityExplorer
    - UniverseLib.Mono
    - Tomlet
    - mcs

 * misc
    - Newtonsoft.Json
    - YamlDotNet

# Compatible with

* list of compatible mods that
    - have no effect on NoPortal
    - or simply can not be used because the mod NoPortal is installed

* dev mods
    - XRayVision
    - UnityExplorer.BIE5.Mono

* portal mods
    - Multiverse
    - AnyPortal
    - AsgardPortal
    - ColorfulPortals
    - ProperPortals
    - DEF_handy_portals
    - ImmersivePortals
    - IndestructiblePortals
    - TeleportEverything
    - PreventAccidentalInteraction
    - PortalMarker
    - RareMagicPortal
    - Resurrection
    - TargetPortal
    - AnyPortal
    - UniversalPortalsMod
    - UnrestrictedPortals
    - FasterTeleportation
    - ValheimFastTeleport
    - TeleportableWagon
    - TeleporterTitle

* other mods
    - EnhancedPotions

# Incompatible with

* list of incompatible mods that
    - do not work with NoPortal
    - or generate errors when NoPortal is installed
    - or allows teleportation without portals, which is contrary to the purpose of NoPortal

* incompatible mods
    - MapTeleporter
    - MapTeleport

If one of this mods installed, NoPortal wil be disabled.

# Note

If you found more mods that are incompatible, feel free to report them as issue at [Github](https://github.com/NitrinaxValheim/NoPortal/issues).

# Changelog

* Version 0.0.1.6
    * Temporary disabling of the possibility to set the 'noportals' global key, because I haven't found a reliable method for it yet. It sometimes just doesn't work or reports that the key has been set although it is not present in the global key list ingame.

* Version 0.0.1.5
    * added config for
        - force noportals global key (toggle)

* Version 0.0.1.4
    * added config for
        - plugin enabled (toggle)
        - show changes at startup (toggle)
        - browse all piece tables (toggle)
        - allow portals (dropdown) (None, Vanilla, Custom, All)
        - set noportals global key (toggle) (if 'allow portals' set to 'None')

* Version 0.0.1.3
    * updated code to new template based format

* Version 0.0.1.2
    * updated to use Jotunn 2.6.7

* Version 0.0.1.1
    * unused code parts removed
    * checked against a bunch of portal and teleport mods
    * updated README.md

* Version 0.0.1.0
    * code for supporting all registered PieceTables reworked

* Version 0.0.0.9
    * List for easier extensibility added for
        - hammer piece tables to support custom hammers
        - portal prefab names to support custom portals
        - obsolete portal mods that create an error with the current Valheim version

* Version 0.0.0.8
    * added toggle showModErrorInfo
    * toggle showDetailedModInfo renamed to showModDetailedInfo
    * global key "noportals" is set automatically

* Version 0.0.0.7
    * Added check for other installed (incompatible) portal mods, if found, this mod will be disabled

* Version 0.0.0.6
    * set showModInfo to true

* Version 0.0.0.5
    * fix comments, no code changes

* Version 0.0.0.4
    * set showEventErrorInfo to true

* Version 0.0.0.3
    * Initial release.

# Source code

Can be found at [Github](https://github.com/NitrinaxValheim/NoPortal).

# Contributions are welcome

* bug fixes
* adding compatibility with another mod
* For new features, please contact me first, by opening an issue on [Github](https://github.com/NitrinaxValheim/NoPortal/issues) and explaining what you intend.
