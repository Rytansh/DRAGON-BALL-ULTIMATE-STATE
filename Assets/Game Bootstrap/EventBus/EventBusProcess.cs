using UnityEngine;

public class EventBusProcess : IBootstrapProcess
{
    public int Order => SimulationBootstrapOrder.EventBus;

    public void Initialise(WorldContext rootContext)
    {
        IEventBus eventBus = new DefaultEventBus();
        rootContext.Register<IEventBus>(eventBus);

        Logging.System("Event Bus initialised.");
    }
}
