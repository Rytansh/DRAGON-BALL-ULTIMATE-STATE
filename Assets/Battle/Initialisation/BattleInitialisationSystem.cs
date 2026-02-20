using Unity.Entities;
using Unity.Collections;
using DBUS.Core.Components.Determinism;
using DBUS.Core.Components.Turns;
public partial struct BattleInitialisationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (battleState, battle)
                 in SystemAPI.Query<RefRW<BattleState>>()
                             .WithAll<BattleTag>()
                             .WithEntityAccess())
        {
            if (battleState.ValueRO.Phase != BattlePhase.Creating)
                continue;

            ecb.AddComponent(battle, new TurnCounter { CurrentTurn = 0 });

            ecb.AddComponent(battle, new MaxActionPoints { Value = 4 });

            ecb.AddComponent(battle, new RemainingActionPoints { Value = 4 });

            battleState.ValueRW.Phase = BattlePhase.Initialising;
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

