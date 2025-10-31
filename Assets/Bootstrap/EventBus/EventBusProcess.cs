using UnityEngine;

public class EventBusProcess : IBootstrapProcess
{
    public int Order => BootstrapOrderSequence.EventBus;

    public void Initialise(WorldContext rootContext)
    {
        IEventBus eventBus = new DefaultEventBus();
        rootContext.Register<IEventBus>(eventBus);

        Logging.System("Event Bus initialised.");
    }
}
