using UnityEngine;

public interface IRNGProvider
{
    public DeterministicRNG GetRNG(string key, ulong salt = 0);
}
