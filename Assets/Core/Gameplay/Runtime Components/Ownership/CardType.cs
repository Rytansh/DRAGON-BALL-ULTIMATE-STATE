using Unity.Entities;

namespace DBUS.Core.Components.Combat
{
    public struct Character: IComponentData
    {
        public Entity Battle;
    }
    // holds whether an entity is a character.

    public struct Skill: IComponentData{}
    // holds whether an entity is a skill.
}

