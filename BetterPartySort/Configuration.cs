using System;
using System.Collections.Generic;
using Dalamud.Configuration;

namespace BetterPartySort;

[Serializable]
public class Configuration : IPluginConfiguration {
    public int Version { get; set; } = 0;

    public bool IsConfigWindowMovable { get; set; } = true;
    public bool SomePropertyToBeSavedAndWithADefault { get; set; } = true;

    public PartyConfiguration[] PartyConfigurations { get; set; } = [
        new("Light parties", "THMRTHMR", [
            PartyRole.Tank1, PartyRole.Healer1, PartyRole.Melee1, PartyRole.Ranged1,
            PartyRole.Tank2, PartyRole.Healer2, PartyRole.Melee2, PartyRole.Ranged2
        ]),
        
        new("Conga", "TTHHMMRR", [
            PartyRole.Tank1, PartyRole.Tank2, PartyRole.Healer1, PartyRole.Healer2,
            PartyRole.Melee1, PartyRole.Melee2, PartyRole.Ranged1, PartyRole.Ranged2
        ])
    ];

    // the below exist just to make saving less cumbersome
    public void Save() {
        Dalamud.PluginInterface.SavePluginConfig(this);
    }
}
