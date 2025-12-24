using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(ContentLookupSystem))]
public partial struct ContentLookupCleanupSystem : ISystem
{
    public void OnDestroy(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<ContentLookupTables>())
            return;

        ref var lookups =
            ref SystemAPI.GetSingletonRW<ContentLookupTables>().ValueRW;

        lookups.Dispose();

        Logging.System("[LookupCleanup] Content lookup tables disposed.");
    }
}


