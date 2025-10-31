using UnityEngine;

public sealed class SimulationWorld
{
    public WorldContext localContext { get; private set; }
    private readonly WorldContext rootContext;

    public SimulationWorld(WorldContext rootContext)
    {
        this.rootContext = rootContext;
    }

    public void Initialise()
    {
        localContext = new WorldContext();

        ISeedProvider rootSeedProvider = rootContext.Resolve<ISeedProvider>();
        DefaultRNGProvider localRNG = new DefaultRNGProvider(rootSeedProvider.WorldSeed);

        localContext.Register<ISeedProvider>(rootSeedProvider);
        localContext.Register<IRNGProvider>(localRNG);

        Logging.System("SimulationWorld context created.");
    }

    public void Dispose()
    {
        // Clean-up logic later
        Logging.System("SimulationWorld disposed.");
    }
}

