# Resonite for macOS

This repo contains scripts, patches & a custom engine runner for running Resonite on macOS using the Renderide renderer.

This assumes you're running on an Apple Silicon Mac. Intel Macs are not supported.

## What Works
 - [x] Basic engine bring up
 - [x] Renderer IPC
 - [x] Renderide
 - [x] Audio
    - [x] Microphone & voice transmit
    - [x] Audio output (Ensure Audio > Playback Buffer Size is at least Medium1024. Anything else is broken on anything except Windows)
    - [x] Hear others (see above)
 - [ ] Crunch Image Compression, without this, images won't be visible to others until you go to an inspector, and ensure CrunchCompress is turned off. This also might be why a bunch of textures are really low resolution
 
 - [ ] Platform integration
    - [x] System info (identifies as Platform.OSX, shows correct CPU & GPU model in logs)
    - [x] Clipboard
    - [ ] Saving screenshots to disk doesn't seem to work
    - [ ] Drag and drop (this would require work on Renderide side)

## You will need:

- A copy of Resonite downloaded from Steam (either copied from a Windows machine, or downloaded using [DepotDownloader](https://github.com/SteamRE/DepotDownloader/) or [SteamCMD](https://developer.valvesoftware.com/wiki/SteamCMD))
- A built copy of [Renderide](https://github.com/DoubleStyx/Renderide)
- The .NET 10 SDK installed.
- Homebrew installed.

## Instructions

0. Despite the precompiled binaries in `binaries/` I was too lazy to recompile FreeType. Install using Brew: `brew install freetype`
1. Clone this repo
2. Run `./run.sh`
   - This will expect that:
       - A copy of Resonite can be found in the `resonite/` directory next to the script.
       - A copy of Renderide can be found in the `renderide/` directory next to the script, and that it is was built with with `cargo build --profile dev-fast` (thus, `/renderide/target/dev-fast/renderide-renderer` exists)
   - If this is wrong, edit the variables at the top of `run.sh` to point to the correct locations.

## Installing [ResoniteModLoader](http://github.com/resonite-modding-group/ResoniteModLoader/)

You should _not_ follow the steps normally used to install ResoniteModLoader. (as the runner already comes with Harmony, and uses different paths)

Just download the [latest ResoniteModLoader.dll](https://github.com/resonite-modding-group/ResoniteModLoader/releases/latest/download/ResoniteModLoader.dll) and place it in the same folder as the run.sh script.
 

## Native libraries

This repo contains a few native libraries recompiled for macOS (of questionable providence). There are some instructions in `README.natives.md`
