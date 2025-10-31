using UnityEngine;

public class LoggingProcess : IBootstrapProcess
{
    public int Order => BootstrapOrderSequence.Logging;

    public void Initialise(WorldContext context)
    {
        // Register a logger service (in HSR, this later forwards to internal tools)
        ILoggingService logger = new DefaultLoggingService();
        context.Register<ILoggingService>(logger);

        Logging.System("Logging service registered to root context.");
    }
}

