using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using DBUS.Core.Components.Requests;
using DBUS.Core.Components.Determinism;
public partial struct BattleSetupCompleteSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (battleState, battleEntity)
                 in SystemAPI.Query<RefRW<BattleState>>()
                             .WithAll<BattleTag>()
                             .WithNone<SpawnCharacterRequest>()
                             .WithEntityAccess())
        {
            if (battleState.ValueRO.Current != BattlePhase.SetupComplete)
                continue;

            battleState.ValueRW.Current = BattlePhase.Running;
            Logging.System("Battle is now running.");
        }
    }
}


