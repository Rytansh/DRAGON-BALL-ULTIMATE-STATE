using UnityEngine;
using Unity.Entities;
using DBUS.Battle.Components.Requests;

public class BattleTestHarness : MonoBehaviour
{
    void Start()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        var em = world.EntityManager;
   
        var e = em.CreateEntity();
        em.AddComponentData(e, new StartBattleRequest());

    }
}
