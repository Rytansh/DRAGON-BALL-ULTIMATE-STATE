using System;
using Unity.Entities;
using Unity.Collections;
using DBUS.Battle.Components.Requests;
using DBUS.Battle.Components.Determinism;
using DBUS.Battle.Components.Ownership;

[UpdateInGroup(typeof(BattleCreationGroup))]
public partial struct BattleCreationSystem : ISystem 
{ 
    public void OnUpdate(ref SystemState state) 
    { 
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp); 
        foreach (var (_, requestEntity) in SystemAPI.Query<RefRO<StartBattleRequest>>().WithEntityAccess()) 
        { 
            Entity battleEntity = ecb.CreateEntity(); 
            ecb.AddComponent<BattleTag>(battleEntity);
            ecb.AddComponent(battleEntity, new BattleState { Phase = BattlePhase.Creating });

            Entity player = ecb.CreateEntity();
            ecb.AddComponent(player, new PlayerTag {});
            ecb.AddComponent(player, new OwnedBattle { Battle = battleEntity }); 

            ecb.DestroyEntity(requestEntity); 

            Logging.System("[Battle] Battle created.");
        } 
        
        ecb.Playback(state.EntityManager); 
        ecb.Dispose(); } }

