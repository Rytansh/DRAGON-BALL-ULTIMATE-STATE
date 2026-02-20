using Unity.Entities;
using DBUS.Core.Components.Determinism;
using DBUS.Core.Components.Turns;
public partial struct BattleBeginTurnSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var battleState
                 in SystemAPI.Query<RefRW<BattleState>>())
        {
            if (battleState.ValueRO.Phase != BattlePhase.BattleReady)
                continue;

            Logging.System("[Battle] Entering turn system.");

            battleState.ValueRW.Phase = BattlePhase.TurnStart;
        }
    }
}