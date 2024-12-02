using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI.Info;

namespace BetterPartySort;

public class SortFunctions {
    private unsafe delegate void SwapDelegate(InfoProxyPartyMember *proxyPartyMember, int index1, int index2, bool doUpdate);

    private unsafe delegate void SortDelegate(InfoProxyPartyMember* proxyPartyMember);

    [Signature("E8 ?? ?? ?? ?? B3 01 88 5C 24 60")]
    private SwapDelegate? _swapOrder = null;
    
    [Signature("E8 ?? ?? ?? ?? 41 80 A5 ?? ?? ?? ?? ?? B2 01")]
    private SortDelegate? _fullSort = null;

    public SortFunctions() {
        Dalamud.GameInteropProvider.InitializeFromAttributes(this);
    }

    public unsafe void SwapPartyMembers(InfoProxyPartyMember *proxyPartyMember, int index1, int index2, bool doUpdate) {
        if (_swapOrder == null) {
            Dalamud.Log("ChangeOrder signature not found!");
        }

        _swapOrder(proxyPartyMember, index1, index2, doUpdate);
    }
    
    public unsafe void SortParty(InfoProxyPartyMember *proxyPartyMember) {
        if (_swapOrder == null) {
            Dalamud.Log("SortParty signature not found!");
        }

        _fullSort(proxyPartyMember);
    }
}
