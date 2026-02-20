using Unity.Entities;
using Unity.Collections;
using DBUS.Core.Components.Turns;
using DBUS.Core.Components.Determinism;
using DBUS.Core.Components.Requests;
public partial struct TurnEndSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (battleState, turnCounter)
                 in SystemAPI.Query<
                     RefRW<BattleState>,
                     RefRW<TurnCounter>>())
        {
            if (battleState.ValueRO.Phase != BattlePhase.TurnEnd)
                continue;

            Logging.System($"[Turn] Ending turn {turnCounter.ValueRO.CurrentTurn}.");

            battleState.ValueRW.Phase = BattlePhase.TurnStart;
        }
    }
}


