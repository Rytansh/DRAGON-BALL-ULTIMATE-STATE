using Archeus.Core.Debugging;
using Unity.Entities;

namespace Archeus.Game.Bootstrap
{
    public sealed class BattleSimulationWorldProcess : IBootstrapProcess
    {
        public int Order => SimulationBootstrapOrder.SimulationWorld;

        public void Initialise(WorldContext rootContext)
        {
            World ecsWorld = BattleSimulationBootstrap.SimulationEcsWorld;

            if (ecsWorld == null || !ecsWorld.IsCreated)
            {
                throw new System.InvalidOperationException(
                    "Archeus Simulation ECS World has not been created."
                );
            }

            BattleSimulationWorld simulationWorld = new BattleSimulationWorld(ecsWorld);

            rootContext.Register(simulationWorld);

            Logging.Info(
                LogCategory.Setup,
                $"Simulation world registered using ECS World: " + $"{ecsWorld.Name}"
            );
        }
    }
}
