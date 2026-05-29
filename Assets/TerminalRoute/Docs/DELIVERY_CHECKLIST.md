# Terminal Route Delivery Checklist

## Playable Demo

- Open `Assets/Scenes/Menu.unity`.
- Press Play.
- Confirm `Nova Viagem` starts the route.
- Confirm `Creditos` opens from the main menu, and `C` opens it too.
- Confirm `A/D` or arrow keys steer only left/right.
- Confirm `F` toggles mirror view.
- Confirm the steering wheel visibly turns when steering.
- Confirm stops pause the bus and continue automatically.
- Confirm at least one ending appears.

## Builds

- Use `Terminal Route > Setup > Apply MVP Player Settings`.
- Use `Terminal Route > Setup > Rebuild Route Editor Preview` if the Route scene preview is missing.
- Use `Terminal Route > Assets > Extract Pack Children To Prefabs` if you want real pack pieces as draggable prefabs.
- Use `Terminal Route > Build > All MVP Builds`.

Or build separately:

- Use `Terminal Route > Build > Windows MVP`.
- Use `Terminal Route > Build > WebGL MVP`.

Expected outputs:

- `Builds/Windows/TerminalRoute.exe`
- `Builds/WebGL/`

## Package

- Include Windows build.
- Include WebGL build or uploadable WebGL folder.
- Include `Assets/TerminalRoute/Docs/MVP_README.md`.
- Include `Assets/TerminalRoute/Docs/ASSET_CREDITS.md`.
- Confirm the credits screen shows Nero Soares & Paulo Monteiro.
- Confirm itch.io asset URLs/licenses are present in `ASSET_CREDITS.md`.

## Last-Minute Cut Rule

If time runs out, keep the playable build stable. Cut polish before cutting menu, route, mirror, episodes, endings, or builds.
