using System.Collections.Generic;
using ImGuiNET;

namespace BetterPartySort.Windows;

public class SortOptionsTab {
    private readonly Plugin plugin;
    private Dictionary<int, string> roleDictionary;

    public SortOptionsTab(Plugin plugin) {
        this.plugin = plugin;
        roleDictionary = [this.plugin.Configuration.Tank1Index, "Tank 1"];
    }

    
    public void Draw() {
        plugin.SortManager.PopulatePartyMembers();
        var partyMembers = plugin.SortManager.PartyMembers;

        var tanks = partyMembers.FindAll(partyMember => partyMember.JobType == SortUtility.JobType.Tank);
        var healers = partyMembers.FindAll(partyMember => partyMember.JobType == SortUtility.JobType.Healer);
        var melee = partyMembers.FindAll(partyMember => partyMember.JobType == SortUtility.JobType.Melee);
        var ranged = partyMembers.FindAll(partyMember => partyMember.JobType == SortUtility.JobType.Ranged);

        var value = plugin.Configuration.Tank1Index;
        if (ImGui.Combo("Tank 1", ref value, tanks.ConvertAll(partyMember => partyMember.Name).ToArray(),
                        tanks.Count)) {
            plugin.Configuration.Tank1Index = value;
            plugin.Configuration.Tank2Index = value == 0 ? 1 : 0;
            plugin.Configuration.Save();

            var index = plugin.SortManager.PartyMembers.IndexOf(tanks[value]);
            Dalamud.Log();
            plugin.SortManager.PartyMembers[index].Role = PartyRole.Tank1;
        }

        value = plugin.Configuration.Tank2Index;
        if (ImGui.Combo("Tank 2", ref value, tanks.ConvertAll(partyMember => partyMember.Name).ToArray(),
                        tanks.Count)) {
            plugin.Configuration.Tank2Index = value;
            plugin.Configuration.Tank1Index = value == 0 ? 1 : 0;
            plugin.Configuration.Save();

            var index = plugin.SortManager.PartyMembers.IndexOf(tanks[value]);
            Dalamud.Log(index.ToString());
            plugin.SortManager.PartyMembers[index].Role = PartyRole.Tank2;
        }

        value = plugin.Configuration.Healer1Index;
        if (ImGui.Combo("Healer 1", ref value, healers.ConvertAll(partyMember => partyMember.Name).ToArray(),
                        healers.Count)) {
            plugin.Configuration.Healer1Index = value;
            plugin.Configuration.Healer2Index = value == 0 ? 1 : 0;
            plugin.Configuration.Save();

            healers[value].Role = PartyRole.Healer1;
        }

        value = plugin.Configuration.Healer2Index;

        if (ImGui.Combo("Healer 2", ref value, healers.ConvertAll(partyMember => partyMember.Name).ToArray(),
                        healers.Count)) {
            plugin.Configuration.Healer2Index = value;
            plugin.Configuration.Healer1Index = value == 0 ? 1 : 0;
            plugin.Configuration.Save();

            healers[value].Role = PartyRole.Healer2;
        }

        value = plugin.Configuration.Melee1Index;

        if (ImGui.Combo("Melee 1", ref value, melee.ConvertAll(partyMember => partyMember.Name).ToArray(),
                        melee.Count)) {
            plugin.Configuration.Melee1Index = value;
            plugin.Configuration.Melee2Index = value == 0 ? 1 : 0;
            plugin.Configuration.Save();

            melee[value].Role = PartyRole.Melee1;
        }

        value = plugin.Configuration.Melee2Index;

        if (ImGui.Combo("Melee 2", ref value, melee.ConvertAll(partyMember => partyMember.Name).ToArray(),
                        melee.Count)) {
            plugin.Configuration.Melee2Index = value;
            plugin.Configuration.Melee1Index = value == 0 ? 1 : 0;
            plugin.Configuration.Save();

            melee[value].Role = PartyRole.Melee2;
        }

        value = plugin.Configuration.Ranged1Index;

        if (ImGui.Combo("Ranged 1", ref value, ranged.ConvertAll(partyMember => partyMember.Name).ToArray(),
                        ranged.Count)) {
            plugin.Configuration.Ranged1Index = value;
            plugin.Configuration.Ranged2Index = value == 0 ? 1 : 0;
            plugin.Configuration.Save();

            ranged[value].Role = PartyRole.Ranged1;
        }

        value = plugin.Configuration.Ranged2Index;

        if (ImGui.Combo("Ranged 2", ref value, ranged.ConvertAll(partyMember => partyMember.Name).ToArray(),
                        ranged.Count)) {
            plugin.Configuration.Ranged2Index = value;
            plugin.Configuration.Ranged1Index = value == 0 ? 1 : 0;
            plugin.Configuration.Save();

            ranged[value].Role = PartyRole.Ranged2;
        }
    }
}
