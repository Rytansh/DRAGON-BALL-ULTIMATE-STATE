using UnityEngine;

public class DefaultRNGProvider : IRNGProvider
{
    public DeterministicRNG rng { get; private set; }

    public DefaultRNGProvider(ulong seed) => rng = new DeterministicRNG(seed);
}
