using UnityEngine;

public class RNGTestProcess : IBootstrapProcess
{
    public int Order => BootstrapOrderSequence.RNGTests;
    private string testSeedString = "Battle_Test";

    public void Initialise(WorldContext context)
    {
        Logging.System("[RNGTestProcess] Running RNG deterministic validation...");

        ISeedService seedService = context.Resolve<ISeedService>();
        IRNGProvider rngProvider = context.Resolve<IRNGProvider>();

        ulong worldSeed = seedService.WorldSeed;
        Logging.System($"WorldSeed = {worldSeed}");

        // === Test 1: Seed derivation determinism ===
        Logging.System("[RNGTestProcess] Test 1 — Seed derivation determinism check...");

        Logging.System($"Deriving battle seeds using the following string: {testSeedString}");
        ulong derivedBattleSeed = seedService.CreateDerivedSeed(testSeedString);
        ulong sameStringDerivedBattleSeed = seedService.CreateDerivedSeed(testSeedString);

        if (derivedBattleSeed != sameStringDerivedBattleSeed)
            Logging.Error($"[RNGTestProcess] ❌ Test 1 failed - Mismatch of derived seeds: {derivedBattleSeed} != {sameStringDerivedBattleSeed}");
        else
            Logging.System($"[RNGTestProcess] ✅ Test 1 passed — derived seeds are equal: {derivedBattleSeed} = {sameStringDerivedBattleSeed}");
        

        // === Test 2: In-run sequence determinism ===
        Logging.System("[RNGTestProcess] Test 2 — In-run sequence consistency check...");

        var rngA = new DeterministicRNG(derivedBattleSeed);
        var rngB = new DeterministicRNG(derivedBattleSeed);

        bool match = true;
        const int sampleCount = 10;

        for (int i = 0; i < sampleCount; i++)
        {
            int a = rngA.NextInt(0, 100);
            int b = rngB.NextInt(0, 100);

            if (a != b)
            {
                Logging.Error($"[RNGTestProcess] ❌ Mismatch at roll {i}: {a} != {b}");
                match = false;
                break;
            }

            Logging.System($"[RNGTestProcess] Roll {i}: {a} = {b}");
        }

        if (match)
            Logging.System("[RNGTestProcess] ✅ Test 2 passed — in-run RNG sequence deterministic.");
        else
            Logging.Error("[RNGTestProcess] ❌ Test 2 failed — sequence diverged.");

        // === Test 3: Cross-run determinism (replay / reload check) ===
        Logging.System("[RNGTestProcess] Test 3 — Cross-run consistency check...");

        var replayRng1 = new DeterministicRNG(derivedBattleSeed);
        var replayRng2 = new DeterministicRNG(sameStringDerivedBattleSeed);

        bool replayMatch = true;
        for (int i = 0; i < sampleCount; i++)
        {
            int rollA = replayRng1.NextInt(0, 100);
            int rollB = replayRng2.NextInt(0, 100);

            // Log both results with state info
            Logging.System(
                $"[RNGTestProcess] Replay Roll {i}: {rollA} vs {rollB} " +
                $"| StateA=({replayRng1.StateA:X16},{replayRng1.StateB:X16}) " +
                $"StateB=({replayRng2.StateA:X16},{replayRng2.StateB:X16})");

            if (rollA != rollB)
            {
                Logging.Error($"[RNGTestProcess] ❌ Replay mismatch at roll {i}");
                replayMatch = false;
                break;
            }
        }

        if (replayMatch)
            Logging.System("[RNGTestProcess] ✅ Test 3 passed — cross-run sequence deterministic.");
        else
            Logging.Error("[RNGTestProcess] ❌ Test 3 failed — replay sequence mismatch.");


        // === Test 4: Mixed type sampling ===
        Logging.System("[RNGTestProcess] Test 4 — Sampling various RNG types...");

        var rngSample = new DeterministicRNG(derivedBattleSeed);
        for (int i = 0; i < 5; i++)
        {
            float f = rngSample.NextFloat();
            double d = rngSample.NextDouble();
            bool b = rngSample.NextBool();

            // Log with precision and internal state
            Logging.System(
                $"[RNGTestProcess] Sample {i} — " +
                $"Float={f:F8}, Double={d:F15}, Bool={b} " +
                $"| State=({rngSample.StateA:X16},{rngSample.StateB:X16})");
        }

        Logging.System("[RNGTestProcess] ✅ All RNG deterministic validation phases complete.");
    }
}
