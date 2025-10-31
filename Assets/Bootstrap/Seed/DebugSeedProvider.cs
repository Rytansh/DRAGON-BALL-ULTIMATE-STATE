using UnityEngine;
using System;

public sealed class DebugSeedProvider : ISeedProvider
{
    private readonly ulong _worldSeed;

    public DebugSeedProvider()
    {
        _worldSeed = (ulong)(DateTime.UtcNow.Ticks & 0xFFFFFFF);
    }

    public ulong WorldSeed => _worldSeed;

    public ulong CreateDerivedSeed(ulong salt)
    {
        unchecked
        {
            ulong hash = 14695981039346656037UL; // FNV offset basis
            hash ^= _worldSeed;
            hash *= 1099511628211UL; // FNV prime
            hash ^= salt;
            hash *= 1099511628211UL;
            return hash;
        }
    }
}

