using System.IO;
using BetterPartySort.Config;
using BetterPartySort.Windows;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;

namespace BetterPartySort;

public sealed class Plugin : IDalamudPlugin {
    private const string CommandName = "/bps";
    private const string SortCommandName = "/bsort";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("BetterPartySort");
    private ConfigWindow ConfigWindow { get; init; }

    public readonly SortManager SortManager;

    public Plugin(IDalamudPluginInterface pluginInterface) {
        pluginInterface.Create<Dalamud>();
        Configuration = Dalamud.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        
        ConfigWindow = new ConfigWindow(this);

        WindowSystem.AddWindow(ConfigWindow);

        Dalamud.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
            HelpMessage = "Open the BetterPartySort menu."
        });

        Dalamud.CommandManager.AddHandler(SortCommandName, new CommandInfo(OnSortCommand) {
            HelpMessage = "Sort the party list according to the specified configuration."
        });

        Dalamud.PluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        Dalamud.PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin

        // Dalamud.ClientState.TerritoryChanged += OnTerritoryChange;

        SortManager = new SortManager(this);

        Dalamud.DutyState.DutyStarted += OnDutyCommence;
    }

    private void OnDutyCommence(object? sender, ushort e) {
        if (Configuration.SortOnDutyCommence) {
            SortManager.SortParty();
        }
    }

    // private void OnTerritoryChange(ushort territoryId) {
    //     Dalamud.Condition.ConditionChange += OnConditionChange;
    //     Dalamud.Log("Registering condition listener.");
    // }
    //
    // private unsafe void OnConditionChange(ConditionFlag flag, bool value) {
    //     var conditions = Dalamud.Condition.AsReadOnlySet().ToList();
    //         // Dalamud.Log("---------------Begin condition list----------------");
    //         // foreach (var condition in conditions) {
    //         //     Dalamud.Log(condition.ToString());
    //         // }
    //         //
    //     if (conditions.Count == 3 && conditions.Contains(ConditionFlag.NormalConditions) &&
    //         conditions.Contains(ConditionFlag.BoundByDuty) && conditions.Contains(ConditionFlag.BoundByDuty56)) {
    //         Dalamud.Log("Sorting party list and unregistering condition listener.");
    //         var party = AgentHUD.Instance()->PartyMembers;
    //         foreach (var player in party) {
    //             if (player.Object != null) {
    //                 Dalamud.Log(player.Object->NameString);
    //             }
    //         }
    //         Dalamud.Condition.ConditionChange -= OnConditionChange;
    //         SortManager.SortParty();
    //     }
    //     else if (conditions.Count == 1 && conditions.Contains(ConditionFlag.NormalConditions)) {
    //         Dalamud.Condition.ConditionChange -= OnConditionChange;
    //         Dalamud.Log("Unregistering condition listener.");
    //     }
    // }

    public void Dispose() {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();

        Dalamud.CommandManager.RemoveHandler(CommandName);
        Dalamud.CommandManager.RemoveHandler(SortCommandName);

        Dalamud.DutyState.DutyStarted -= OnDutyCommence;
    }

    private void OnCommand(string command, string args) {
        ToggleConfigUI();
    }

    private void OnSortCommand(string command, string args) {
        SortManager.SortParty();
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
}
