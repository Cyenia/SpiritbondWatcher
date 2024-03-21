namespace SpiritbondWatcher;

using Dalamud.Interface.Components;
using ImGuiNET;
using System;
using System.Numerics;

internal class ConfigUI(Config configuration) : IDisposable
{
    private const ImGuiWindowFlags Flags = ImGuiWindowFlags.NoResize |
                                           ImGuiWindowFlags.NoCollapse |
                                           ImGuiWindowFlags.NoScrollbar |
                                           ImGuiWindowFlags.NoScrollWithMouse;

    private bool _visible;
    public bool Visible
    {
        set => _visible = value;
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
            var lineByLineValue = configuration.BondedGearDisplayLineByLine;
            if (ImGui.Checkbox("Display gear line by line", ref lineByLineValue))
            {
                if (configuration.BondedGearDisplayLineByLine != lineByLineValue)
                {
                    configuration.BondedGearDisplayLineByLine = lineByLineValue;
                    configuration.Save();
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