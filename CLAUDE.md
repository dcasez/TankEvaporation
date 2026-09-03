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

The csproj references Mod Settings at `version-1.1\Scripts\*.dll` — the build the game actually loads on 1.1. It also publicizes all Timberborn/Bindito references, which matters when a signature question comes up: see the publicizer note in the workspace CLAUDE.md.

---

## Build and Test

Build **Release**, not Debug. Output lands in `bin\Release\netstandard2.1\`.

Install path: `C:\Users\DCase\Documents\Timberborn\Mods\TankEvaporation\version-1.1\`

Copy only what changed:

| Changed | Copy |
|---|---|
| Any `.cs` file | `TankEvaporation.dll` |
| UI text | `Localizations\enUS.csv` |
| Version or dependencies | `manifest.json` |

I test in game myself. Build and copy, then tell me what to check and wait.

---

## Packaging — Two Version Folders

One Workshop item serves both game versions:

| Folder | manifest `Version` | `MinimumGameVersion` | DLL |
|---|---|---|---|
| `version-1.0\` | `1.0.2` | `1.0.0.0` | **frozen** |
| `version-1.1\` | `1.1.0` | `1.1.0.0` | current build |

Same `Id` (`TankEvaporation`) in both. Each folder needs its own `manifest.json`, DLL and `Localizations\` (currently byte-identical between the two). The item root keeps only `thumbnail.jpg` and `workshop_data.json`.

**The 1.0 DLL can never be rebuilt** — the 1.0 `Managed` folder no longer exists. The only copy is preserved at `..\Modding\artifacts\1.0-build\` (md5 `480ba1e3...`). Never overwrite or delete it. A 1.0-branch bug is unfixable; that folder exists to keep 1.0 players working, not to be maintained.

See the workspace CLAUDE.md for how the loader picks a folder and how to confirm which one it chose.

---

## Releasing

In order:

1. Bump `Version` in `manifest.json`
2. Rebuild, copy to the mods folder, test in game
3. Commit and push to `main`
4. GitHub release, tagged `X.Y.Z` — bare number, no `v` prefix (matches `1.0.1`, `1.0.2`, `1.1.0`), with the DLL, `manifest.json` and `enUS.csv` attached
5. Steam Workshop update from the in-game mod uploader — back up the description text before touching it, and add a changelog line at the top

Steam Workshop has no versioning of its own. The number in `manifest.json` is the only version anyone sees.

---

## Things That Have Bitten Us

**Evaporation once drained warehouses.** The drain loop hit every building with an inventory, not just tanks. Fixed by checking for a `SingleGoodAllower` component, plus a guard against reserved stock. Any change to the drain loop needs re-testing against a warehouse holding logs or food.

**Do not identify tanks by prefab name.** Checking for "Tank" in the building name is fragile — it breaks on modded tanks and on game updates. Use component checks.

**`Inventory.Take` was removed in 1.1.** It split into `TakeConsumed` / `TakeExported` / `TakeExisting`, each passing a `StockChangeType` to the `InventoryStockChanged` event. **Use `TakeExisting`** — it passes `StockChangeType.None`. `TakeConsumed` passes `Consumed`, which `DistrictGoodsBalance` adds to its consumption registry, feeding `GoodsSampler` and the in-game goods statistics UI. Evaporation is not consumption by a building, so recording it there would inflate a player-visible water consumption figure.

**Verify class names before writing code.** This project failed repeatedly early on by guessing at the API. The following are **confirmed against 1.1.2.4** by decompiling and by a clean build:

`Inventory` (`AmountInStock`, `UnreservedAmountInStock`, `Capacity`, `TakeExisting`), `SingleGoodAllower` (`HasAllowedGood`, `AllowedGood`), `GoodAmount(string, int)`, `BlockableObject.IsUnblocked` (in `Timberborn.BlockingSystem`), `EntityRegistry.Entities` → `ReadOnlyList<EntityComponent>`, `DaytimeStartEvent`, `EventBus.Register`/`Unregister` with `[OnEvent]`, `IEntityPanelFragment` (`ShowFragment` still takes `BaseComponent`), `EntityPanelModule.Builder`, `ILoc.T`, `IModStarter`, `IModEnvironment`, and `RangeIntModSetting` / `ModSettingsOwner` from Mod Settings 1.1. Water's good ID is still the string `"Water"`.

Re-check all of it after the next game update. `IStartableComponent` was deleted outright in 1.1 — the component lifecycle now has only `IAwakableComponent`, `IUpdatableComponent` and `ILateUpdatableComponent` — so outright removals do happen, not just renames.
