using FMOD.Studio;

namespace Jackdaw.Audio.FMODAudio;

/// <summary>
/// A component responsible for loading and playing sound files from FMOD bank data.
/// Runs setup and update once it's added to the main actor tree.
/// </summary>
public class AudioManager : Component {
    public readonly FMOD.Studio.System FMODInstance;

    /// <param name="game">The current game instance.</param>
    /// <param name="directory">The file path to load the bank files from, relative to the game's content root folder.</param>
    public AudioManager(Game game) : base(game) {
        FMOD.Studio.System.create(out FMODInstance);
        FMODInstance.initialize(1024, INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);
    }

    protected override void Update() {
        FMODInstance.update();
    }

    protected override void Invalidated() {
        FMODInstance.release();
    }
}