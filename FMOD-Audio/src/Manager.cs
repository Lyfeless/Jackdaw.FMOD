using FMOD.Studio;

namespace Jackdaw.Audio.FMODAudio;

/// <summary>
/// A component responsible for loading and playing sound files from FMOD bank data.
/// Runs setup and update once it's added to the main actor tree.
/// </summary>
/// <param name="game">The current game instance.</param>
/// <param name="directory">The file path to load the bank files from, relative to the game's content root folder.</param>
public class AudioManager(Game game, FMOD.Studio.System instance) : Component(game) {
    public readonly FMOD.Studio.System FMODInstance = instance;

    protected override void Update() => FMODInstance.update();
    protected override void Invalidated() => FMODInstance.release();

    public static FMOD.Studio.System CreateFMODInstance() {
        FMOD.Studio.System.create(out FMOD.Studio.System instance);
        instance.initialize(1024, INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);
        return instance;
    }
}