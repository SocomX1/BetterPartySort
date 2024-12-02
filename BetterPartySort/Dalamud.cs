using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace BetterPartySort;

public class Dalamud {
    [PluginService]
    public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;

    [PluginService]
    public static ITextureProvider TextureProvider { get; private set; } = null!;

    [PluginService]
    public static ICommandManager CommandManager { get; private set; } = null!;

    [PluginService]
    public static IPartyList PartyList { get; private set; } = null!;

    [PluginService]
    public static IGameConfig GameConfig { get; private set; } = null!;

    [PluginService]
    public static IAddonEventManager AddonManager { get; private set; } = null!;

    [PluginService]
    public static IPluginLog PluginLog { get; private set; } = null!;

    [PluginService]
    public static IAddonLifecycle AddonLifecycle { get; private set; } = null!;

    [PluginService]
    public static IObjectTable ObjectTable { get; private set; } = null!;
    
    [PluginService]
    public static IClientState ClientState { get; private set; } = null!;
    
    [PluginService]
    public static IDutyState DutyState { get; private set; } = null!;
    
    [PluginService]
    public static ICondition Condition { get; private set; } = null!;

    [PluginService]
    public static IGameInteropProvider GameInteropProvider { get; private set; } = null!;
    
    [PluginService]
    public static IFramework Framework { get; private set; } = null!;
    
    public static void Log(string message) {
        PluginLog.Info(message);
    }
}
