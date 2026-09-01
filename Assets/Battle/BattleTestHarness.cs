using Archeus.Battle.Components.Requests;
using Archeus.Game.Bootstrap;
using Unity.Entities;
using UnityEngine;

public class BattleTestHarness : MonoBehaviour
{
    void Start()
    {
        var world = ArcheusSimulationBootstrap.SimulationEcsWorld.EntityManager;

        ISeedService seedService = RunBootstrap.RootContext.Resolve<ISeedService>();

        var e1 = world.CreateEntity();
        world.AddComponentData(
            e1,
            new StartBattleRequest
            {
                BattleID = 1,
                BattleSeed = 12345678,
                BattleConfigID = 0,
            }
        );
    }
}
