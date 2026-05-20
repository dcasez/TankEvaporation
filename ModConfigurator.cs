using Bindito.Core;

namespace TankEvaporation;

[Context("Game")]
public class ModConfigurator : Configurator
{
    public override void Configure()
    {
        Bind<TankEvaporationSettings>().AsSingleton();
        Bind<EvaporationService>().AsSingleton();
    }
}