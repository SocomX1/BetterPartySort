using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;

namespace BetterPartySort;

public class SortManager {
    private Plugin plugin;
    private Configuration configuration;

    public List<PartyMember> PartyMembers;

    public SortManager(Plugin plugin) {
        this.plugin = plugin;
        configuration = this.plugin.Configuration;
        PartyMembers = new List<PartyMember>();
    }

    public unsafe void PopulatePartyMembers() {
        //PartyMembers.Clear();
        var partyList = new List<PartyMember>();

        var partyHud = AgentHUD.Instance()->PartyMembers;
        foreach (var player in partyHud) {
            if (player.Object != null) {
                partyList.Add(new PartyMember(player));
            }
        }

        if (!Equals(PartyMembers, partyList)) {
            PartyMembers = partyList;
        }
    }

    public void AssignRoles() {
        
    }

    public unsafe void SortParty(int sortConfigIndex = 0) {
        PartyConfiguration partyConfig = configuration.PartyConfigurations[sortConfigIndex];

        foreach (var VARIABLE in PartyMembers) {
            Dalamud.Log(VARIABLE.Name + ", " + VARIABLE.JobType + ", " + VARIABLE.Role);
        }

        for (int i = 0; i < 8; i++) {
            var role = partyConfig.GetRoleByIndex(i);
            var id = PartyMembers.Find(player => player.Role == role).EntityId;
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
