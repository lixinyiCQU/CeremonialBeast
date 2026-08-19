# Ceremonial Beast

[English](README.md) | [简体中文](README.zh-CN.md)

![Ceremonial Beast](CeremonialBeast/mod_image.png)

Ceremonial Beast is an unofficial playable-character mod for **Slay the Spire 2**. It turns the original Ceremonial Beast monster into a full character built around ritual timing, volatile stat conversion, and card enchantments.

The project is currently in early development. Balance, localization, and compatibility may change as the game is updated.

## Features

- A complete playable character that reuses the original Ceremonial Beast scene, animations, and sound effects.
- **91 card entries**, including starter cards, generated cards, Ancient cards, and co-op cards.
- **10 relics**, **3 potions**, and **4 custom enchantments**.
- English and Simplified Chinese localization.
- Multiplayer-aware card effects and combat hooks.

## Core mechanics

- **Plow** grants temporary Strength and removes the same amount of temporary Dexterity at the start of your turn. Taking unblocked damage removes Plow.
- **Ringing** temporarily prevents normal card play. Many cards enter, consume, or specifically require this state, turning it into a timing resource.
- **Enchantments** modify cards with persistent or combat-only effects: Blessing, Inspire, Cultivate, and Accumulate.

## Requirements

### To play

- Slay the Spire 2 with mod support enabled.
- [BaseLib-StS2](https://github.com/Alchyr/BaseLib-StS2) 3.4.5 or newer.

### To build

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0).
- A local Slay the Spire 2 installation containing `sts2.dll` and `0Harmony.dll`.
- MegaDot/Godot 4.5.1 when exporting the `.pck` asset package.

## Installation

1. Install BaseLib in `<Slay the Spire 2>/mods/BaseLib/`.
2. Download and extract a Ceremonial Beast release.
3. Place the extracted folder in the game's `mods` directory.

The final layout should be:

```text
Slay the Spire 2/
└── mods/
    ├── BaseLib/
    │   ├── BaseLib.dll
    │   ├── BaseLib.json
    │   └── BaseLib.pck
    └── CeremonialBeast/
        ├── CeremonialBeast.dll
        ├── CeremonialBeast.json
        └── CeremonialBeast.pck
```

All players in a multiplayer lobby should use matching mod and dependency versions.

## Building from source

Clone the repository and enter its root directory:

```powershell
git clone <repository-url>
cd CeremonialBeast
```

The project auto-detects a default Steam installation. For a custom game or MegaDot location, create a local configuration:

```powershell
Copy-Item Directory.Build.props.example Directory.Build.props
```

Edit `Directory.Build.props`, then build the C# project:

```powershell
dotnet build CeremonialBeast.csproj -c Release
```

To export the Godot resources and produce `CeremonialBeast.pck`:

```powershell
dotnet publish CeremonialBeast.csproj -c Release
```

The project copies build output to `<Slay the Spire 2>/mods/CeremonialBeast/`.

## Project structure

| Path | Purpose |
| --- | --- |
| `CeremonialBeastCode/` | C# character, cards, powers, relics, potions, enchantments, and managers |
| `CeremonialBeast/` | Godot resources, artwork, UI assets, and localization |
| `CeremonialBeast/localization/eng/` | English localization |
| `CeremonialBeast/localization/zhs/` | Simplified Chinese localization |
| `CeremonialBeast.json` | Mod manifest |
| `Sts2PathDiscovery.props` | Cross-platform game path discovery |
| `Directory.Build.props.example` | Optional local path configuration template |

The source layout follows the conventions used by the [BaseLib character template](https://github.com/Alchyr/ModTemplate-StS2) and other open-source Slay the Spire 2 character mods.

## Compatibility

Slay the Spire 2 is in active development and game updates may change C# hooks or method signatures. Build this mod against the installed game's current `sts2.dll`. If the mod fails to load, check the latest game log under the Slay the Spire 2 user-data directory.

## Credits and disclaimer

- [Mega Crit](https://www.megacrit.com/) for Slay the Spire 2 and the original Ceremonial Beast assets.
- [Alchyr/BaseLib-StS2](https://github.com/Alchyr/BaseLib-StS2) for the content framework and character-mod support.
- The Slay the Spire 2 modding community and open-source templates used as structural references.

This is a non-commercial fan project. It is not affiliated with or endorsed by Mega Crit. Slay the Spire 2 and its original assets belong to their respective rights holders.
