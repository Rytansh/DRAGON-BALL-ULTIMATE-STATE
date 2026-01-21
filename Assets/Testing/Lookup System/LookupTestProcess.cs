using UnityEngine;
using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct LookupTestProcess : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ContentLookupTables>();
        // state.Enabled = true; comment this whenever you want to disable the test process
    }

    public void OnUpdate(ref SystemState state)
    {
        ref var lookups = ref SystemAPI.GetSingletonRW<ContentLookupTables>().ValueRW;
        ref var registry = ref SystemAPI.GetSingleton<ContentBlobRegistryComponent>().BlobRegistryReference.Value;

        uint testCharacterId = StableHash32.HashFromString("C1"); 

        if (lookups.CharacterIDToIndex.TryGetValue(testCharacterId, out int index))
        {
            ref var character = ref registry.Characters[index];
            Logging.System($"[LookupValidation] Character found at index {index} with HP {character.CharacterBlobBaseStats.HP} and attack {character.CharacterBlobBaseStats.ATK}.");
        }
        else
        {
            Logging.Error($"[LookupValidation] Character NOT found!");
        }


        state.Enabled = false;
    }
}

