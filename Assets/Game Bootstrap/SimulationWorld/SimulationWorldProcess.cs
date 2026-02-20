using Unity.Entities;

public class SimulationWorldProcess : IBootstrapProcess
{
    public int Order => SimulationBootstrapOrder.SimulationWorld;

    public void Initialise(WorldContext rootContext)
    {
        SimulationWorld simulationWorld = new SimulationWorld(rootContext);
        simulationWorld.Initialise();

        World ecsWorld = World.DefaultGameObjectInjectionWorld;

        simulationWorld.localContext.Register<World>(ecsWorld);

        rootContext.Register<SimulationWorld>(simulationWorld);

        Logging.System($"Simulation world initialised using ECS World: {ecsWorld.Name}");
    }
}

