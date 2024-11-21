using BetterPartySort.Party;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BetterPartySort.Config;

public class PartyConfiguration(string configurationName, string configurationDescription, PartyRole[] sortOrder) {
    public string ConfigurationName { get; set; } = configurationName;
    public string ConfigurationDescription { get; set; } = configurationDescription;

    [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
    private readonly PartyRole[] sortOrder = sortOrder;

    public PartyRole GetRoleByIndex(int index) {
        return sortOrder[index];
    }
}
