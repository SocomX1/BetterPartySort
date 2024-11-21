using System.Collections.Generic;
using BetterPartySort.Party;
using ImGuiNET;

namespace BetterPartySort.Windows;

public class SortOptionsTab {
    private readonly Plugin plugin;

    private static readonly List<RoleStruct> Roles = [
        new(PartyRole.Tank1, JobType.Tank, "Tank 1"),
        new(PartyRole.Healer1, JobType.Healer, "Healer 1"),
        new(PartyRole.Melee1, JobType.Melee, "Melee 1"),
        new(PartyRole.Ranged1, JobType.Ranged, "Ranged 1"),
        new(PartyRole.Tank2, JobType.Tank, "Tank 2"),
        new(PartyRole.Healer2, JobType.Healer, "Healer 2"),
        new(PartyRole.Melee2, JobType.Melee, "Melee 2"),
        new(PartyRole.Ranged2, JobType.Ranged, "Ranged 2")
    ];

    public SortOptionsTab(Plugin plugin) {
        this.plugin = plugin;
        this.plugin.PartyManager.PopulatePartyMembers();
    }

    public void Draw() {
        for (int i = 0; i < 8; i++) {
            var lightParty = i <= 3 ? plugin.PartyManager.LightParties[0] : plugin.PartyManager.LightParties[1];

            var role = Roles[i];
            var playersOfJob = plugin.PartyManager.GetPlayersOfJobType(role.JobType);
            var playerNames = playersOfJob.ConvertAll(player => player.Name);

            var currentIndex = -1;
            if (lightParty.PartyIndexDict.TryGetValue(role.JobType, out int value)) {
                currentIndex = value;
            }

            if (ImGui.Combo(role.Name, ref currentIndex, playerNames.ToArray(), playerNames.Count)) {
                if (plugin.PartyManager.ValidatePartyComposition()) {
                    var previousIndex = lightParty.PartyIndexDict[role.JobType];
                    lightParty.PartyIndexDict[role.JobType] = currentIndex;

                    if (previousIndex != currentIndex) {
                        lightParty.UnregisterLightPartyMember(
                            playersOfJob.Find(player => player.Name == playerNames[previousIndex]));
                        lightParty.RegisterLightPartyMember(
                            playersOfJob.Find(player => player.Name == playerNames[currentIndex]));
                    }
                }
            }

            if (i == 3) {
                ImGui.Separator();
            }
        }
    }
}
