using Unity.Entities;
using Archeus.Battle.Buffers.Events;
using Archeus.Battle.Components.Stats;
using Archeus.Content.Registries;
using Archeus.Battle.Components.Core;
using Archeus.Battle.Buffers.Presentation;

namespace Archeus.Battle.Events.Context
{
    public struct BattleContext
    {
        public Entity Battle;
        public DynamicBuffer<ChainedBattleEvent> ChainBuffer;
        public DynamicBuffer<PresentationFact> PresentationFacts;

        public ComponentLookup<CharacterStats> StatsLookup;
        public ComponentLookup<CurrentHealth> HealthLookup;
        public ComponentLookup<BattleRNG> RNGLookup;
        
        public BufferLookup<ActiveEffect> EffectLookup;
        public BlobAssetReference<ContentBlobRegistry> BattleRegistryReference;
    }
}