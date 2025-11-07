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

        ISeedService rootSeedService = rootContext.Resolve<ISeedService>();
        IRNGProvider localRNG = new DefaultRNGProvider(rootSeedService);

        localContext.Register<ISeedService>(rootSeedService);
        localContext.Register<IRNGProvider>(localRNG);

        Logging.System("SimulationWorld context created.");
    }

    public void Dispose()
    {
        // Clean-up logic later
        Logging.System("SimulationWorld disposed.");
    }
}

