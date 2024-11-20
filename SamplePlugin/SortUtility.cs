using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace SamplePlugin;

public static class SortUtility
{
    public static unsafe byte GetIndexByEntityId(uint entityId)
    {
        var partyList = AgentHUD.Instance()->PartyMembers;
        foreach (var partyMember in partyList)
        {
            if (partyMember.EntityId == entityId)
            {
                return partyMember.Index;
            }
        }

        return 0;
    }

    public static unsafe void UpdatePartyMemberIndex(byte originalIndex, int newIndex)
    {
        var partyList = AgentHUD.Instance()->PartyMembers;
        for (int i = 0; i < partyList.Length; i++)
        {
            if (partyList[i].Index == originalIndex)
            {
                partyList[i].Index = (byte)newIndex;
            }
        }
    }
}
