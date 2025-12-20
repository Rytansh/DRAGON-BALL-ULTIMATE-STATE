using UnityEngine;

public class PresentationWorldProcess : IBootstrapProcess
{
    public int Order => PresentationBootstrapOrder.PresentationWorld;

    public void Initialise(WorldContext rootContext)
    {
        PresentationWorld presentationWorld = new PresentationWorld(rootContext);
        presentationWorld.Initialise();

        rootContext.Register<PresentationWorld>(presentationWorld);

        Logging.System("Presentation world initialised.");
    }
}

