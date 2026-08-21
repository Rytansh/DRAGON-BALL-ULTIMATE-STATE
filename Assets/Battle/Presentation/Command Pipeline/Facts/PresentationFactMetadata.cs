using Unity.Entities;

namespace Archeus.Battle.Presentation.Commands.Facts
{
    public struct PresentationFactMetadata
    {
        public uint OriginID;
        public Entity Source;
        public Entity Target;
    }
}
