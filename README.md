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
- Approve / Deny SGB loading maps for custom inter-scene navigation

## Usage

### Importing an SGB Project

1. Export an SGB Project using the Unity Exporter.
2. Open the exported Unity project at least once, with the same version of Unity that you'll use in your current project. This generates necessary assets.
3. In the project that you'd like to import the project into, open the SGB Manager window by navigating to SGB_IMOD -> SGB Manager on the top of the Unity Editor.
4. Click the Import button to open the SGB Import Wizard. Follow steps in the wizard to complete!

### Entering / Exiting an SGB Project at Runtime

SGB IMod exposes a class named SGBManager to help with this.

```C#
// Enter the title screen of the game
await SGBManager.LoadSmileGameAsync("theNameOfTheGame");

...

// Load the first save file in the game
await SGBManager.LoadSmileGameAsync("someGame", 0);

...

// Load into the 2nd save file, at a specific map location
var mapLoadParams = new LoadSGBMapArgs {
    MapName = "the_shack",
    StartPosition = new Vector2Int(
        MapStartPositionX.Value, 
        MapStartPositionY.Value
    ),
    StartDirection = MapStartDirection.Value,
    StartHeight = MapStartHeight.Value
};
await SGBManager.LoadSmileGameAsync("demoGame", 1, mapLoadParams);

...

// Exit the currently loaded SGB game
await SGBManager.UnloadSmileGameAsync();
```

### Controlling Pause Menu Options

```C#
// Make the Exit Button invisible and non-interactable
SGBPauseMenuOptions.ExitButton.IsVisible = false
SGBPauseMenuOptions.ExitButton.IsInteractable = false

...

// Make the Save Button visible, but not-interactable
SGBPauseMenuOptions.SaveButton.IsVisible = true
SGBPauseMenuOptions.SaveButton.IsInteractable = false

...

// For more details on what pause menu options can be controller, check the properties in `Runtime/Scripts/SGBPauseMenuOptions.cs`
```
