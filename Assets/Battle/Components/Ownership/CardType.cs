using Unity.Entities;

namespace DBUS.Battle.Components.Ownership
{
    public struct Character: IComponentData
    {
        public Entity Battle;
    }

    public struct Skill: IComponentData
    {
        public Entity Battle;
    }
}

