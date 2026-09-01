using Archeus.Content.Registries;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;

namespace Archeus.Game.Bootstrap
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct SimulationContentLoadSystem : ISystem
    {
        private bool loadRequested;
        private Entity gameConfigSceneEntity;

        public void OnUpdate(ref SystemState state)
        {
            if (!loadRequested)
            {
                var sceneGuid = SceneSystem.GetSceneGUID(
                    ref state,
                    "Assets/Other/Scenes/GameConfig.unity"
                );

                gameConfigSceneEntity = SceneSystem.LoadSceneAsync(state.WorldUnmanaged, sceneGuid);

                loadRequested = true;

                Debug.Log("[Simulation Content] GameConfig load requested.");

                return;
            }

            if (!SceneSystem.IsSceneLoaded(state.WorldUnmanaged, gameConfigSceneEntity))
            {
                return;
            }

            bool registryExists = SystemAPI.HasSingleton<ContentBlobRegistryComponent>();

            Debug.Log(
                $"[Simulation Content] GameConfig loaded. " + $"Registry found: {registryExists}"
            );

            state.Enabled = false;
        }
    }
}
