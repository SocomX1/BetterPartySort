using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Common.Lua;
using ImGuiNET;

namespace BetterPartySort.Windows;

public class SortOptionsTab {
    private readonly Plugin plugin;
    private Configuration config;

    public SortOptionsTab(Plugin plugin) {
        this.plugin = plugin;
        config = plugin.Configuration;
    }

    public unsafe void Draw() {
        if (plugin.PartyManager.party.Count != 8) {
            plugin.SortManager.PopulatePartyMembers();
        }

        for (int i = 0; i < 4; i++) {
            var lightParty = plugin.PartyManager.LightParties[0];

            var role = SortManager.roles[i];
            var playersOfJob = plugin.PartyManager.GetPlayersOfJobType(role.type);
            var playerNames = playersOfJob.ConvertAll(player => player.Name);

            var value = lightParty.PartyIndexDict[role.type];

            if (ImGui.Combo(role.name, ref value, playerNames.ToArray(), playerNames.Count)) {
                var previousValue = lightParty.PartyIndexDict[role.type];
                lightParty.PartyIndexDict[role.type] = value;
                if (previousValue != value) {
                    lightParty.UnregisterLightPartyMember(
                        playersOfJob.Find(player => player.Name == playerNames[previousValue]));
                    lightParty.RegisterLightPartyMember(playersOfJob.Find(player => player.Name == playerNames[value]));
                }
            }
        }

        ImGui.Separator();

        for (int i = 4; i < 8; i++) {
            var lightParty = plugin.PartyManager.LightParties[1];
            var role = SortManager.roles[i];
            var playersOfJob = plugin.PartyManager.GetPlayersOfJobType(role.type);
            var playerNames = playersOfJob.ConvertAll(player => player.Name);

            var value = lightParty.PartyIndexDict[role.type];

            if (ImGui.Combo(role.name, ref value, playerNames.ToArray(), playerNames.Count)) {
                var previousValue = lightParty.PartyIndexDict[role.type];
                lightParty.PartyIndexDict[role.type] = value;
                if (previousValue != value) {
                    lightParty.UnregisterLightPartyMember(
                        playersOfJob.Find(player => player.Name == playerNames[previousValue]));
                    lightParty.RegisterLightPartyMember(playersOfJob.Find(player => player.Name == playerNames[value]));
                }
            }
        }
    }
}
