using System;
using Unity.Entities;
using Unity.Collections;
using DBUS.Core.Components.Combat;
using DBUS.Core.Components.Requests;
using DBUS.Core.Components.Determinism;

public partial struct BattleCreationSystem : ISystem 
{ 
    public void OnUpdate(ref SystemState state) 
    { 
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp); 
        foreach (var (_, requestEntity) in SystemAPI.Query<RefRO<StartBattleRequest>>().WithEntityAccess()) 
        { 
            Entity battleEntity = ecb.CreateEntity(); 
            ecb.AddComponent<BattleTag>(battleEntity); 
            ecb.AddComponent(battleEntity, new BattleState {Phase = BattlePhase.Creating}); 
            ecb.DestroyEntity(requestEntity); 
        } 
        
        ecb.Playback(state.EntityManager); 
        ecb.Dispose(); } }

