using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;

namespace BetterPartySort;

public class PartyManager {
    public List<PartyMember> party;

    public LightParty[] LightParties;

    public PartyManager() {
        party = new List<PartyMember>();
        LightParties = [new LightParty(this), new LightParty(this)];
    }
    
    public List<PartyMember> GetPlayersOfJobType(JobType jobType) {
        return party.FindAll(existingMember => existingMember.JobType == jobType);
    }

    public unsafe void RegisterPartyMember(HudPartyMember hudPartyMember) {
        if (hudPartyMember.Object != null) {
            PartyMember newMember = new PartyMember(hudPartyMember);
            party.Add(newMember);
            if (!LightParties[0].players.Exists(existingMember => existingMember.JobType == newMember.JobType)) {
                Dalamud.Log(newMember.Name + " is registered for light party 1");
                LightParties[0].RegisterLightPartyMember(newMember);
            }
            else {
                Dalamud.Log(newMember.Name + " is registered for light party 2");
                LightParties[1].RegisterLightPartyMember(newMember);
            }
        }
    }

    public PartyMember GetPartyMemberByRole(PartyRole role) {
        int lightPartyNumber = int.Parse(Regex.Match(role.ToString(), @"\d+").Value);
        return LightParties[lightPartyNumber - 1].players.Find(existingMember => existingMember.JobType == SortUtility.GetJobTypeFromRole(role));
    }

    // public List<PartyMember> GetSubListOfType(JobType jobType) {
    //     switch (jobType) {
    //         case JobType.Tank:
    //             return tanks;
    //         case JobType.Healer:
    //             return healers;
    //         case JobType.Melee:
    //             return melee;
    //         case JobType.Ranged:
    //             return ranged;
    //     }
    //
    //     return null;
    // }



    // public bool IsPlayerInLightParty(PartyMember partyMember, int partyNumber) {
    //     if (partyNumber == 1) {
    //         return lightParty1.Contains(partyMember);
    //     }
    //
    //     if (partyNumber == 2) {
    //         return lightParty2.Contains(partyMember);
    //     }
    //
    //     return false;
    // }
    //
    // public int GetPlayerPartyNumber(PartyMember partyMember) {
    //     if (lightParty1.Contains(partyMember)) {
    //         return 1;
    //     }
    //
    //     if (lightParty2.Contains(partyMember)) {
    //         return 2;
    //     }
    //
    //     return 0;
    // }
    //
    // public void SwapPlayerParties(PartyMember party1Player, PartyMember party2Player) {
    //     lightParty1.Remove(party1Player);
    //     lightParty2.Add(party1Player);
    //
    //     lightParty2.Remove(party2Player);
    //     lightParty1.Add(party2Player);
    // }

    public class LightParty {
        private readonly PartyManager partyManager;
        public List<PartyMember> players;
        public Dictionary<JobType, int> PartyIndexDict;

        public LightParty(PartyManager partyManager) {
            this.partyManager = partyManager;
            players = new List<PartyMember>();
            PartyIndexDict = new Dictionary<JobType, int>();
        }

        public void RegisterLightPartyMember(PartyMember partyMember) {
            players.Add(partyMember);
            PartyIndexDict[partyMember.JobType] =
                partyManager.GetPlayersOfJobType(partyMember.JobType).IndexOf(partyMember);
        }

        public void UnregisterLightPartyMember(PartyMember partyMember) {
            players.Remove(partyMember);
        }

        // public PartyMember GetLightPartyMemberOfType(JobType jobType) {
        //     PartyMember partyMember = null;
        //     switch (jobType) {
        //         case JobType.Tank:
        //             partyMember = partyManager.tanks[PartyIndexDict[jobType]];
        //             break;
        //         case JobType.Healer:
        //             partyMember = partyManager.healers[PartyIndexDict[jobType]];
        //             break;
        //         case JobType.Melee:
        //             partyMember = partyManager.melee[PartyIndexDict[jobType]];
        //             break;
        //         case JobType.Ranged:
        //             partyMember = partyManager.ranged[PartyIndexDict[jobType]];
        //             break;
        //         default:
        //             break;
        //     }
        //
        //     return partyMember;
        // }
    }
}
