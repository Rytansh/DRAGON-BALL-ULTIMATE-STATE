using Unity.Entities;

namespace DBUS.Core.Components.Determinism
{
    public struct SpawnIndex: IComponentData
    {        
        public ulong Value;
    }
}