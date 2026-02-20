using Unity.Entities;
using Unity.Collections;
using DBUS.Core.Components.Turns;
using DBUS.Core.Components.Determinism;
using DBUS.Core.Components.Requests;
public partial struct TurnStartSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (battleState, turnCounter, maxAP, remainingAP)
                 in SystemAPI.Query<
                     RefRW<BattleState>,
                     RefRW<TurnCounter>,
                     RefRO<MaxActionPoints>,
                     RefRW<RemainingActionPoints>>())
        {
            if (battleState.ValueRO.Phase != BattlePhase.TurnStart)
                continue;

            turnCounter.ValueRW.CurrentTurn++;

            remainingAP.ValueRW.Value = maxAP.ValueRO.Value;

            Logging.System($"[Turn] Starting turn {turnCounter.ValueRW.CurrentTurn} with {remainingAP.ValueRO.Value}/{maxAP.ValueRO.Value} action points.");

            battleState.ValueRW.Phase = BattlePhase.Drawing;
        }
    }
}

