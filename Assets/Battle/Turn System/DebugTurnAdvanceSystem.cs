using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using DBUS.Core.Components.Turns;
using DBUS.Core.Components.Requests;

public partial struct DebugTurnAdvanceSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        foreach (var battleState
                 in SystemAPI.Query<RefRW<BattleState>>())
        {
            if (battleState.ValueRO.Phase != BattlePhase.Drawing)
                continue;

            battleState.ValueRW.Phase = BattlePhase.TurnEnd;
        }
    }
}

