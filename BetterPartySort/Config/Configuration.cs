using System;
using Dalamud.Configuration;

namespace BetterPartySort.Config;

[Serializable]
public class Configuration : IPluginConfiguration {
    public int Version { get; set; } = 0;
    public bool SortOnDutyCommence { get; set; } = true;

    // the below exist just to make saving less cumbersome
    public void Save() {
        Dalamud.PluginInterface.SavePluginConfig(this);
    }
}
