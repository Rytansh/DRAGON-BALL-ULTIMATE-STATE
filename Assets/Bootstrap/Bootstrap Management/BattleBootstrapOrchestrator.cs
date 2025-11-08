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
        var bootstrapTime = Stopwatch.StartNew();
        foreach (var process in ordered)
        {
            string processName = process.GetType().Name;
            var processTimer = Stopwatch.StartNew();
            try
            {
                Logging.System($"[Bootstrap] → Initialising: {processName}...");
                process.Initialise(context);
                processTimer.Stop();

                Logging.System($"[Bootstrap] ✓ {processName} initialised successfully ({processTimer.ElapsedMilliseconds} ms).");
            }
            catch (Exception ex)
            {
                processTimer.Stop();
                Logging.Error($"[Bootstrap] ✗ Failed to initialise {processName} ({processTimer.ElapsedMilliseconds} ms). Exception: {ex.Message}");
                UnityEngine.Debug.Log(ex);

                // Optionally: decide whether to continue or abort bootstrapping
                // For now, continue — but in production, you might check for critical processes
            }
        }
        bootstrapTime.Stop();
        Logging.System($"[Bootstrap] All processes attempted. Bootstrapping complete ({bootstrapTime.ElapsedMilliseconds} ms).");
    }
}

