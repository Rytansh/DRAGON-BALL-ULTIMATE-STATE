using Unity.Entities;
using UnityEngine;

namespace Archeus.Game.Bootstrap
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct SimulationWorldProbeSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            Debug.Log("[ECS Bootstrap] Archeus Simulation World is updating.");

            state.Enabled = false;
        }
    }
}
