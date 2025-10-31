using UnityEngine;

public class SeedGenProcess : IBootstrapProcess
{
    public int Order => BootstrapOrderSequence.Seeding;

    public void Initialise(WorldContext context)
    {
        DebugSeedProvider seedProvider = new DebugSeedProvider();
        context.Register<ISeedProvider>(seedProvider);
    }
}
