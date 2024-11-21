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

    public void InitParty() {
        party.Clear();

        foreach (var lightParty in LightParties) {
            lightParty.InitLightParty();
        }

        PopulatePartyMembers();
    }

    public unsafe bool ValidatePartyComposition() {
        if (AgentHUD.Instance()->PartyMemberCount != 8)
            return false;

        if (GetHudPlayersOfJobTypeCount(JobType.Tank) != 2)
            return false;

        if (GetHudPlayersOfJobTypeCount(JobType.Healer) != 2)
            return false;

        if (GetHudPlayersOfJobTypeCount(JobType.Melee) < 1)
            return false;

        if (GetHudPlayersOfJobTypeCount(JobType.Ranged) < 1)
            return false;

        return true;
    }

    public unsafe void PopulatePartyMembers() {
        var partyHud = AgentHUD.Instance()->PartyMembers;

        foreach (var player in partyHud) {
            if (player.Object != null) {
                var newMember = new PartyMember(player);

                if (!party.Contains(newMember)) {
                    RegisterPartyMember(newMember);
                }
            }
        }
    }

    public void RegisterPartyMember(PartyMember partyMember) {
        party.Add(partyMember);
        if (!LightParties[0].Players.Exists(existingMember => existingMember.JobType == partyMember.JobType)) {
            LightParties[0].RegisterLightPartyMember(partyMember);
        }
        else {
            LightParties[1].RegisterLightPartyMember(partyMember);
        }
    }

    public List<PartyMember> GetPlayersOfJobType(JobType jobType) {
        return party.FindAll(existingMember => existingMember.JobType == jobType);
    }

    private unsafe int GetHudPlayersOfJobTypeCount(JobType jobType) {
        List<HudPartyMember> hudPartyMembers = new List<HudPartyMember>(AgentHUD.Instance()->PartyMembers.ToArray());
        return hudPartyMembers.FindAll(existingMember => existingMember.Object != null &&
                                                         SortUtility.JobTypeDict[existingMember.Object->ClassJob] ==
                                                         jobType).Count;
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

        public void InitLightParty() {
            Players.Clear();
            PartyIndexDict.Clear();
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
