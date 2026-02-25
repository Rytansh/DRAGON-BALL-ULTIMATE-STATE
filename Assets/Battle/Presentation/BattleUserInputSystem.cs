using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using DBUS.Battle.Components.Turns;
using DBUS.Battle.Components.Requests;
using DBUS.Battle.Components.Determinism;
using DBUS.Battle.Components.Ownership;

public partial struct BattleUserInputSystem : ISystem
{

    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (ownedBattle, player)
                    in SystemAPI.Query<
                        RefRO<OwnedBattle>>()
                        .WithAll<PlayerTag>()
                        .WithEntityAccess())
        {
            var battleState = SystemAPI.GetComponent<BattleState>(ownedBattle.ValueRO.Battle);
            if(battleState.Phase == BattlePhase.Planning)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Entity inputEntity = ecb.CreateEntity();
                    ecb.AddComponent<InputSystemTag>(inputEntity);
                    ecb.AddComponent(inputEntity, new EndPlanningRequest{Player = player});
                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    Entity inputEntity = ecb.CreateEntity();
                    ecb.AddComponent<InputSystemTag>(inputEntity);
                    ecb.AddComponent(inputEntity, new PlayActionRequest{Player = player});
                }
                if (Input.GetKeyDown(KeyCode.W))
                {
                    Entity inputEntity = ecb.CreateEntity();
                    ecb.AddComponent<InputSystemTag>(inputEntity);
                    ecb.AddComponent(inputEntity, new PlaceCardRequest{Player = player});
                }
            }
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

