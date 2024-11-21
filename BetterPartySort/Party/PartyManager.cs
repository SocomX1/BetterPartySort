using System.Collections.Generic;
using System.Text.RegularExpressions;
using BetterPartySort.Utils;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace BetterPartySort.Party;

public class PartyManager {
    private List<PartyMember> party;
    public LightParty[] LightParties;

    public PartyManager() {
        party = new List<PartyMember>();
        LightParties = [new LightParty(this), new LightParty(this)];
    }

    public unsafe void PopulatePartyMembers() {
        var partyHud = AgentHUD.Instance()->PartyMembers;
        foreach (var player in partyHud) {
            if (player.Object != null) {
                RegisterPartyMember(player);
            }
        }
    }

    public unsafe void RegisterPartyMember(HudPartyMember hudPartyMember) {
        if (hudPartyMember.Object != null) {
            PartyMember newMember = new PartyMember(hudPartyMember);
            party.Add(newMember);
            if (!LightParties[0].Players.Exists(existingMember => existingMember.JobType == newMember.JobType)) {
                Dalamud.Log(newMember.Name + " is registered for light party 1");
                LightParties[0].RegisterLightPartyMember(newMember);
            }
            else {
                Dalamud.Log(newMember.Name + " is registered for light party 2");
                LightParties[1].RegisterLightPartyMember(newMember);
            }
        }
    }

    public List<PartyMember> GetPlayersOfJobType(JobType jobType) {
        return party.FindAll(existingMember => existingMember.JobType == jobType);
    }

    public int GetRegisteredPlayerCount() {
        return party.Count;
    }

    public PartyMember GetPartyMemberByRole(PartyRole role) {
        int lightPartyIndex = int.Parse(Regex.Match(role.ToString(), @"\d+").Value) - 1;
        var lightPartyMembers = LightParties[lightPartyIndex].Players;
        return lightPartyMembers.Find(existingMember => existingMember.JobType == SortUtility.GetJobTypeFromRole(role));
    }

    public class LightParty {
        private readonly PartyManager partyManager;
        public List<PartyMember> Players;
        public Dictionary<JobType, int> PartyIndexDict;

        public LightParty(PartyManager partyManager) {
            this.partyManager = partyManager;
            Players = new List<PartyMember>();
            PartyIndexDict = new Dictionary<JobType, int>();
        }

        public void RegisterLightPartyMember(PartyMember partyMember) {
            Players.Add(partyMember);
            PartyIndexDict[partyMember.JobType] =
                partyManager.GetPlayersOfJobType(partyMember.JobType).IndexOf(partyMember);
        }

        public void UnregisterLightPartyMember(PartyMember partyMember) {
            Players.Remove(partyMember);
        }
    }
}
