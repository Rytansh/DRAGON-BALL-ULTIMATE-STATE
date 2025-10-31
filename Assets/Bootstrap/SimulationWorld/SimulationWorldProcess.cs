using UnityEngine;

public class SimulationWorldProcess : IBootstrapProcess
{
    public int Order => BootstrapOrderSequence.SimulationWorld;

    public void Initialise(WorldContext rootContext)
    {
        SimulationWorld simulationWorld = new SimulationWorld(rootContext);
        simulationWorld.Initialise();

        rootContext.Register<SimulationWorld>(simulationWorld);

        Logging.System("Simulation world initialised.");
    }
}

