using UnityEngine;
using System.Runtime.CompilerServices;
using System;

public struct DeterministicRNG
{
    private ulong state;
    public DeterministicRNG(ulong WorldSeed) => state = WorldSeed;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int NextInt(int min, int max)
    {
        if (min >= max)
            throw new ArgumentException("min must be less than max");

        state = NextState(state);
        uint range = (uint)(max - min);
        return min + (int)(state % range);
    }

    private static ulong NextState(ulong s) => s * 6364136223846793005UL + 1;

}
