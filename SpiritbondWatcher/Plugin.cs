using System;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using System.Threading.Tasks;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SpiritbondWatcher;

internal sealed class Plugin : IDalamudPlugin
{
    private const string Command = "/spritbond";

    private IDalamudPluginInterface? PluginInterface { get; init; }
    private ICommandManager CommandManager { get; init; }
    private IClientState Client { get; init; }
    private IDataManager Data { get; init; }
    private IChatGui Chat { get; init; }
    private Config Config { get; init; }
    private ConfigUI ConfigUI { get; init; }
    private IGameGui GameGui { get; init; }

    private DalamudLinkPayload Payload { get; }

    public Plugin(IDalamudPluginInterface pluginInterface, ICommandManager commandManager, IClientState client, IDataManager data, IChatGui chat, IGameGui gui)
    {
        PluginInterface = pluginInterface;
        CommandManager = commandManager;
        Client = client;
        Data = data;
        Chat = chat;
        GameGui = gui;

        Config = PluginInterface.GetPluginConfig() as Config ?? new Config();
        Config.Initialize(PluginInterface);
        ConfigUI = new ConfigUI(Config);
        Payload = PluginInterface!.AddChatLinkHandler(1, HandleCommand);

        CommandManager.AddHandler(Command, new CommandInfo(OnCommand)
        {
            HelpMessage = "Display bonded gear"
        });
        Client.TerritoryChanged += OnZoneChange;

        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        PluginInterface.UiBuilder.OpenMainUi += DrawConfigUI;
    }

    private void OnZoneChange(ushort e)
    {
        OnCommand(Command, "zone");
    }

    private void OnCommand(string cmd, string args)
    {
        Task.Run(() => GearChecker.CheckGear(Payload, Data, Chat, Config, args));

        var moreInfo = new SeStringBuilder()
            .Add(Payload)
            .AddText("[")
            .AddUiForeground("Click to see more information", 32)
            .AddText("]")
            .Add(RawPayload.LinkTerminator)
            .Build();
        Chat.Print(moreInfo);
    }

    private void DrawUI()
    {
        ConfigUI.Draw();
    }

    private void DrawConfigUI()
    {
        ConfigUI.Visible = !ConfigUI.Visible;
    }

    private unsafe void HandleCommand(uint id, SeString message) {
        if (GameGui.GetAddonByName("MaterializeDialog") == IntPtr.Zero)
            ActionManager.Instance()->UseAction(ActionType.GeneralAction, 14);
    }

    public void Dispose()
    {
        PluginInterface!.RemoveChatLinkHandler();
        CommandManager.RemoveHandler(Command);
        Client.TerritoryChanged -= OnZoneChange;
    }
}