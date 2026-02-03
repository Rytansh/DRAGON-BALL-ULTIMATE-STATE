using Unity.Entities;
using Unity.Collections;
using DBUS.Core.Components.Turns;
using DBUS.Core.Components.Determinism;
using DBUS.Core.Components.Requests;
public partial struct BattleTurnEndSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (battleState, turnState, turnCounter, maxAP, remainingAP, battleEntity)
                 in SystemAPI.Query<
                     RefRO<BattleState>,
                     RefRW<TurnState>,
                     RefRW<TurnCounter>,
                     RefRO<MaxActionPoints>,
                     RefRW<RemainingActionPoints>>()
                    .WithAll<AdvanceTurnRequest>()
                    .WithEntityAccess())
        {
            if (battleState.ValueRO.Current != BattlePhase.Running)
                continue;

            if (turnState.ValueRO.Current != TurnPhase.End)
                continue;

                Logging.System("[Turn] Action points at the end of turn " + turnCounter.ValueRW.CurrentTurn + ": " + remainingAP.ValueRW.Value);
                ecb.RemoveComponent<AdvanceTurnRequest>(battleEntity);
                turnState.ValueRW.Current = TurnPhase.Start;

        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

