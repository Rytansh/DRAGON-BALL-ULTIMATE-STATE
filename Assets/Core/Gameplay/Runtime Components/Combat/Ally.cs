using Unity.Entities;

namespace DBUS.Core.Components.Combat
{
    public struct Ally: IComponentData{}
    // holds whether an entity is an ally.

    public struct Enemy: IComponentData{}
    // holds whether an entity is an enemy.
}
