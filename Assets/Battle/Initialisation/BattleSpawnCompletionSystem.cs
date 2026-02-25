using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using DBUS.Battle.Components.Requests;
using DBUS.Battle.Components.Determinism;
using DBUS.Battle.Components.Setup;
using DBUS.Battle.Components.Ownership;

[UpdateInGroup(typeof(BattleSpawningGroup))]
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

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (battleState, battle)
                in SystemAPI.Query<RefRO<BattleState>>()
                            .WithAll<BattleTag>()
                            .WithNone<BattleSpawningCompleteTag>()
                            .WithEntityAccess())
        {
            if (battleState.ValueRO.Phase != BattlePhase.Spawning)
                continue;

            ecb.AddComponent<BattleSpawningCompleteTag>(battle);

            Logging.System("[Battle] Battle spawn requests resolved.");
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}


