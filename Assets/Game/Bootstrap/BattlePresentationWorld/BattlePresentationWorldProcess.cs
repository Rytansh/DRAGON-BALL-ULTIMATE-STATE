using System;
using Archeus.Battle.Systems.Presentation;
using Archeus.Core.Debugging;
using Unity.Entities;

namespace Archeus.Game.Bootstrap
{
    public sealed class BattlePresentationWorldProcess : IBootstrapProcess
    {
        public int Order => PresentationBootstrapOrder.BattlePresentationWorld;

        public void Initialise(WorldContext rootContext)
        {
            World ecsWorld = new World("Battle Presentation", WorldFlags.Simulation);

            Type[] presentationSystems =
            {
                typeof(BattlePresentationProbeSystem),
                typeof(PresentationFactImportSystem),
            };

            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(
                ecsWorld,
                presentationSystems
            );

            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(ecsWorld);

            BattlePresentationWorld presentationWorld = new BattlePresentationWorld(ecsWorld);

            rootContext.Register(presentationWorld);

            // Create reference to the presentation bridge
            BattlePresentationBridge bridge = rootContext.Resolve<BattlePresentationBridge>();
            EntityManager entityManager = presentationWorld.EcsWorld.EntityManager;
            Entity bridgeEntity = entityManager.CreateEntity();
            entityManager.SetName(bridgeEntity, "Battle Presentation Bridge Reference");
            entityManager.AddComponentObject(
                bridgeEntity,
                new BattlePresentationBridgeReference { Bridge = bridge }
            );

            Logging.Info(
                LogCategory.Setup,
                $"Battle Presentation created using ECS World: {ecsWorld.Name}"
            );
        }
    }
}
