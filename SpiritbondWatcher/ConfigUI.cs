namespace SpiritbondWatcher;

using Dalamud.Interface.Components;
using ImGuiNET;
using System;
using System.Numerics;

internal class ConfigUI : IDisposable
{
    private readonly Config _configuration;

    private const ImGuiWindowFlags Flags = ImGuiWindowFlags.NoResize |
                                           ImGuiWindowFlags.NoCollapse |
                                           ImGuiWindowFlags.NoScrollbar |
                                           ImGuiWindowFlags.NoScrollWithMouse;

    private bool _visible;
    public bool Visible
    {
        set => _visible = value;
    }

    public ConfigUI(Config configuration)
    {
        _configuration = configuration;
    }

    public void Draw()
    {
        if (!_visible)
        {
            return;
        }

        ImGui.SetNextWindowSize(new Vector2(232, 75), ImGuiCond.Always);
        if (ImGui.Begin("Spiritbond Watcher", ref this._visible, Flags))
        {
            var lineByLineValue = this._configuration.BondedGearDisplayLineByLine;
            if (ImGui.Checkbox("Display gear line by line", ref lineByLineValue))
            {
                if (_configuration.BondedGearDisplayLineByLine != lineByLineValue)
                {
                    _configuration.BondedGearDisplayLineByLine = lineByLineValue;
                    _configuration.Save();
                }
            }
            ImGuiComponents.HelpMarker("Display bonded gear line by line");
        }
        ImGui.End();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}