using Archeus.Battle.Presentation.Commands.Facts;
using Unity.Entities;


namespace Archeus.Battle.Buffers.Presentation
{
    public struct PresentationFact : IBufferElementData
    {
        public PresentationFactType Type;
        public PresentationFactMetadata Metadata;
        public PresentationFactPayload Payload;
    }
}

