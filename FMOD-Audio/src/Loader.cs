using FMOD.Studio;

namespace Jackdaw.Audio.FMODAudio;

public class AudioLoader(FMOD.Studio.System instance, string group) : AssetLoaderStage {
    const string BANK_EXTENSION = ".bank";
    const string STRINGS_EXTENSION = "strings";

    readonly string Group = group;
    readonly FMOD.Studio.System FMODInstance = instance;

    public override void Run(Assets assets) {
        AudioStorage storage = new(FMODInstance);

        AssetProviderItem[] banks = assets.Provider.GetItemsInGroup(Group, BANK_EXTENSION);

        foreach (AssetProviderItem file in banks.Where(e => e.Name.EndsWith(STRINGS_EXTENSION))) {
            LoadBank(assets, storage, file);
        }

        foreach (AssetProviderItem file in banks.Where(e => !e.Name.EndsWith(STRINGS_EXTENSION))) {
            LoadBank(assets, storage, file);
        }

        assets.RegisterCustomAssetStorage<SoundEvent>(storage);
        assets.SetFallback<Bank>(new());
    }

    void LoadBank(Assets assets, AudioStorage storage, AssetProviderItem bank) {
        using Stream stream = assets.Provider.GetItemStream(bank);
        byte[] bytes = new byte[stream.Length];
        stream.ReadExactly(bytes);
        FMODInstance.loadBankMemory(bytes, LOAD_BANK_FLAGS.NORMAL, out Bank bankData);

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