using Unity.Entities;
using Unity.Collections;
using DBUS.Core.Components.Determinism;
using DBUS.Core.Components.Turns;
public partial struct BattleInitialisationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb =
            new EntityCommandBuffer(Allocator.Temp);

        foreach (var (battleState, entity)
                 in SystemAPI.Query<RefRO<BattleState>>()
                             .WithAll<BattleTag>()
                             .WithNone<TurnCounter>()
                             .WithNone<TurnState>()
                             .WithNone<MaxActionPoints>()
                             .WithNone<RemainingActionPoints>()
                             .WithEntityAccess())
        {
            if (battleState.ValueRO.Current != BattlePhase.Running)
                continue;

            ecb.AddComponent(entity, new TurnCounter { CurrentTurn = 0 });
            ecb.AddComponent(entity, new MaxActionPoints { Value = 4 });
            ecb.AddComponent(entity, new RemainingActionPoints { Value = 2 });
            ecb.AddComponent(entity, new TurnState { Current = TurnPhase.Start });
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
