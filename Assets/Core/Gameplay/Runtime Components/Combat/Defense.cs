using Unity.Entities;

namespace DBUS.Core.Components.Combat
{
    public struct CurrentDEF: IComponentData
    {
        public int Value;
    }
    // holds the current defense of a character during combat.
}
