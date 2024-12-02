using System;
using System.Collections.Generic;
using BetterPartySort.Config;
using Dalamud.Game.Config;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;

namespace BetterPartySort;

public class SortManager {
    private Plugin plugin;
    private Configuration configuration;
    private SortFunctions sortFunctions;

    private readonly Dictionary<uint, JobRole[]> roleOrderDict = new() {
        { 0, [JobRole.Tank, JobRole.Healer, JobRole.DPS] },
        { 1, [JobRole.Tank, JobRole.DPS, JobRole.Healer] },
        { 2, [JobRole.Healer, JobRole.Tank, JobRole.DPS] },
        { 3, [JobRole.Healer, JobRole.DPS, JobRole.Tank] },
        { 4, [JobRole.DPS, JobRole.Tank, JobRole.Healer] },
        { 5, [JobRole.DPS, JobRole.Healer, JobRole.Tank] }
    };

    //this game absolutely sucks
    private readonly Dictionary<uint, uint> jobClassDict = new() {
        { 19, 1 },  //PLD -> GLA
        { 20, 2 },  //MNK -> PGL
        { 21, 3 },  //WAR -> MRD
        { 22, 4 },  //DRG -> LNC
        { 23, 5 },  //BRC -> ARC
        { 24, 6 },  //WHM -> CNJ
        { 25, 7 },  //BLM -> THM
        { 27, 26 }, //SMN -> ACN
        { 30, 29 }  //NIN -> ROG
    };

    private List<uint> jobOrder;

    public SortManager(Plugin plugin) {
        this.plugin = plugin;
        configuration = this.plugin.Configuration;
        sortFunctions = new SortFunctions();
    }

    private unsafe void PopulateJobOrderList() {
        var module = (nint)UIModule.Instance() + 0xA0820;

        var dpsOrder = new Span<ushort>((void*)(module + 0x88), 16).ToArray();
        var tankOrder = new Span<ushort>((void*)(module + 0x48), 16).ToArray();
        var healerOrder = new Span<ushort>((void*)(module + 0x68), 16).ToArray();

        Dictionary<JobRole, ushort[]> jobOrderDict = new Dictionary<JobRole, ushort[]> {
            { JobRole.DPS, dpsOrder },
            { JobRole.Tank, tankOrder },
            { JobRole.Healer, healerOrder }
        };

        var playerRoleNumber = Dalamud.ClientState.LocalPlayer.ClassJob.Value.Role;

        var roleOption = playerRoleNumber switch {
            1 => UiConfigOption.PartyListSortTypeTank,
            2 or 3 or 5 => UiConfigOption.PartyListSortTypeDps,
            4 => UiConfigOption.PartyListSortTypeHealer,
            _ => UiConfigOption.PartyListSortTypeDps
        };

        var roleOrder = Dalamud.GameConfig.UiConfig.GetUInt(roleOption.GetAttribute<GameConfigOptionAttribute>().Name);
        var partyArrangement = roleOrderDict[roleOrder];

        jobOrder = [];

        for (var i = 0; i < partyArrangement.Length; i++) {
            var jobs = jobOrderDict[partyArrangement[i]];
            foreach (var jobID in jobs) {
                if (jobID != 0 && IsJobIDInParty(jobID)) {
                    jobOrder.Add(jobID);
                }
            }
        }
    }

    public unsafe void SortParty() {
        sortFunctions.SortParty(InfoProxyPartyMember.Instance());
        Dalamud.Framework.Update += OnFrameworkTick;
    }

    private unsafe void OnFrameworkTick(IFramework framework) {
        PopulateJobOrderList();

        var party = AgentHUD.Instance()->PartyMembers;
        var playerIndex = party[0].Index;
        var playerRoleNumber = ConvertJobToClass(Dalamud.ClientState.LocalPlayer.ClassJob.Value.RowId);

        for (int i = 1; i < party.Length; i++) {
            if (party[i].Object != null) {
                if (jobOrder.IndexOf(playerRoleNumber) > jobOrder.IndexOf(GetJobIDByPartyIndex(i))) {
                    sortFunctions.SwapPartyMembers(InfoProxyPartyMember.Instance(), playerIndex, playerIndex + 1, true);
                    playerIndex++;
                }
            }
        }

        Dalamud.Framework.Update -= OnFrameworkTick;
    }

    private unsafe bool IsJobIDInParty(ushort jobID) {
        foreach (var player in AgentHUD.Instance()->PartyMembers) {
            if (player.Object != null) {
                if (player.Object->ClassJob == jobID) {
                    return true;
                }
            }
        }

        return false;
    }

    private unsafe uint GetJobIDByPartyIndex(int partyIndex) {
        uint jobID = 0;

        foreach (var player in AgentHUD.Instance()->PartyMembers) {
            if (player.Object != null && player.Index == partyIndex) {
                jobID = player.Object->ClassJob;
                if (jobClassDict.TryGetValue(jobID, out var value)) {
                    jobID = value;
                }
            }
        }

        return jobID;
    }

    /*
     * Jobs with classes are represented by class ID in the client's role sort settings,
     * but by job ID when reading the ClassJob of a HudPartyMember.
     */
    private uint ConvertJobToClass(uint jobID) {
        return jobClassDict.GetValueOrDefault(jobID, jobID);
    }

    private enum JobRole {
        DPS,
        Tank,
        Healer
    }
}
