using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiNET;

namespace BetterPartySort.Windows;

public class SortOptionsTab {
    private readonly Plugin plugin;

    public SortOptionsTab(Plugin plugin) {
        this.plugin = plugin;
    }

    public unsafe void Draw() {
        var party = AgentHUD.Instance()->PartyMembers;
        List<string> tanks = new();
        foreach (var player in party) {
            if (player.Object != null) {
                if (SortUtility.JobTypeDict[player.Object->ClassJob] == SortUtility.JobType.Tank) {
                    tanks.Add(player.Object->NameString);
                }
            }
        }

        var value = 0;
        if (plugin.partyDict.ContainsKey(PartyRole.Tank1))
            value = (int)plugin.partyDict[PartyRole.Tank1];
        
        

        if (ImGui.Combo("Tank1", ref value, tanks.ToArray(), tanks.Count)) {
            plugin.partyDict[PartyRole.Tank1] = value;
        }

        if (ImGui.Combo("Tank2", ref value, tanks.ToArray(), tanks.Count)) { }
    }
}
