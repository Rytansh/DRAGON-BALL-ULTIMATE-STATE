using Archeus.Core.Debugging;
using Unity.Entities;

namespace Archeus.Game.Bootstrap
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct BattleSimulationProbeSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            Logging.Info(LogCategory.System, "Archeus Simulation World is updating.");

            state.Enabled = false;
        }
    }
}
