# SGB IMod

![A screencap of the Unity Editor, demonstrating this plugin's SGB Manager tab](docs/example_image.png)

A heavy modification of [SMILE GAME BUILDER](https://store.steampowered.com/app/483950/SMILE_GAME_BUILDER/)'s [Unity Exporter](https://store.steampowered.com/app/766450/SMILE_GAME_BUILDER_Exporter_for_Unity/) Runtime that makes manipulating the runtime state easier, fixes some problems with the original runtime, and adds support for embedding multiple SGB projects in one parent Unity project.

This has been built primarily for Cobysoft Joe's [Dome-King Cabbage](https://cobysoft.co/) via the [Naninovel extension](https://github.com/BOBBO-NET/net.bobbo.sgb.imod.naninovel/) that uses this package. I don't really intend to be active on this project outside of features / fixes necessary for Dome-King - though if you find this useful and have problems with it, don't be afraid to drop a ticket in the Issues tab.

## Features

- Installable from [OpenUPM](https://openupm.com/packages/net.bobbo.sgb.imod/)
- Support for multiple SGB Unity Exports into a single project
- SGB projects can be dynamically entered and exited at runtime
  - Supports entry to the game as a whole, or a specific map
- Supports overriding how SGB reads / writes save file data streams
- Supports overriding what elements are available in SGB's pause menu
- Supports overriding what font SGB uses
- Supports manipulating SGB's volume settings and mixer routing
- Supports Unity's [Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.2/manual/index.html)
- Fixes visual visual bugs with the default SGB Unity Exporter Runtime
