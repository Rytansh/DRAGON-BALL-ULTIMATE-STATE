using UnityEngine;

public class ConfigProcess : IBootstrapProcess
{
    public int Order => BootstrapOrderSequence.Config;

    public void Initialise(WorldContext context)
    {
        IConfigRegistry configRegistry = new ConfigRegistry();
        context.Register<IConfigRegistry>(configRegistry);
        
        IConfigProvider configProvider = new DefaultConfigProvider();
        configProvider.LoadConfigurations(context);
        context.Register<IConfigProvider>(configProvider);
    }
}
