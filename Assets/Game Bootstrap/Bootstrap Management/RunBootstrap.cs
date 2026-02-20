using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Entities;
using DBUS.Core.Components.GameState;

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
        
        DontDestroyOnLoad(gameObject);
    }

    public void Run()
    {
        Logging.System("[Bootstrap Runner] Starting BattleBootstrap...");
        bootstrapEntry = new BattleBootstrapEntry();

        try
        {
            bootstrapEntry.Initialise();

            World ecsWorld = World.DefaultGameObjectInjectionWorld;

            EntityManager em = ecsWorld.EntityManager;

            em.CreateEntity(typeof(GameBootstrapCompleteTag));
            Logging.System("[Bootstrap Runner] BattleBootstrap completed successfully!");
        }
        catch (System.Exception)
        {
            Logging.Error("[Bootstrap Runner] BattleBootstrap failed!");
        }
    }
}
