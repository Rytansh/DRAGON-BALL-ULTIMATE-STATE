using Unity.Entities;

namespace DBUS.Core.Components.Combat
{
    public struct MaxHP : IComponentData
    {
        public int Value;
    }
    // holds the max HP of a character at the beginning of combat.

    public struct CurrentHP : IComponentData
    {
        public int Value;
    }
    // holds the current HP of a character during combat.
}
