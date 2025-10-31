using UnityEngine;

public class RunBootstrap : MonoBehaviour
{
    private BattleBootstrapEntry bootstrapEntry;

    [Header("Bootstrap Settings")]
    [Tooltip("Run bootstrap on Awake automatically.")]
    public bool autoRun = true;

    private void Awake()
    {
        if (autoRun)
            Run();
    }

    public void Run()
    {
        Logging.System("[Bootstrap Runner] Starting BattleBootstrap...");
        bootstrapEntry = new BattleBootstrapEntry();

        try
        {
            bootstrapEntry.Initialise();
            Logging.System("[Bootstrap Runner] BattleBootstrap completed successfully!");
        }
        catch (System.Exception ex)
        {
            Logging.Error("[Bootstrap Runner] BattleBootstrap failed!");
        }
    }
}
