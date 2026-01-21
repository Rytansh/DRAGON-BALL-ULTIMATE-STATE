using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using DBUS.Core.Components.Turns;
using DBUS.Core.Components.Requests;

public partial struct DebugAdvanceTurnSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (turnState, battleEntity)
                 in SystemAPI.Query<RefRO<TurnState>>()
                              .WithAll<BattleTag>()
                              .WithNone<AdvanceTurnRequest>()
                              .WithEntityAccess())
        {
            if (turnState.ValueRO.Current != TurnPhase.End)
                continue;

            ecb.AddComponent<AdvanceTurnRequest>(battleEntity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
