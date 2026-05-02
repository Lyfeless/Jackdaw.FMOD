using FMOD.Studio;

namespace Jackdaw.Audio.FMODAudio;

public class AudioLoader(FMOD.Studio.System instance, string group) : AssetLoaderStage {
    record struct BankInfo(EventDescription[] Events, Bus[] Banks);

    const string BANK_EXTENSION = ".bank";
    const string STRINGS_EXTENSION = "strings";

    readonly string Group = group;
    readonly FMOD.Studio.System FMODInstance = instance;

    readonly Dictionary<AssetProviderItem, BankInfo> CachedBankInfo = [];
    readonly AudioStorage Storage = new(instance);

    bool startupCached = false;


    public override AssetProviderItem[] GetLoadOptions(Assets assets) {
        AssetProviderItem[] banks = assets.Provider.GetItemsInGroup(Group, BANK_EXTENSION);
        CacheStartup(assets, banks);
        return [.. banks.Where(e => !e.Name.EndsWith(STRINGS_EXTENSION))];
    }

    public override void RunLoad(Assets assets, AssetCollection collection) {
        foreach (AssetProviderItem item in Filter(collection)) {
            LoadBank(assets, item);
        }
    }

    public override void RunUnload(Assets assets, AssetCollection collection) {
        foreach (AssetProviderItem item in Filter(collection)) {
            UnloadBank(assets, item);
        }
    }

    void LoadBank(Assets assets, AssetProviderItem bank) {
        using Stream stream = assets.Provider.GetItemStream(bank);
        byte[] bytes = new byte[stream.Length];
        stream.ReadExactly(bytes);
        FMODInstance.loadBankMemory(bytes, LOAD_BANK_FLAGS.NORMAL, out Bank bankData);

        if (!CachedBankInfo.TryGetValue(bank, out BankInfo bankInfo)) {
            bankData.getEventList(out EventDescription[] bankEvents);
            bankData.getBusList(out Bus[] bankBuses);
            bankInfo = new(bankEvents, bankBuses);
            CachedBankInfo.Add(bank, bankInfo);
        }

        foreach (EventDescription bankEvent in bankInfo.Events) {
            bankEvent.getPath(out string eventPath);
            Storage.Events.Add(eventPath, bankEvent);
        }

        foreach (Bus bankBus in bankInfo.Banks) {
            bankBus.getPath(out string busPath);
            AddAsset(assets, busPath, bankBus);
        }
    }

    void UnloadBank(Assets assets, AssetProviderItem bank) {
        if (!CachedBankInfo.TryGetValue(bank, out BankInfo value)) { return; }

        foreach (EventDescription bankEvent in value.Events) {
            bankEvent.getPath(out string eventPath);
            Storage.Events.Remove(eventPath);
        }

        foreach (Bus bankBus in value.Banks) {
            bankBus.getPath(out string busPath);
            RemoveAsset<Bus>(assets, busPath);
        }
    }

    void CacheStartup(Assets assets, AssetProviderItem[] banks) {
        if (startupCached) { return; }

        startupCached = true;

        assets.Storage.Register<SoundEvent>(Storage);
        assets.SetFallback<Bank>(new());

        foreach (AssetProviderItem file in banks.Where(e => e.Name.EndsWith(STRINGS_EXTENSION))) {
            LoadBank(assets, file);
        }
    }

    AssetProviderItem[] Filter(AssetCollection collection)
        => collection.Filter(Group, BANK_EXTENSION);
}