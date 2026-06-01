# Terminal Route - Demo Presentation Guide

Presentation date: June 1, 2026

## One-Sentence Pitch

Terminal Route is a PS1-style horror driving demo where the player drives a night bus through a repeating route, manages stops, checks the mirror, and survives strange events inside and outside the bus.

## Short Opening Script

"For this prototype, we focused on making a playable horror loop rather than a full final game. The player is a bus driver on a night route. The controls are intentionally simple: the bus moves forward automatically, and the player only steers, checks the mirror, and controls the doors. The horror comes from timing, visibility, passengers, and the fear of looking away from the road."

## What The Demo Shows

- A complete playable loop from menu to win/loss ending.
- Portuguese UI with optional English toggle in the menu.
- A low-poly PS1/VHS horror style.
- A bus cockpit view with steering, HUD, mirror, passengers, stops, and route events.
- Eight required bus stops.
- Failure conditions: missing stops, leaving the road, hitting oncoming buses, sanity loss, and looking too long at the close mirror passenger.
- Win condition: complete all eight stops.
- Result screen with stats: stops, missed stops, sanity, time, and ending cause.

## Controls

- `A / D` or arrow keys: steer left and right.
- `F`: toggle mirror / look back.
- `E`: open or close doors.
- `Enter`: start from menu / return from ending.
- `C`: credits screen.
- `T`: controls screen.
- Menu button: fullscreen.

Important explanation:
The player cannot accelerate or brake manually. The bus auto-drives. The challenge is positioning, reaction, and deciding when to look away from the road.

## Recommended Live Demo Flow

1. Start on the main menu.
   - Point out the title art, language toggle, controls, credits, and fullscreen button.

2. Open the controls screen briefly.
   - Explain that the game is intentionally simple to learn.

3. Start the route.
   - Mention that the bus automatically moves forward.
   - Steer gently and show the cockpit, HUD, and road.

4. Approach the first stop.
   - Explain the green stop zone.
   - Move right near the curb.
   - Let the bus slow down and stop.
   - Show doors opening and passengers boarding.

5. Show the mirror once.
   - Explain that the mirror is useful but dangerous.
   - If the close passenger appears, immediately turn forward to demonstrate the reaction mechanic.

6. Continue to later events if time allows.
   - Mention the silence/passenger event.
   - Mention the monkey/ball/inverted controls/lights-out events.
   - Point out oncoming buses and road hazards.

7. Either finish or intentionally trigger a failure.
   - A controlled failure is okay if time is short.
   - The result screen proves the game has proper ending states and stats.

## Key Mechanics To Explain

### Auto-Driving Bus

The player is a driver, but not a racing driver. The bus moves by itself at route speed. This makes the player focus on lane control, stops, mirror checks, and horror interruptions.

### Stop System

There are eight required stops. The player must move close to the right-side stop zone. If the player passes a stop too far from the curb, it counts as a miss. Missing two stops triggers a loss.

### Doors

Doors can be opened or closed with `E` or the in-game button. At stops, the doors open automatically and close again when the bus leaves. This makes the bus stop moment feel intentional instead of instant.

### Mirror

The mirror is used to reveal horror events behind the driver. Looking back can cost sanity. Sometimes a passenger appears very close to the camera; the player must turn forward quickly or lose.

### Sanity

Sanity is hidden from the normal HUD. It affects control, visual instability, and danger. The player feels it through steering drift, distortion, and route pressure rather than a normal health bar.

### Events

The demo includes several MVP events:

- `O Silencio`: passengers freeze and stare when checked in the mirror.
- `O Macaco`: a monkey appears and moves closer after mirror checks.
- `A Bola`: a ball rolls through the bus and vanishes when observed.
- Inverted controls: left and right steering swap temporarily.
- Lights out: the bus lights fail, visibility drops, and the HUD becomes more stressful.
- Wrong passenger count: the HUD passenger count becomes unreliable during creepy moments.

### Hazards

The road has cones, crates, tires, road patches, roadside figures, and oncoming buses. Some hazards cost sanity. Oncoming bus collision is a loss condition.

## What Is Procedural / Code-Generated

Most of the route is assembled by scripts:

- Road segments.
- Guardrails.
- Stop zones.
- Bus stops and waiting passengers.
- Roadside trees/buildings/silhouettes.
- Road clutter and hazards.
- Bus cockpit overlays and runtime UI.

This helped keep the scope realistic for a short university demo.

## External Assets Used

Asset sources:

- Characters PSX by Elbolilloduro on itch.io.
- Bus Stop by Elbolilloduro on itch.io.
- Roads Procedural by Elbolilloduro on itch.io.

Credits are shown in-game, and asset URLs/licenses are tracked in `ASSET_CREDITS.md`.

## Design Choices

### Why PS1 Style?

The PS1 style helps the horror mood and keeps production feasible. Lower detail means the player fills in the gaps, which can make the game feel creepier.

### Why Simple Controls?

The project goal is tension, not driving simulation. By removing acceleration and braking, the player has fewer buttons but more pressure from timing and positioning.

### Why Portuguese UI?

The course documentation and presentation context use Portuguese, so the game defaults to Portuguese. English is included for accessibility and testing.

## Technical Overview

- Engine: Unity 2022.3 LTS.
- Rendering: URP.
- Target platforms: Windows and WebGL.
- Input: keyboard.
- Assets: low-poly/procedural Unity primitives plus imported PSX asset packs.
- Audio: runtime synthesized clips for engine, drone, chimes, hits, doors, and events.
- Scenes: Menu, Route, Ending.

Important systems:

- `TerminalRouteGame`: main runtime coordinator.
- `BusController`: auto-driving, steering, lane danger, road failure.
- `RouteManager`: stop timing, stop validation, missed stops.
- `EpisodeManager`: horror events and loop state.
- `MirrorThreatManager`: close mirror passenger reaction event.
- `RoadHazardManager`: road clutter collision/sanity hits.
- `SceneFactory`: builds the bus, route, stops, passengers, and world dressing.
- `TerminalRouteUi`: menu, HUD, credits, controls, endings, language toggle.

## If Something Goes Wrong During The Presentation

### If WebGL is slow

Say: "The Windows build is the primary stable build. WebGL is included for accessibility but Unity WebGL can be heavier depending on browser and machine."

### If the player misses stops

Move right earlier. The green zone is on the right side. The bus must be close to the curb before reaching the stop.

### If the mirror event kills you

That is expected. The mechanic is: if a close passenger appears, press `F` quickly to look forward.

### If audio is too low

Mention that the audio is currently synthesized in Unity for the prototype and will be replaced or expanded later.

### If the game ends abruptly

Explain that the demo now shows a short route-end message and then loads the result scene with stats and cause.

## Likely Teacher Questions

### "What is the objective?"

Complete eight stops without missing two, crashing, losing sanity, or failing mirror events.

### "What makes this a horror game?"

The player is trapped in a routine night route. Horror comes from the mirror, passengers, sound, low visibility, wrong information, and sudden route events.

### "What did you prioritize?"

A playable vertical slice: menu, route, core driving, stops, events, fail states, endings, credits, and WebGL/Windows viability.

### "What is missing for the final game?"

More polished audio, more authored events, better animation, more endings, final poster/site/video/GDD package, and stronger environmental storytelling.

### "Why not use realistic assets?"

The schedule was short, so the demo uses low-poly PS1-style assets and procedural assembly. That keeps production realistic and supports the horror aesthetic.

## Suggested Closing

"This demo proves the core loop: drive, stop, check the mirror, survive the route, and reach a result screen. The next step would be expanding the number of events, improving animation/audio, and turning the playable prototype into the full final package with poster, site, video, and updated GDD."

## Final Rehearsal Checklist

- Test menu buttons: New Trip, Controls, Credits, Language, Fullscreen.
- Test first stop: move right, stop in green zone, doors open.
- Test mirror: press `F`, return with `F`.
- Test doors: press `E`.
- Test one loss condition if needed: miss two stops or keep looking during close passenger.
- Test ending/result screen.
- Have Windows build ready as backup if WebGL has browser issues.
