using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;

namespace SpiritbondWatcher;

internal sealed class Plugin : IDalamudPlugin
{
    private const string Command = "/sbw";

    private DalamudPluginInterface PluginInterface { get; init; }
    private ICommandManager CommandManager { get; init; }
    private IClientState Client { get; init; }
    private IDataManager Data { get; init; }
    private IChatGui Chat { get; init; }
    private Config Config { get; init; }
    private ConfigUI ConfigUI { get; init; }

    public Plugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
        [RequiredVersion("1.0")] ICommandManager commandManager,
        [RequiredVersion("1.0")] IClientState client,
        [RequiredVersion("1.0")] IDataManager data,
        [RequiredVersion("1.0")] IChatGui chat)
    {
        PluginInterface = pluginInterface;
        CommandManager = commandManager;
        Client = client;
        Data = data;
        Chat = chat;

        Config = PluginInterface.GetPluginConfig() as Config ?? new Config();
        Config.Initialize(PluginInterface);
        ConfigUI = new ConfigUI(Config);

        CommandManager.AddHandler(Command, new CommandInfo(this.OnCommand)
        {
            HelpMessage = "Display bonded gear"
        });
        Client.TerritoryChanged += this.OnZoneChange;

        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
    }

    private void OnZoneChange(ushort e)
    {
        OnCommand(Command, "zone");
    }

    private void OnCommand(string cmd, string args)
    {
        Task.Run(() => GearChecker.CheckGear(Data, Chat, Config, args));
    }

    private void DrawUI()
    {
        ConfigUI.Draw();
    }

    private void DrawConfigUI()
    {
        ConfigUI.Visible = true;
    }

    public void Dispose()
    {
        CommandManager.RemoveHandler(Command);
        Client.TerritoryChanged -= OnZoneChange;
    }
}