using Bindito.Core;
using ModSettings.Common;
using ModSettings.Core;
using Timberborn.Modding;
using Timberborn.SettingsSystem;
using Timberborn.SingletonSystem;

namespace TankEvaporation;

public class TankEvaporationSettings : ModSettingsOwner, IUnloadableSingleton
{
    public static TankEvaporationSettings? Instance { get; private set; }

    public RangeIntModSetting EvaporationPercent { get; } = new(
        5, 1, 20,
        ModSettingDescriptor.CreateLocalized("TE.EvaporationPercent")
            .SetLocalizedTooltip("TE.EvaporationPercentTooltip"));

    public ModSetting<bool> WaterOnly { get; } = new(
        false,
        ModSettingDescriptor.CreateLocalized("TE.WaterOnly")
            .SetLocalizedTooltip("TE.WaterOnlyTooltip"));

    protected override string ModId => "TankEvaporation";

    public TankEvaporationSettings(
        ISettings settings,
        ModSettingsOwnerRegistry modSettingsOwnerRegistry,
        ModRepository modRepository)
        : base(settings, modSettingsOwnerRegistry, modRepository) { }

    protected override void OnAfterLoad()
    {
        base.OnAfterLoad();
        Instance = this;
    }

    public void Unload()
    {
        Instance = null;
    }
}

[Context("MainMenu")]
public class SettingsMenuConfig : Configurator
{
    public override void Configure()
    {
        Bind<TankEvaporationSettings>().AsSingleton();
    }
}