using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace BetterPartySort;

public class PartyMember {
    public uint EntityId { get; set; }
    public string Name { get; set; }
    public SortUtility.JobType JobType { get; set; }
    public PartyRole Role { get; set; }

    public unsafe PartyMember(HudPartyMember playerStruct) {
        EntityId = playerStruct.EntityId;
        Name = playerStruct.Object->NameString;
        JobType = SortUtility.JobTypeDict[playerStruct.Object->ClassJob];
    }
}
