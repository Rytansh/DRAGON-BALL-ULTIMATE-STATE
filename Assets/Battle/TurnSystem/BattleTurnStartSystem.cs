using Unity.Entities;
using Unity.Collections;
using DBUS.Core.Components.Turns;
using DBUS.Core.Components.Determinism;
using DBUS.Core.Components.Requests;
public partial struct BattleTurnStartSystem : ISystem
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
                    .WithNone<AdvanceTurnRequest>()
                    .WithEntityAccess())
        {
            if (battleState.ValueRO.Current != BattlePhase.Running)
                continue;

            if (turnState.ValueRO.Current != TurnPhase.Start)
                continue;

            turnCounter.ValueRW.CurrentTurn++;
            Logging.System("[Turn] Starting turn: " + turnCounter.ValueRW.CurrentTurn);
            remainingAP.ValueRW.Value = maxAP.ValueRO.Value;
            Logging.System("[Turn] Action points at the start of turn " + turnCounter.ValueRW.CurrentTurn + ": " + remainingAP.ValueRW.Value);

            turnState.ValueRW.Current = TurnPhase.End;
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

