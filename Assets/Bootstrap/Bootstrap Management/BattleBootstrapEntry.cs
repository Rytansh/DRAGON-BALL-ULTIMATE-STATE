using UnityEngine;

public sealed class BattleBootstrapEntry
{
    private readonly BattleBootstrapOrchestrator orchestrator;
    private readonly WorldContext rootContext;

    public BattleBootstrapEntry()
    {
        orchestrator = new BattleBootstrapOrchestrator();
        rootContext = new WorldContext();
    }

    public void Initialise()
    {
        Logging.System("=== Battle Bootstrap Started ===");

        // Register all processes (in any order)
        orchestrator.Register(new LoggingProcess());
        orchestrator.Register(new ConfigProcess());
        orchestrator.Register(new SeedGenProcess());
        orchestrator.Register(new RNGDeterminationProcess());
        orchestrator.Register(new SimulationWorldProcess());
        orchestrator.Register(new PresentationWorldProcess());
        orchestrator.Register(new EventBusProcess());

        // Initialise everything in order (using BootstrapOrderSequence)
        orchestrator.InitialiseAll(rootContext);

        Logging.System("=== Battle Bootstrap Completed ===");
    }
}

