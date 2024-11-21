using System;
using BetterPartySort.Utils;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace BetterPartySort.Party;

public class PartyMember : IEquatable<PartyMember> {
    public uint EntityId { get; set; }
    public string Name { get; set; }
    public JobType JobType { get; set; }

    public unsafe PartyMember(HudPartyMember playerStruct) {
        EntityId = playerStruct.EntityId;
        Name = playerStruct.Object->NameString;
        JobType = SortUtility.JobTypeDict[playerStruct.Object->ClassJob];
    }

    public bool Equals(PartyMember? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return EntityId == other.EntityId && Name == other.Name && JobType == other.JobType;
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((PartyMember)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine(EntityId, Name, (int)JobType);
    }
}
