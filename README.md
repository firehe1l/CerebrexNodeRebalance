# Cerebrex Node Rebalance

![RimWorld](https://img.shields.io/badge/RimWorld-1.6-blue)
![Odyssey](https://img.shields.io/badge/DLC-Odyssey-orange)
![Biotech](https://img.shields.io/badge/DLC-Biotech-green)

A RimWorld mod that transforms the Cerebrex Node into a true endgame reward worthy of conquering a mechanoid hive.

## ✨ Features

### 🔧 Enhanced Stats
- **+30 Mechanoid Bandwidth** (doubled from vanilla 15)
- **+50% Mechanoid Work Speed** (increased from vanilla 12%)

### 📦 Supply Drop Ability
Request mechanoid supply drops with a 2-3 day cooldown. Choose from:
- 350 Steel
- 100 Plasteel
- 20 Components
- 5 Advanced Components
- 25 Industrial Medicine

### 🕊️ Mechanoid Faction Neutralization
Automatically sets the Mechanoid faction to **Neutral** upon:
- Completing the Odyssey mechanoid quest
- Equipping the Cerebrex Node

---

## 📥 Installation

### Steam Workshop (Coming Soon)
*Subscribe to the mod and it will auto-install.*

### Manual Installation
1. Download the [latest release](../../releases)
2. Extract to `YourRimWorldFolder/Mods/`
3. Enable in RimWorld's mod manager
4. Place after **Core**, **Biotech**, and **Odyssey** in load order

---

## ⚙️ Requirements

- **RimWorld**: 1.6.4633+
- **Harmony**
- **DLC Required**:
  - Biotech
  - Odyssey

---

## 🔧 Compatibility

- ✅ Safe to add to existing saves
- ✅ Compatible with most mods
- ⚠️ Load **after** other mechanoid-related mods
- ❌ Do not use with other Cerebrex Node rebalance mods

---

## 🛠️ Technical Details

### Implementation
- **XML Patches**: Stat modifications via `PatchOperationReplace`
- **C# + Harmony 2.3.3**: Custom abilities and faction changes
- **Precompiled DLL**: No compilation required

### For Developers
Source code is included in the `Source/` folder. To rebuild:
1. Install .NET Framework 4.7.2 SDK
2. Update references in `CerebrexRebalance.csproj` to match your RimWorld install path
3. Build via Visual Studio or `dotnet build`

---

## 📝 Changelog

### v1.0.0 (2026-02-01)
- Initial release
- Cerebrex Node stat buffs
- Supply drop ability
- Mechanoid faction neutralization

---

## 🐛 Bug Reports

Found a bug? [Open an issue](../../issues) with:
- RimWorld version
- Mod list (export from mod manager)
- Steps to reproduce
- Log file (found in `RimWorld/Logs/`)

---

## 📜 License

This mod is released under the MIT License. See [LICENSE](LICENSE) for details.

---

## 🙏 Credits

- **Author**: Antigravity
- **Harmony Library**: [pardeike](https://github.com/pardeike/Harmony)
- **RimWorld**: Ludeon Studios

---

## ☕ Support

Enjoying the mod? Consider:
- ⭐ Starring this repository
- 🔗 Sharing with friends
- 💬 Leaving feedback in issues

---

**Made with ❤️ for the RimWorld community**
