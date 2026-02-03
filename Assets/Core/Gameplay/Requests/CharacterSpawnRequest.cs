using Unity.Entities;

namespace DBUS.Core.Components.Requests
{
    public struct SpawnCharacterRequest : IComponentData
    {
        public int Slot;
        public int MaxHealth;
        public int Attack;
        public int Defense;
    }
}

