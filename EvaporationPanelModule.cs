using Bindito.Core;
using Timberborn.EntityPanelSystem;

namespace TankEvaporation;

[Context("Game")]
public class EvaporationPanelModule : Configurator
{
    public override void Configure()
    {
        Bind<EvaporationFragment>().AsSingleton();
        MultiBind<EntityPanelModule>().ToProvider<EvaporationPanelProvider>().AsSingleton();
    }
}

public class EvaporationPanelProvider : IProvider<EntityPanelModule>
{
    private readonly EvaporationFragment _fragment;

    public EvaporationPanelProvider(EvaporationFragment fragment)
    {
        _fragment = fragment;
    }

    public EntityPanelModule Get()
    {
        var builder = new EntityPanelModule.Builder();
        builder.AddBottomFragment(_fragment);
        return builder.Build();
    }
}