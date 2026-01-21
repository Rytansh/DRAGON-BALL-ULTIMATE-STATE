using Unity.Entities;

namespace DBUS.Core.Components.Determinism
{
    public struct BattleRNG: IComponentData
    {
        public ulong StateA;
        public ulong StateB;
    }

}
