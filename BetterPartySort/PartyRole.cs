using System;
using System.Collections.Generic;

namespace BetterPartySort;

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

public struct RoleStruct(PartyRole role, JobType type, string name) {
    public PartyRole role = role;
    public JobType type = type;
    public string name = name;
}


