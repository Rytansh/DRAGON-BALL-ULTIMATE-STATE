using Unity.Entities;
using Unity.Collections;
using DBUS.Battle.Components.Combat;
using DBUS.Battle.Components.Requests;
using DBUS.Battle.Components.Ownership;

[UpdateInGroup(typeof(BattleSetupGroup))]

public partial struct CharacterSpawnSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb =
            new EntityCommandBuffer(Allocator.Temp);

        foreach (var (request, requestEntity)
                 in SystemAPI.Query<RefRO<SpawnCharacterRequest>>()
                             .WithEntityAccess())
        {
            Entity character = ecb.CreateEntity();

            ecb.AddComponent(character, new Character{Battle = request.ValueRO.Battle});
            ecb.AddComponent(character, new CharacterSlot{Value = request.ValueRO.Slot});
            ecb.AddComponent(character, new CharacterStats
            {
                Attack = request.ValueRO.Attack,
                Defense = request.ValueRO.Defense,
                MaxHealth = request.ValueRO.MaxHealth
            });
            ecb.AddComponent(character, new CurrentHealth{Value = request.ValueRO.MaxHealth});

            ecb.DestroyEntity(requestEntity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

