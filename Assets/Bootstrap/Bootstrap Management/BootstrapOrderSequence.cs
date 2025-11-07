using UnityEngine;

public static class BootstrapOrderSequence
{
    public const int Seeding = 0;
    public const int RNG = 10;
    public const int RNGTests = 11;
    public const int Config = 20;
    public const int SimulationWorld = 30;
    public const int PresentationWorld = 40;
    public const int EventBus = 50;
    public const int Logging = 60;
}
