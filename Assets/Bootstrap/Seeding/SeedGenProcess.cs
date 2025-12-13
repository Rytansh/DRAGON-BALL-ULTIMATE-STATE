using UnityEngine;

public class SeedGenProcess : IBootstrapProcess
{
    public int Order => BootstrapOrderSequence.Seeding;

    public void Initialise(WorldContext context)
    {
        ISeedService seedService = new SeedService();
        context.Register<ISeedService>(seedService);
        Logging.System($"World seed generated: {seedService.WorldSeed}");
    }
}
