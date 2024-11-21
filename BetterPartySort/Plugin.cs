using System.IO;
using BetterPartySort.Config;
using BetterPartySort.Party;
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
    private MainWindow MainWindow { get; init; }

    public readonly SortManager SortManager;
    public readonly PartyManager PartyManager;

    public Plugin(IDalamudPluginInterface pluginInterface) {
        pluginInterface.Create<Dalamud>();
        Configuration = Dalamud.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        // you might normally want to embed resources and load them from the manifest stream
        var goatImagePath = Path.Combine(Dalamud.PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this, goatImagePath);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

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
        Dalamud.PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        Dalamud.ClientState.TerritoryChanged += OnTerritoryChange;

        SortManager = new SortManager(this);
        PartyManager = new PartyManager();
    }

    public void Dispose() {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        Dalamud.CommandManager.RemoveHandler(CommandName);
        Dalamud.CommandManager.RemoveHandler(SortCommandName);
        
        Dalamud.ClientState.TerritoryChanged -= OnTerritoryChange;
    }

    private void OnCommand(string command, string args) {
        ToggleMainUI();
    }

    private void OnSortCommand(string command, string args) {
        SortManager.SortParty(int.Parse(args));

        foreach (var order in Configuration.PartyConfigurations) {
            Dalamud.Log("\n" + order.ConfigurationName);
            for (int i = 0; i < 8; i++) {
                Dalamud.Log(order.GetRoleByIndex(i).ToString());
            }
        }
    }

    private void OnTerritoryChange(ushort territoryId) {
        PartyManager.InitParty();
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
}
