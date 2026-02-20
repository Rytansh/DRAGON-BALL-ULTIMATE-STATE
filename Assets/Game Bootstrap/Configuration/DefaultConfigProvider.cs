using UnityEngine;

public class DefaultConfigProvider : IConfigProvider
{
    public void LoadConfigurations(WorldContext context)
    {
        //load assets/blobs later
        Logging.System("Configurations loaded successfully.");
    }
}
