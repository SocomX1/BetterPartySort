using System;
using System.Collections.Generic;
using System.IO;
using BetterPartySort.Windows;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;

namespace BetterPartySort;

public sealed class Plugin : IDalamudPlugin {
    private const string CommandName = "/pmycommand";
    private const string SortCommandName = "/bsort";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("BetterPartySort");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    private Dictionary<PartyRole, uint> partyDict;

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
            HelpMessage = "A useful message to display in /xlhelp"
        });

        Dalamud.CommandManager.AddHandler(SortCommandName, new CommandInfo(OnDebugCommand) {
            HelpMessage = "Output party and sort order values to console"
        });

        Dalamud.CommandManager.AddHandler("/assign", new CommandInfo(OnRoleCommand) {
            HelpMessage = "designate party member role"
        });

        Dalamud.PluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        Dalamud.PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        Dalamud.PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        partyDict = new Dictionary<PartyRole, uint>(8);
    }

    public void Dispose() {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        Dalamud.CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args) {
        // in response to the slash command, just toggle the display status of our main ui
        ToggleMainUI();
    }

    private unsafe void OnDebugCommand(string command, string args) {
        sortParty();
    }

    private unsafe void OnRoleCommand(string command, string args) {
        Dalamud.Log(AgentHUD.Instance()->CurrentTargetId.ToString());
        Enum.TryParse(args, out PartyRole role);
        partyDict.Add(role, AgentHUD.Instance()->CurrentTargetId);
    }


    private unsafe void sortParty() {
        var partyList = AgentHUD.Instance()->PartyMembers;

        PartyConfiguration partyConfig = new PartyConfiguration("Light parties", "LP1->LP2", [
            PartyRole.Tank1, PartyRole.Healer1, PartyRole.Melee1, PartyRole.Ranged1,
            PartyRole.Tank2, PartyRole.Healer2, PartyRole.Melee2, PartyRole.Ranged2
        ]);

        for (int i = 0; i < partyList.Length; i++) {
            Dalamud.Log(i + ": " + partyList[i].EntityId + ", " + partyList[i].Index);
        }

        for (int i = 0; i < 8; i++) {
            var role = partyConfig.GetRoleByIndex(i);
            var id = partyDict[role];
            var targetIndex = SortUtility.GetIndexByEntityId(id);
            string sortParams = "/psort " + (i + 1) + " " + (targetIndex + 1);

            Dalamud.Log(sortParams + ": " + role + ", " + id + ", " + targetIndex);

            SortUtility.UpdatePartyMemberIndex((byte)i, targetIndex);
            if (i != targetIndex)
                RaptureShellModule.Instance()->ShellCommandModule.ExecuteCommandInner(
                    Utf8String.FromString(sortParams), RaptureShellModule.Instance()->UIModule);
        }
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
}
