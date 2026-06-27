# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Casino VR — a Unity VR slot-machine demo built on the ValemVR Game Jam Template, targeting **Meta Quest 2** standalone (Android). Academic project (UTEC CS2H01). UI strings, log messages, and code comments are in **Spanish** — preserve the language when editing them.

## Toolchain

- **Unity 2022.3.62f3** (LTS) — see `ProjectSettings/ProjectVersion.txt`. The README mentioning "Unity 6" in script headers is aspirational; the actual editor target is 2022.3.
- **Render pipeline:** URP 14.0.12.
- **XR stack:** `com.unity.xr.interaction.toolkit` 2.6.4, `com.unity.xr.oculus` 4.5.1, `com.unity.xr.openxr` 1.14.3. Android Build Support is required to deploy to Quest.
- **MCP:** `com.coplaydev.coplay` is in `Packages/manifest.json` — the `mcp__coplay-mcp__*` tools talk to a running Unity Editor instance for this project. Use them for scene/asset operations instead of editing `.unity`/`.prefab` YAML by hand.

There are no CLI build/test scripts: open the project in the Unity Editor, switch the build target to Android, and build to APK for Quest. The `*.csproj` / `*.sln` at the root are Unity-regenerated — don't edit them.

## Architecture

### Scene flow

Two scenes, both in `Assets/Scenes/`, registered as build indices 0 and 1:

1. `1 Start Scene.unity` — main menu (`GameStartMenu.cs`).
2. `2 Game Scene.unity` — casino + slot machine.

Transitions go through `SceneTransitionManager.singleton.GoToSceneAsync(int)` which fades via a `FadeScreen` reference. Scene index `1` is **hardcoded** in `GameStartMenu.StartGame()`; if you reorder scenes in Build Settings, update that call.

### Slot machine (core gameplay)

The slot machine is split into two cooperating MonoBehaviours, plus a session-stats singleton:

- **`SlotMachineController.cs`** owns game logic: credits, bets, result generation, payout table, spin orchestration. The entry point is `TrySpin()` — wire it to the `SelectEntered` event of the spin button's `XRSimpleInteractable`.
- **`ReelStrip.cs`** owns one reel's visuals. It procedurally builds a vertical strip of quads at `Awake()` (`visibleRows + 2` quads, the extras are top/bottom buffers), scrolls them downward, recycles off-screen quads back to the top with random symbol materials, and on `Stop(idx)` decelerates and snaps the chosen material to the center row.
- **`SessionManager.cs`** (`DontDestroyOnLoad` singleton) records spins, win/loss streaks, biggest win, jackpots, and session duration for the end-of-session summary UI.

`SlotMachineController.SpinRoutine` is the canonical sequence: compute result up front → call `Spin()` on all reels → wait `spinDuration` → call `Stop(result[i])` on each reel with `staggerDelay` between them → wait for the last reel's `IsSpinning` to drop → pay out and report to `SessionManager`.

**Test cheat — remove before release:** `SlotMachineController.GenerateResult()` forces a win every 5th spin (`_spinCount % 5 == 0`). The comment in the file flags this for removal.

**Symbol-index inconsistency to watch:** Three places disagree about what index → which symbol:
- `SlotMachineController.payMultipliers` tooltip: `[0]=BAR [1]=Seven [2]=Cherry [3]=Lemon`
- `SlotMachineController.CalculatePayout` switch: `[0]=BAR [1]=Cherry [2]=Lemon [3]=Seven`
- `ReelStrip.symbolMaterials` tooltip: `[0]=BAR [1]=Seven [2]=Cherry [3]=Lemon`

`CalculatePayout` hardcodes its own per-symbol payout table and **ignores `payMultipliers`** entirely (the `GetMult` helper is dead code). When editing payouts, edit the `switch` in `CalculatePayout`, and keep the symbol order in `ReelStrip.symbolMaterials` aligned with what the switch assumes.

### Singletons

Three singletons coexist with different lifetimes — don't conflate them:
- `SessionManager.Instance` — `DontDestroyOnLoad`, persists across scenes.
- `AudioManager.instance` — `DontDestroyOnLoad`, plays named clips from a `Sound[]` inspector array.
- `SceneTransitionManager.singleton` — **does not** `DontDestroyOnLoad`; lives only in the current scene. Each scene that needs transitions must contain a `Transition Manager` prefab instance.

### Assets layout

Game-specific code lives in `Assets/Scripts/`. Everything else under `Assets/` (`XR/`, `XRI/`, `XRI_Examples/`, `Samples/`, `Andtech/`, the various model packs) is third-party / template content — prefer adding new scripts to `Assets/Scripts/` rather than modifying vendored folders.
