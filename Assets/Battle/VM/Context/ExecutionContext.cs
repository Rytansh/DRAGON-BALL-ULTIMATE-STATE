using Unity.Entities;
using Archeus.Battle.Components.Stats;
using Archeus.Battle.Buffers.Events;
using Archeus.Content.Registries;
using Archeus.Battle.Events.Context;
using Archeus.Battle.Components.Ownership;
using Archeus.Battle.Buffers.Combat;

namespace Archeus.Battle.VM.Execution
{
    public struct AbilityExecutionContext
    {
        public DynamicBuffer<ChainedBattleEvent> ChainedEventQueue;
        public ComponentLookup<CharacterStats> CharacterStatsLookup;

        public DynamicBuffer<BattleParticipant> BattleParticipants;
        public ComponentLookup<Team> TeamLookup;
        public ComponentLookup<CurrentHealth> CurrentHealthLookup;

        public BlobAssetReference<ContentBlobRegistry> ContentRegistry;
        public DynamicBuffer<BehaviourRuntimeState> StateBuffer;
        public int StateIndex;

        public EventEmissionContext EmissionContext;
    }
}