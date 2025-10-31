using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public sealed class BattleBootstrapOrchestrator
{
    private readonly List<IBootstrapProcess> _processInitialisers = new();

    public void Register(IBootstrapProcess initialiser)
        => _processInitialisers.Add(initialiser);

    public void InitialiseAll(WorldContext context)
    {
        _processInitialisers
            .OrderBy(i => i.Order)
            .ToList()
            .ForEach(i => {
                Logging.System($"[Bootstrap] process initialising: {i.GetType().Name}...");
                i.Initialise(context);
            });
    }
}
