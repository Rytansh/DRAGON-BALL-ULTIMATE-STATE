using UnityEngine;

public interface ISeedProvider
{
    ulong WorldSeed { get; }
    ulong CreateDerivedSeed(ulong salt);
}
