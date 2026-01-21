using Unity.Entities;

namespace DBUS.Core.Components.Determinism
{
    public struct BattleID: IComponentData
    {        
        public ulong Value;
    }
}