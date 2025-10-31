using UnityEngine;

public class RNGDeterminationProcess : IBootstrapProcess
{
    public int Order => BootstrapOrderSequence.RNG;

    public void Initialise(WorldContext context)
    {
        ISeedProvider seedProvider = context.Resolve<ISeedProvider>();
        
        ulong WorldSeed = seedProvider.WorldSeed;

        IRNGProvider rngProvider = new DefaultRNGProvider(WorldSeed);
        context.Register<IRNGProvider>(rngProvider);
    }
}
