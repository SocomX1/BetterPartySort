using BetterPartySort.Config;
using BetterPartySort.Utils;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;

namespace BetterPartySort;

public class SortManager {
    private Plugin plugin;
    private Configuration configuration;

    public SortManager(Plugin plugin) {
        this.plugin = plugin;
        configuration = this.plugin.Configuration;
    }

    public unsafe void SortParty(int sortConfigIndex = 0) {
        PartyConfiguration partyConfig = configuration.PartyConfigurations[sortConfigIndex];

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
