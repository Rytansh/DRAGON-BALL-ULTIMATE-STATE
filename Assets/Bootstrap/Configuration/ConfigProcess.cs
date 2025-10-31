using UnityEngine;

public class ConfigProcess : IBootstrapProcess
{
    public int Order => BootstrapOrderSequence.Config;

    public void Initialise(WorldContext context)
    {
        ConfigRegistry configRegistry = new ConfigRegistry();
        context.Register<IConfigRegistry>(configRegistry);
        
        DefaultConfigProvider configProvider = new DefaultConfigProvider();
        configProvider.LoadConfigurations(context);
        context.Register<IConfigProvider>(configProvider);
    }
}
