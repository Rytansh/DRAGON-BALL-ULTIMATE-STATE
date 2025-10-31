using UnityEngine;

public sealed class PresentationWorld
{
    public WorldContext localContext { get; private set; }
    private readonly WorldContext rootContext;

    public PresentationWorld(WorldContext rootContext)
    {
        this.rootContext = rootContext;
    }

    public void Initialise()
    {
        localContext = new WorldContext();

        IConfigProvider rootConfigProvider = rootContext.Resolve<IConfigProvider>();
        localContext.Register<IConfigProvider>(rootConfigProvider);

        Logging.System("PresentationWorld context created.");
    }

    public void Dispose()
    {
        // cleanup logic later
        Logging.System("PresentationWorld disposed.");
    }
}

