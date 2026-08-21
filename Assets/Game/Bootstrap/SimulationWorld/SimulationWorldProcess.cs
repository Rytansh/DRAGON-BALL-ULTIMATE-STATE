using Archeus.Core.Debugging;
using Unity.Entities;

namespace Archeus.Game.Bootstrap
{
    public class SimulationWorldProcess : IBootstrapProcess
    {
        public int Order => SimulationBootstrapOrder.SimulationWorld;

        public void Initialise(WorldContext rootContext)
        {
            SimulationWorld simulationWorld = new SimulationWorld(rootContext);
            simulationWorld.Initialise();

            rootContext.Register<SimulationWorld>(simulationWorld);

            Logging.Info(LogCategory.Setup,$"Simulation world initialised using ECS World: {rootContext.Resolve<World>()}");
        }
    }
}

