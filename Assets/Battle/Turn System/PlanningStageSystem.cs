using Unity.Entities;
using Unity.Collections;
using DBUS.Battle.Components.Turns;
using DBUS.Battle.Components.Determinism;
using DBUS.Battle.Components.Requests;
using DBUS.Battle.Components.Ownership;
using DBUS.Battle.Components.Combat;

[UpdateInGroup(typeof(PlanningStageGroup))]
public partial struct PlanningStageSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        
        ProcessPlaceCardRequest(ref state, ecb);
        ProcessPlayActionRequest(ref state, ecb);
        ProcessEndPlanningRequest(ref state, ecb);
        //planning phase should not perform actions, only calculate action points, etc, and check if actions can be performed
        //all actions should be added to a queue later

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    private void ProcessPlaceCardRequest(ref SystemState state, EntityCommandBuffer ecb)
    {
        foreach (var (request, requestEntity)
         in SystemAPI.Query<RefRO<PlaceCardRequest>>()
                     .WithEntityAccess())
        {
            var player = request.ValueRO.Player;

            if (!SystemAPI.HasComponent<PlayerHand>(player) || !SystemAPI.HasComponent<RemainingActionPoints>(player) || !IsPlanningPhase(ref state, player))
            {
                ecb.DestroyEntity(requestEntity);
                continue;
            }

            var hand = SystemAPI.GetComponentRW<PlayerHand>(player);
            var ap   = SystemAPI.GetComponentRW<RemainingActionPoints>(player);

            if (ap.ValueRO.Value > 0 && hand.ValueRO.Current > 0)
            {
                hand.ValueRW.Current--;
                ap.ValueRW.Value--;
                Logging.System("[Battle] Placed a card from hand.");
            }
            else {Logging.Warning("[Battle] No action points or no cards in hand available.");}

            ecb.DestroyEntity(requestEntity);
        }
    }

    private void ProcessPlayActionRequest(ref SystemState state, EntityCommandBuffer ecb)
    {
        foreach (var (request, requestEntity)
         in SystemAPI.Query<RefRO<PlayActionRequest>>()
                     .WithEntityAccess())
        {
            var player = request.ValueRO.Player;

            if (!SystemAPI.HasComponent<RemainingActionPoints>(player) || !IsPlanningPhase(ref state, player))
            {
                ecb.DestroyEntity(requestEntity);
                continue;
            }

            var ap = SystemAPI.GetComponentRW<RemainingActionPoints>(player);

            if (ap.ValueRO.Value > 0) 
            {
                ap.ValueRW.Value--;
                Logging.System("[Battle] Expended 1 action point.");
            }
            else {Logging.Warning("[Battle] No action points remaining for the turn.");}

            ecb.DestroyEntity(requestEntity);
        }
    }
    private void ProcessEndPlanningRequest(ref SystemState state, EntityCommandBuffer ecb)
    {
        foreach (var (request, requestEntity)
         in SystemAPI.Query<RefRO<EndPlanningRequest>>()
                     .WithEntityAccess())
        {
            var player = request.ValueRO.Player;

            if (!SystemAPI.HasComponent<OwnedBattle>(player))
            {
                ecb.DestroyEntity(requestEntity);
                continue;
            }
            var ownedBattle = SystemAPI.GetComponent<OwnedBattle>(player);
            var battle = ownedBattle.Battle;

            if (SystemAPI.HasComponent<BattlePlanningCompleteTag>(battle)|| !IsPlanningPhase(ref state, player))
            {
                ecb.DestroyEntity(requestEntity);
                continue;
            }
            Logging.System("[Battle] Planning phase complete.");
            ecb.AddComponent<BattlePlanningCompleteTag>(battle);
            ecb.DestroyEntity(requestEntity);
        }
    }

    private bool IsPlanningPhase(ref SystemState state, Entity player)
    {
        if (!SystemAPI.HasComponent<OwnedBattle>(player)) return false;
        var ownedBattle = SystemAPI.GetComponent<OwnedBattle>(player);
        var battleState = SystemAPI.GetComponent<BattleState>(ownedBattle.Battle);
        return battleState.Phase == BattlePhase.Planning;
    }

}