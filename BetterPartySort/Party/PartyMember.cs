using BetterPartySort.Utils;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace BetterPartySort.Party;

public class PartyMember {
    public uint EntityId { get; set; }
    public string Name { get; set; }
    public JobType JobType { get; set; }
    
    public unsafe PartyMember(HudPartyMember playerStruct) {
        EntityId = playerStruct.EntityId;
        Name = playerStruct.Object->NameString;
        JobType = SortUtility.JobTypeDict[playerStruct.Object->ClassJob];
    }
    
}
