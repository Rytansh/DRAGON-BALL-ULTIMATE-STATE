using Unity.Entities;
using Archeus.Battle.Buffers.Events;
using Archeus.Battle.Buffers.VM;
using Archeus.Battle.Events.Context;
using Archeus.Battle.Components.Stats;
using Archeus.Content.Registries;
using Archeus.Content.Blobs;
using Archeus.Core.Debugging;

namespace Archeus.Battle.VM.Execution
{
    public static class BehaviourExecutor
    {
        public static void Execute(BehaviourExecutionRequest request, ref BattleEvent evt, ref BattleContext ctx, DynamicBuffer<BehaviourRuntimeState> stateBuffer)
        {
            ref ContentBlobRegistry registry = ref ctx.BattleRegistryReference.Value;

            ref BehaviourDefinitionBlob behaviour = ref registry.Behaviours[request.BehaviourIndex];
            ref BehaviourTriggerBlob trigger = ref behaviour.Triggers[request.TriggerIndex];
            int stateIndex = request.RegistrationIndex;

            int programIndex = trigger.VMProgramIndex;

            Logging.Info(
    LogCategory.Event,
    $"[BEHAVIOUR EXECUTE] " +
    $"BehaviourIndex={request.BehaviourIndex} | " +
    $"TriggerIndex={request.TriggerIndex} | " +
    $"ProgramIndex={trigger.VMProgramIndex} | " +
    $"Owner={request.Owner.Index} | " +
    $"Gen={request.EmissionContext.StructuralData.Generation}"
);

if (programIndex < 0 ||
    programIndex >= registry.AbilityPrograms.Length)
{
    Logging.Warn(
        LogCategory.Event,
        $"Invalid VM program index {programIndex}. " +
        $"AbilityPrograms.Length={registry.AbilityPrograms.Length}"
    );

    return;
}

            AbilityExecutionFrame frame = new AbilityExecutionFrame
            {
                ProgramIndex = programIndex,
                BehaviourOwner = request.Owner,
                Source = request.Source,
                Target = request.Target,
                InstructionPointer = 0
            };

            AbilityExecutionContext context = new AbilityExecutionContext
            {
                ChainedEventQueue = ctx.ChainedEventQueue,
                CharacterStatsLookup = ctx.StatsLookup,
                ContentRegistry = ctx.BattleRegistryReference,
                StateBuffer = stateBuffer,
                StateIndex = stateIndex,
                BattleParticipants = ctx.Participants,
                TeamLookup = ctx.TeamLookup,
                CurrentHealthLookup = ctx.HealthLookup,
                EmissionContext = request.EmissionContext
            };

            AbilityInterpreter.Execute(ref frame, ref context, ref evt);
        }
    }
}
