using Unity.Entities;

namespace DBUS.Core.Components.Combat
{
    public struct CurrentATK : IComponentData
    {
        public int Value;
    }
    // holds the current attack of a character during combat.
}
