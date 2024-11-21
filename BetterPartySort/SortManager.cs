using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;

namespace BetterPartySort;

public class SortManager {
    private Plugin plugin;
    private Configuration configuration;

    public static List<RoleStruct> roles = [
        new RoleStruct(PartyRole.Tank1, JobType.Tank, "Tank 1"),
        new RoleStruct(PartyRole.Healer1, JobType.Healer, "Healer 1"),
        new RoleStruct(PartyRole.Melee1, JobType.Melee, "Melee 1"),
        new RoleStruct(PartyRole.Ranged1, JobType.Ranged, "Ranged 1"),
        new RoleStruct(PartyRole.Tank2, JobType.Tank, "Tank 2"),
        new RoleStruct(PartyRole.Healer2, JobType.Healer, "Healer 2"),
        new RoleStruct(PartyRole.Melee2, JobType.Melee, "Melee 2"),
        new RoleStruct(PartyRole.Ranged2, JobType.Ranged, "Ranged 2")
    ];

    public SortManager(Plugin plugin) {
        this.plugin = plugin;
        configuration = this.plugin.Configuration;
    }

    public unsafe void PopulatePartyMembers() {
        var partyHud = AgentHUD.Instance()->PartyMembers;
        foreach (var player in partyHud) {
            if (player.Object != null) {
                plugin.PartyManager.RegisterPartyMember(player);
            }
        }
    }

    public unsafe void SortParty(int sortConfigIndex = 0) {
        PartyConfiguration partyConfig = configuration.PartyConfigurations[sortConfigIndex];

        // foreach (var VARIABLE in PartyMembers) {
        //     Dalamud.Log(VARIABLE.Name + ", " + VARIABLE.JobType + ", " + VARIABLE.Role);
        // }

        for (int i = 0; i < 8; i++) {
            var role = partyConfig.GetRoleByIndex(i);
            var id = plugin.PartyManager.GetPartyMemberByRole(role).EntityId;
            var targetIndex = SortUtility.GetIndexByEntityId(id);
            string sortParams = "/psort " + (i + 1) + " " + (targetIndex + 1);

            if (i != targetIndex) {
                Dalamud.Log(sortParams + ": " + role + ", " + id + ", " + targetIndex);
                SortUtility.UpdatePartyMemberIndex((byte)i, targetIndex);

                RaptureShellModule.Instance()->ShellCommandModule.ExecuteCommandInner(
                    Utf8String.FromString(sortParams), RaptureShellModule.Instance()->UIModule);
            }
        }
    }
}
