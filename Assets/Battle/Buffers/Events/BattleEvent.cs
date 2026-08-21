using Unity.Entities;
using Archeus.Battle.Events.Payloads;
using Archeus.Battle.Data.Events;
using Archeus.Battle.Events.Context;

namespace Archeus.Battle.Buffers.Events
{
    public struct BattleEvent : IBufferElementData
    {
        public BattleEventType Type;
        public BattleEventScope Scope;

        public Entity Source;
        public Entity Target;
        public EventPayload Payload;
        public EventStructuralData StructuralData;
    }

    public enum BattleEventScope : byte
    {
        Targeted,
        Global
    }
}
