using System;
using System.IO;
using System.Text;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Command;
using Dalamud.Game.Config;
using Dalamud.Interface.Internal.Windows.Data.Widgets;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using SamplePlugin.Windows;

namespace SamplePlugin;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService]
    internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;

    [PluginService]
    internal static ITextureProvider TextureProvider { get; private set; } = null!;

    [PluginService]
    internal static ICommandManager CommandManager { get; private set; } = null!;

    [PluginService]
    internal static IPartyList PartyList { get; private set; } = null!;

    [PluginService]
    internal static IGameConfig GameConfig { get; private set; } = null!;

    [PluginService]
    internal static IPluginLog PluginLog { get; private set; } = null!;

    [PluginService]
    internal static IAddonLifecycle AddonLifecycle { get; private set; } = null!;

    private const string CommandName = "/pmycommand";
    private const string DebugCommandName = "/sortdata";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("SamplePlugin");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        // you might normally want to embed resources and load them from the manifest stream
        var goatImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this, goatImagePath);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        CommandManager.AddHandler(DebugCommandName, new CommandInfo(OnDebugCommand)
        {
            HelpMessage = "Output party and sort order values to console"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        AddonLifecycle.RegisterListener(AddonEvent.PreRequestedUpdate, "_PartyList", OnPreRequestUpdate);
    }

    private unsafe void debug()
    {
        // foreach (var partyMember in AgentHUD.Instance()->PartyMembers)
        // {
        //     PluginLog.Info(partyMember.Index + ":");
        // }
        //
        // var char1 = AgentHUD.Instance()->PartyMembers[0];
        // var char2 = AgentHUD.Instance()->PartyMembers[1];
        //
        // AgentHUD.Instance()->PartyMembers[0] = char2;
        // AgentHUD.Instance()->PartyMembers[1] = char1;


        // var partyManager = GroupManager.Instance()->MainGroup;

        // PluginLog.Info(PartyList.Length + " party members");
        // PluginLog.Info(PartyList.PartyId.ToString());

        // PluginLog.Info(partyManager.PartyId + " id");

        // foreach (var partyMember in partyManager.PartyMembers)
        // {
        //     PluginLog.Info(partyMember.NameString);
        // }

        // PluginLog.Info(partyManager.PartyMembers[0].NameString);
        // PluginLog.Info(partyManager.PartyMembers[1].NameString);

        // var char1 = partyManager.PartyMembers[0];
        // var char2 = partyManager.PartyMembers[1];
        // partyManager.PartyMembers[0] = char2;
        // partyManager.PartyMembers[1] = char1;


        // var uiConfigSection = GameConfig.UiConfig;
        // var dpsOrder =
        //     uiConfigSection.GetUInt(UiConfigOption.PartyListSortTypeDps.GetAttribute<GameConfigOptionAttribute>().Name);
        // GameConfig.TryGet(UiConfigOption.PartyListSortTypeDps, out UIntConfigProperties? configProperties);

        // GameConfig.Changed += LogUIChange;

        // PluginLog.Info(configProperties.ToString());

        // var partyList = Framework.Instance()->GetUIModule()->GetRaptureUiDataModule()->PartyListHealerOrder;
        // foreach (var job in partyList)
        // {
        //     PluginLog.Info(job.ToString());   
        // }

        // var partyList =
        //     (AddonPartyList*)Framework.Instance()->GetUIModule()->GetRaptureAtkModule()->AtkUnitManager->GetAddonByName(
        //         "_PartyList");
        // var party = partyList->TrustMembers;
        // var char1 = party[5];
        // var char2 = party[6];
        // party[6] = char1;
        // party[5] = char2;
        // PluginLog.Info(partyList->TrustCount.ToString());
    }

    private void OnPreRequestUpdate(AddonEvent eventType, AddonArgs args)
    {
        var numericArgs = ((AddonRequestedUpdateArgs)args).NumberArrayData;
        var stringArgs = ((AddonRequestedUpdateArgs)args).StringArrayData;
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just toggle the display status of our main ui
        ToggleMainUI();
    }

    private void OnDebugCommand(string command, string args)
    {
        string[] strings = args.Split(' ');
        swapPartyMembers(Convert.ToInt32(strings[0]), Convert.ToInt32(strings[1]));
    }

    private unsafe void swapPartyMembers(int index1, int index2)
    {
        var partyList =
            (AddonPartyList*)Framework.Instance()->GetUIModule()->GetRaptureAtkModule()->AtkUnitManager->GetAddonByName(
                "_PartyList");
        var party = partyList->TrustMembers;
        var char1 = party[index1];
        var char2 = party[index2];
        party[index1] = char2;
        party[index2] = char1;
    }

    // public void LogUIChange(object? sender, ConfigChangeEvent e)
    // {
    //     PluginLog.Info(sender.ToString());
    //     PluginLog.Info(e.ToString());
    // }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
}
