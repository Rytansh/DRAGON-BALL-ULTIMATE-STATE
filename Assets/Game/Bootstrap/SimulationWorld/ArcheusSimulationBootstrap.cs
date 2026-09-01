using System;
using Archeus.Battle.Systems.Cards;
using Archeus.Battle.Systems.Setup;
using Archeus.Battle.Systems.Turnflow;
using Archeus.Content.Lookup;
using Unity.Entities;
using UnityEngine;

namespace Archeus.Game.Bootstrap
{
    public sealed class ArcheusSimulationBootstrap : ICustomBootstrap
    {
        public static World SimulationEcsWorld { get; private set; }

        public bool Initialize(string defaultWorldName)
        {
            SimulationEcsWorld = new World("Archeus Simulation World", WorldFlags.Simulation);

            // Unity ECS scene-streaming infrastructure
            var streamingSystems = DefaultWorldInitialization.GetAllSystems(
                WorldSystemFilterFlags.Streaming
            );

            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(
                SimulationEcsWorld,
                streamingSystems
            );

            // Our manually-owned systems
            Type[] archeusSimulationSystems =
            {
                // Bootstrap
                typeof(SimulationWorldProbeSystem),
                typeof(SimulationContentLoadSystem),
                // Content and assets
                typeof(ContentLookupSystem),
                // Battle heirarchy
                typeof(BattleRootGroup),
                typeof(BattleSetupGroup),
                typeof(BattleCreationGroup),
                typeof(BattleInitialisationGroup),
                typeof(BattleSpawningGroup),
                typeof(BattleSimulationGroup),
                typeof(TurnFlowGroup),
                typeof(TurnStartGroup),
                typeof(DrawingStageGroup),
                typeof(PlanningStageGroup),
                typeof(AttackingStageGroup),
                typeof(TurnEndGroup),
                typeof(VMSystemGroup),
                // Battle setup
                typeof(BattlePhaseTransitionSystem),
                typeof(BattleCreationSystem),
                typeof(BattleInitialisationSystem),
                typeof(BattleSpawnRequestSystem),
                typeof(CharacterSpawnSystem),
                typeof(BattleSpawnCompletionSystem),
                typeof(BattleStartSystem),
            };

            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(
                SimulationEcsWorld,
                archeusSimulationSystems
            );

            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(SimulationEcsWorld);

            return false;
        }
    }
}
