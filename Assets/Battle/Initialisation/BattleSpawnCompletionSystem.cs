using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using DBUS.Core.Components.Requests;
using DBUS.Core.Components.Determinism;
public partial struct BattleSpawnCompletionSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        bool spawnRequestsExist =
            SystemAPI.QueryBuilder()
                     .WithAll<SpawnCharacterRequest>()
                     .Build()
                     .CalculateEntityCount() > 0;

        if (spawnRequestsExist)
            return;

        foreach (var (battleState, battle)
                 in SystemAPI.Query<RefRW<BattleState>>()
                             .WithAll<BattleTag>()
                             .WithEntityAccess())
        {
            if (battleState.ValueRO.Phase != BattlePhase.Spawning)
                continue;

            battleState.ValueRW.Phase = BattlePhase.BattleReady;
            Logging.System("[Battle] Battle initialised and running.");
        }
    }
}


