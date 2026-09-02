using Archeus.Battle.Components.Ownership;
using Archeus.Battle.Components.Requests;
using Archeus.Battle.Components.Tags;
using Archeus.Game.Bootstrap;
using Unity.Entities;
using UnityEngine;

public sealed class SimulationTestHarness : MonoBehaviour
{
    private EntityManager entityManager;

    private Entity player = Entity.Null;

    private void Start()
    {
        entityManager = BattleSimulationBootstrap.SimulationEcsWorld.EntityManager;
    }

    private void Update()
    {
        if (!TryResolvePlayer())
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateRequest(new EndPlanningRequest { Player = player });
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            CreateRequest(new PlayActionRequest { Player = player });
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            CreateRequest(new PlaceCardRequest { Player = player });
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CreateRequest(new CycleTargetRequest { Player = player, Direction = -1 });
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CreateRequest(new CycleTargetRequest { Player = player, Direction = 1 });
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            CreateRequest(new CycleCharacterRequest { Player = player, Direction = 1 });
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            CreateRequest(new CycleCharacterRequest { Player = player, Direction = -1 });
        }
    }

    private bool TryResolvePlayer()
    {
        if (player != Entity.Null && entityManager.Exists(player))
        {
            return true;
        }

        EntityQuery query = entityManager.CreateEntityQuery(typeof(PlayerTag), typeof(OwnedBattle));

        if (query.IsEmpty)
        {
            query.Dispose();
            return false;
        }

        player = query.GetSingletonEntity();

        query.Dispose();

        return true;
    }

    private void CreateRequest<T>(T request)
        where T : unmanaged, IComponentData
    {
        Entity entity = entityManager.CreateEntity();

        entityManager.AddComponentData(entity, request);

        entityManager.AddComponent<InputSystemTag>(entity);
    }
}
