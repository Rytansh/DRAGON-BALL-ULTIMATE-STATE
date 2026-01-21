using Unity.Entities;

namespace DBUS.Core.Components.Ownership
{
    public struct CardRuntimeID : IComponentData
    {
        public uint Value;
    }

    public struct CardDefinitionID : IComponentData
    {
        public uint Value;
    }
}