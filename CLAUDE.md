# CLAUDE.md — TankEvaporation

Git rules come from the parent folders. This file covers only what's specific to this mod.

---

## What It Does

Liquid tanks lose a percentage of their **maximum** capacity each day, so large reservoirs stop being free storage. Settings are exposed in-game through the Mod Settings mod:

- **Evaporation rate** — 1–20%, default 5%
- **Water only** — default **on**. Applying evaporation to other liquids was too punishing in playtesting, since they can't be replaced as quickly.

Published on Steam Workshop. Source at `github.com/dcasez/TankEvaporation`.

---

## Files

| File | Does |
|---|---|
| `EvaporationService.cs` | Fires on `DaytimeStartEvent`, drains tanks once per day |
| `EvaporationFragment.cs` | Tank inspect panel — daily leak, days until empty |
| `TankEvaporationSettings.cs` | The two mod settings |
| `EvaporationPanelModule.cs` | Registers the fragment |
| `manifest.json` | Version number and dependencies |
| `Localizations\enUS.csv` | All UI text |

---

## Dependencies

Both required, both must be installed from Steam Workshop before building:

- **Harmony** — Workshop ID `3284904751`
- **Mod Settings** — Workshop ID `3283831040`, mod ID `eMka.ModSettings`

---

## Build and Test

Build **Release**, not Debug. Output lands in `bin\Release\netstandard2.1\`.

Install path: `C:\Users\DCase\Documents\Timberborn\Mods\TankEvaporation\`

Copy only what changed:

| Changed | Copy |
|---|---|
| Any `.cs` file | `TankEvaporation.dll` |
| UI text | `Localizations\enUS.csv` |
| Version or dependencies | `manifest.json` |

I test in game myself. Build and copy, then tell me what to check and wait.

---

## Releasing

In order:

1. Bump `Version` in `manifest.json`
2. Rebuild, copy to the mods folder, test in game
3. Commit and push to `main`
4. GitHub release, tagged `vX.Y.Z`, with the DLL, `manifest.json` and `enUS.csv` attached
5. Steam Workshop update from the in-game mod uploader — back up the description text before touching it, and add a changelog line at the top

Steam Workshop has no versioning of its own. The number in `manifest.json` is the only version anyone sees.

---

## Things That Have Bitten Us

**Evaporation once drained warehouses.** The drain loop hit every building with an inventory, not just tanks. Fixed by checking for a `SingleGoodAllower` component, plus a guard against reserved stock. Any change to the drain loop needs re-testing against a warehouse holding logs or food.

**Do not identify tanks by prefab name.** Checking for "Tank" in the building name is fragile — it breaks on modded tanks and on game updates. Use component checks.

**Verify class names before writing code.** This project failed repeatedly early on by guessing at the API. Confirmed names as of Timberborn 1.0: `Inventory`, `SingleGoodAllower`, `GoodAmount`, `BlockableObject`, `DaytimeStartEvent`, `EventBus` with `[OnEvent]`, `EntityRegistry`, `IModStarter`, `IModEnvironment`, `RangeIntModSetting`, `ModSettingsOwner`. Water's good ID is the string `"Water"`.

**None of the above is verified against Timberborn 1.1.** The 1.1 update moved things in the component system. Treat every name in this file as unconfirmed until re-checked with `ilspycmd` against the current game DLLs.
