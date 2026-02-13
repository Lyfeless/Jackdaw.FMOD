using FMOD.Studio;

namespace Jackdaw.Audio.FMODAudio;

public class AudioLoader(FMOD.Studio.System instance, string path) : AssetLoaderStage {
    const string BANK_EXTENSION = ".bank";
    const string STRINGS_EXTENSION = ".strings.bank";

    readonly string FilePath = path;
    readonly FMOD.Studio.System FMODInstance = instance;

    public override void Run(Assets assets) {
        if (!Path.Exists(FilePath)) { return; }

        AudioStorage storage = new(FMODInstance);

        IEnumerable<string> files = Assets.GetEnumeratedFiles(FilePath, BANK_EXTENSION);

        foreach (string file in files.Where(e => e.EndsWith(STRINGS_EXTENSION))) {
            LoadBank(assets, storage, file);
        }

        foreach (string file in files.Where(e => !e.EndsWith(STRINGS_EXTENSION))) {
            LoadBank(assets, storage, file);
        }

        assets.RegisterCustomAssetStorage<SoundEvent>(storage);
        assets.SetFallback<Bank>(new());
    }

    void LoadBank(Assets assets, AudioStorage storage, string bank) {
        FMODInstance.loadBankFile(bank, LOAD_BANK_FLAGS.NORMAL, out Bank bankData);

        bankData.getEventList(out EventDescription[] bankEvents);
        foreach (EventDescription bankEvent in bankEvents) {
            bankEvent.getPath(out string eventPath);
            storage.Events.Add(eventPath, bankEvent);
        }

        bankData.getBusList(out Bus[] bankBuses);
        foreach (Bus bankBus in bankBuses) {
            bankBus.getPath(out string busPath);
            assets.Add(busPath, bankBus);
        }
    }
}