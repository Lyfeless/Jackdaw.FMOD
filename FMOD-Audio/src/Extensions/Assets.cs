using FMOD.Studio;

namespace Jackdaw.Audio.FMODAudio;

public static class AssetsExtensions {
    extension(Assets assets) {
        public SoundEvent GetSoundEvent(string name) => assets.Get<SoundEvent>(name);
        public Bus GetBus(string name) => assets.Get<Bus>(name);
    }
}