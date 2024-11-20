namespace SamplePlugin;

public class PartyConfiguration
{
    private string configurationName;
    private string configurationDescription;
    public PartyRole[] sortOrder;

    public PartyConfiguration(string configurationName, string configurationDescription, PartyRole[] sortOrder)
    {
        this.configurationName = configurationName;
        this.configurationDescription = configurationDescription;
        this.sortOrder = sortOrder;
    }
}
