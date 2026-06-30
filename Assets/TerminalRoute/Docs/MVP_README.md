# Terminal Route MVP

This Unity prototype implements the May 29 first-playable target:

- Portuguese main menu, HUD, and ending screens.
- Main-menu credits screen with `C` shortcut.
- Separate `Menu`, `Route`, and `Ending` Unity scenes with generated PS1-horror menu and cockpit artwork.
- Auto-driving bus with left/right steering only.
- Road bounds failure.
- Timed stops that recover sanity and advance the route loop.
- Passenger count and cabin passengers thin out at each completed stop.
- Internal sanity state that changes steering and visual degradation.
- Rear-view mode on `F` or right mouse button.
- MVP episodes: `O Silencio`, `O Macaco`, and `A Bola`.
- MVP endings: `BOA VIAGEM` and `A LONGA ROTA`.
- Runtime route polish: dark camera clear, headlights, fog, roadside silhouettes, stops, lamps, terminal gate, warning HUD, and dynamic engine/drone audio.

## Controls

- `A` / Left Arrow: steer left.
- `D` / Right Arrow: steer right.
- `F` / Right Mouse: toggle rear view.
- `C`: credits from main menu.
- `Enter`: start from menu or return after an ending.
- `Escape`: force the long-route ending during a run.

## Assets And Audio

The MVP uses a safe hybrid approach:

- Core gameplay geometry is generated in Unity at runtime from primitives to avoid late broken prefabs.
- Imported asset packs provide textures, PSX passenger models, and visual reference.
- The `Route` scene also has an editor-only static preview builder so the scene view is not empty before Play.
- Use `Terminal Route > Assets > Extract Pack Children To Prefabs` to split imported FBX/DAE child objects into draggable prefabs under `Assets/TerminalRoute/ExtractedPrefabs`.
- Generated menu/cockpit/ending images are stored under `Assets/Resources/TerminalRoute/Art`.
- Audio clips are synthesized in code by `TerminalRouteAudio`.

Track third-party sources in `Assets/TerminalRoute/Docs/ASSET_CREDITS.md` before submission.

## Builds

Open the Unity editor and use:

- `Terminal Route > Setup > Apply MVP Player Settings`
- `Terminal Route > Build > All MVP Builds`

Or build separately:

- `Terminal Route > Build > Windows MVP`
- `Terminal Route > Build > WebGL MVP`

For normal playtesting, open `Assets/Scenes/Menu.unity` and press Play.

Expected outputs:

- `Builds/Windows/TerminalRoute.exe`
- `Builds/WebGL/`

## Unity Version

This project is now hosted in Unity `6000.3.8f1` with URP `17.3.0`. Runtime controls use Unity's built-in input backend so the clean course project does not require the optional Input System package.

After opening the project in Unity 6, allow the URP material upgrade/reimport prompts to finish before judging the visuals.
If Unity shows the Auto Graphics API notice during migration, confirm it; Windows graphics APIs are pinned in Project Settings so the demo does not depend on Unity's changing default.
