namespace SpiritbondWatcher;

using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

[Serializable]
public class Config : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool BondedGearDisplayLineByLine { get; set; }

    [NonSerialized]
    private DalamudPluginInterface? _pluginInterface;

    public void Initialize(DalamudPluginInterface pluginInterface)
    {
        _pluginInterface = pluginInterface;
    }

    public void Save()
    {
        _pluginInterface!.SavePluginConfig(this);
    }
}
