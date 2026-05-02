## FMOD Integration
A loader and controller for using [FMOD](fmod.com) projects in Jackdaw.
This extension is still work-in-progress and may have breaking changes in the future.

### Library Files
This extension makes use of the FMOD and FMOD Studio api, which can't be included in this repo for licensing reasons. The appropriate library files can be found [here](https://www.fmod.com/download). The project will compile without them, but in order to run both api files need to be inside the build directory.

### Usage
The extension uses custom loaders to import audio-related assets. In order to work an `AudioLoader` instance needs to be added to `CustomAssetLoaders` in the game's content config, and an instance of the `AudioManager` component must be in the game's node tree.
```cs
// Get FMOD instance handle
FMOD.Studio.System FMODInstance = AudioManager.CreateFMODInstance();

// Create the game instance with a basic configuration
Game game = new(new GameConfig() {
    // ... Other game configuration
    Content = new() {
        CustomAssetLoaders = [
            new AudioLoader(FMODInstance, "Audio")
        ]
    }
});

// ... Game setup

game.Root.Components.Add(new AudioManager(game, FMODInstance));

// ... Continue game setup
```