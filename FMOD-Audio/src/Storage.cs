using FMOD.Studio;
using Foster.Framework;

namespace Jackdaw.Audio.FMODAudio;

public class AudioStorage(FMOD.Studio.System instance) : IAssetStorage {
    internal readonly FMOD.Studio.System FMODInstance = instance;
    internal readonly Dictionary<string, EventDescription> Events = [];

    internal EventDescription Fallback = new();

    public void Add(string name, object asset) { }

    public object Get(string name) {
        if (Events.TryGetValue(name, out EventDescription output)) { return new SoundEvent(output); }
        Log.Warning($"ASSETS: Failed to find SoundEvent \"{name}\", returning default");
        return GetFallback();
    }

    public string[] GetAssetNames() {
        throw new NotImplementedException();
    }

    public object GetFallback()
        => new SoundEvent(Fallback);

    public void SetFallback(object asset) { }
}