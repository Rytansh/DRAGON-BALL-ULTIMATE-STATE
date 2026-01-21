using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using DBUS.Core.Components.Requests;
using DBUS.Core.Components.Determinism;
public partial struct BattleBootstrapCompleteSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var battleState
                 in SystemAPI.Query<RefRW<BattleState>>()
                             .WithAll<BattleTag>())
        {
            if (battleState.ValueRO.Current != BattlePhase.Bootstrapping)
                continue;

            battleState.ValueRW.Current = BattlePhase.Running;
            Logging.System("Battle is now running.");
        }
    }
}


