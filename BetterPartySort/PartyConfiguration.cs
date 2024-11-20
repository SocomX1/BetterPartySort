namespace BetterPartySort;

public class PartyConfiguration {
    private string configurationName;
    private string configurationDescription;
    private readonly PartyRole[] sortOrder;

    public PartyConfiguration(string configurationName, string configurationDescription, PartyRole[] sortOrder) {
        this.configurationName = configurationName;
        this.configurationDescription = configurationDescription;
        this.sortOrder = sortOrder;
    }

    public PartyRole GetRoleByIndex(int index) {
        return sortOrder[index];
    }
}
