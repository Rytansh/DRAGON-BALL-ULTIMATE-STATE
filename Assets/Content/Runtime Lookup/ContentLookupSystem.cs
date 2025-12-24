using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct ContentLookupSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ContentBlobRegistryComponent>();
    }

    public void OnUpdate(ref SystemState state)
    {
        if (SystemAPI.HasSingleton<ContentLookupTables>())
        {
            state.Enabled = false;
            return;
        }
        var registryRef = SystemAPI
            .GetSingleton<ContentBlobRegistryComponent>()
            .BlobRegistryReference;

        ref var registry = ref registryRef.Value;

        NativeHashMap<uint, int> characterMap = new NativeHashMap<uint, int>(
            registry.Characters.Length,
            Allocator.Persistent
        );

        for (int i = 0; i < registry.Characters.Length; i++)
        {
            if (!characterMap.TryAdd(registry.Characters[i].ID, i))
            {
                Logging.Error("Duplicate asset found. Not registered into lookup.");
            }
        }

        NativeHashMap<uint, int> skillMap = new NativeHashMap<uint, int>(
            registry.Skills.Length,
            Allocator.Persistent
        );

        for (int i = 0; i < registry.Skills.Length; i++)
        {
            if (!skillMap.TryAdd(registry.Skills[i].ID, i))
            {
                Logging.Error("Duplicate asset found. Not registered into lookup.");
            }
        }

        var lookupEntity = state.EntityManager.CreateEntity(typeof(ContentLookupTables));
        state.EntityManager.AddComponentData(
            lookupEntity,
            new ContentLookupTables
            {
                CharacterIDToIndex = characterMap,
                SkillIDToIndex = skillMap
            }
        );
        Logging.System($"[LookupSystem] Lookup system initialised successfully.");
        // One-shot system
        state.Enabled = false;
    }
}

