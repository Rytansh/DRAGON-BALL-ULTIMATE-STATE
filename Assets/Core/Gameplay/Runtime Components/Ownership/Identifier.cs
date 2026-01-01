using Unity.Entities;

namespace DBUS.Core.Components.Ownership
{
    public struct Identifier : IComponentData
    {
        public uint Value;
    }
}