namespace BetterPartySort.Party;

public enum PartyRole {
    Tank1,
    Tank2,
    Healer1,
    Healer2,
    Melee1,
    Melee2,
    Ranged1,
    Ranged2
}

public enum JobType {
    Tank,
    Healer,
    Melee,
    Ranged
}

public struct RoleStruct(PartyRole role, JobType jobType, string name) {
    public PartyRole Role = role;
    public JobType JobType = jobType;
    public string Name = name;
}


