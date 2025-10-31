using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public sealed class BattleBootstrapOrchestrator
{
    private readonly List<IBootstrapProcess> processInitialisers = new();

    public void Register(IBootstrapProcess initialiser)
        => processInitialisers.Add(initialiser);

    public void InitialiseAll(WorldContext context)
    {
        List<IBootstrapProcess> ordered = processInitialisers.OrderBy(p => p.Order).ToList();
        Logging.System($"[Bootstrap] Starting initialisation of {ordered.Count} processes...");

        foreach (var process in ordered)
        {
            string processName = process.GetType().Name;
            var stopwatch = Stopwatch.StartNew();
            try
            {
                Logging.System($"[Bootstrap] → Initialising: {processName}...");
                process.Initialise(context);
                stopwatch.Stop();

                Logging.System($"[Bootstrap] ✓ {processName} initialised successfully ({stopwatch.ElapsedMilliseconds} ms).");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Logging.Error($"[Bootstrap] ✗ Failed to initialise {processName} ({stopwatch.ElapsedMilliseconds} ms). Exception: {ex.Message}");
                UnityEngine.Debug.Log(ex);

                // Optionally: decide whether to continue or abort bootstrapping
                // For now, continue — but in production, you might check for critical processes
            }
        }
        Logging.System("[Bootstrap] All processes attempted. Bootstrapping complete.");
    }
}

