using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace BetterPartySort;

public static class SortUtility {
    public enum JobType {
        Tank,
        Healer,
        Melee,
        Ranged
    }

    //kill me
    public static readonly Dictionary<byte, JobType> JobTypeDict = new() {
        { 1, JobType.Tank },
        { 2, JobType.Melee },
        { 3, JobType.Tank },
        { 4, JobType.Melee },
        { 5, JobType.Ranged },
        { 6, JobType.Healer },
        { 7, JobType.Ranged },
        { 19, JobType.Tank },
        { 20, JobType.Melee },
        { 21, JobType.Tank },
        { 22, JobType.Melee },
        { 23, JobType.Ranged },
        { 24, JobType.Healer },
        { 25, JobType.Ranged },
        { 26, JobType.Ranged },
        { 27, JobType.Ranged },
        { 28, JobType.Healer },
        { 29, JobType.Melee },
        { 30, JobType.Melee },
        { 31, JobType.Ranged },
        { 32, JobType.Tank },
        { 33, JobType.Healer },
        { 34, JobType.Melee },
        { 35, JobType.Ranged },
        { 36, JobType.Ranged },
        { 37, JobType.Tank },
        { 38, JobType.Ranged },
        { 39, JobType.Melee },
        { 40, JobType.Healer },
        { 41, JobType.Melee },
        { 42, JobType.Ranged }
    };

    public static unsafe byte GetIndexByEntityId(uint entityId) {
        var partyList = AgentHUD.Instance()->PartyMembers;
        foreach (var partyMember in partyList) {
            if (partyMember.EntityId == entityId) {
                return partyMember.Index;
            }
        }

        return 0;
    }
    
    public static unsafe void UpdatePartyMemberIndex(byte originalIndex, byte newIndex) {
        var partyList = AgentHUD.Instance()->PartyMembers;
        for (int i = 0; i < partyList.Length; i++) {
            if (partyList[i].Index == originalIndex) {
                partyList[i].Index = newIndex;
            }
        }
    }
}
