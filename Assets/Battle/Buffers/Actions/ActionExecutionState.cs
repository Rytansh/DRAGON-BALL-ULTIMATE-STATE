using Unity.Entities;

namespace Archeus.Battle.Buffers.Actions
{
    public struct ActionExecutionState : IBufferElementData
    {
        public uint ActionExecutionID;
        public ushort NextResultGroupIndex;
    }
}
