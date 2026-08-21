using Unity.Entities;
using Archeus.Battle.Buffers.Events;
using Archeus.Battle.Buffers.Combat;
using Archeus.Battle.Components.Stats;
using Archeus.Battle.Components.Ownership;
using Archeus.Content.Registries;
using Archeus.Battle.Components.Core;
using Archeus.Battle.Buffers.Presentation;

namespace Archeus.Battle.Events.Context
{
    public struct BattleContext
    {
        public Entity Battle;

        // Specific resources belonging to THIS battle
        public DynamicBuffer<ChainedBattleEvent> ChainBuffer;
        public DynamicBuffer<PresentationFact> PresentationFacts;
        public DynamicBuffer<BattleParticipant> Participants;

        // Entity-indexed access to runtime data
        public ComponentLookup<CharacterStats> StatsLookup;
        public ComponentLookup<CurrentHealth> HealthLookup;
        public ComponentLookup<Team> TeamLookup;

        public BufferLookup<ActiveEffect> EffectLookup;

        // Battle-specific data accessed through entity
        public ComponentLookup<BattleRNG> RNGLookup;

        public BlobAssetReference<ContentBlobRegistry> BattleRegistryReference;
    }
}