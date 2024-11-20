using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BetterPartySort;

public class PartyConfiguration {
    public string ConfigurationName { get; set; }
    public string ConfigurationDescription { get; set; }

    [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
    private readonly PartyRole[] sortOrder;

    public PartyConfiguration(string configurationName, string configurationDescription, PartyRole[] sortOrder) {
        this.ConfigurationName = configurationName;
        this.ConfigurationDescription = configurationDescription;
        this.sortOrder = sortOrder;
    }

    public PartyRole GetRoleByIndex(int index) {
        return sortOrder[index];
    }
}
