using Unity.Entities;
using Unity.Collections;
using DBUS.Core.Components.Combat;
using DBUS.Core.Components.Requests;
using DBUS.Core.Components.Determinism;

public partial struct BattleSpawnRequestSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (battleState, battle)
                 in SystemAPI.Query<RefRW<BattleState>>()
                             .WithAll<BattleTag>()
                             .WithEntityAccess())
        {
            if (battleState.ValueRO.Phase != BattlePhase.Initialising)
                continue;

            Entity req1 = ecb.CreateEntity();

            ecb.AddComponent(req1, new SpawnCharacterRequest
            {
                Battle = battle,
                Slot = 1,
                MaxHealth = 100,
                Attack = 20,
                Defense = 10
            });

            Entity req2 = ecb.CreateEntity();

            ecb.AddComponent(req2, new SpawnCharacterRequest
            {
                Battle = battle,
                Slot = 2,
                MaxHealth = 120,
                Attack = 15,
                Defense = 20
            });

            battleState.ValueRW.Phase = BattlePhase.Spawning;
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}


