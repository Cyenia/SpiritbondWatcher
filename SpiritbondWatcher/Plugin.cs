using Dalamud.Data;
using Dalamud.Game.ClientState;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.Threading.Tasks;

namespace SpiritbondWatcher;

internal sealed class Plugin : IDalamudPlugin
{
    public string Name => "Spiritbond Watcher";
    private const string Command = "/sbw";

    private DalamudPluginInterface PluginInterface { get; init; }
    private CommandManager CommandManager { get; init; }
    private ClientState Client { get; init; }
    private DataManager Data { get; init; }
    private ChatGui Chat { get; init; }
    private Config Config { get; init; }
    private ConfigUI ConfigUI { get; init; }

    public Plugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
        [RequiredVersion("1.0")] CommandManager commandManager,
        [RequiredVersion("1.0")] ClientState client,
        [RequiredVersion("1.0")] DataManager data,
        [RequiredVersion("1.0")] ChatGui chat)
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

    private void OnZoneChange(object sender, ushort e)
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