using Unity.Entities;

namespace DBUS.Core.Components.Turns
{
    public struct MaxActionPoints: IComponentData
    {
        public int Value;
    }
    // holds the maximum action points in a turn.

    public struct RemainingActionPoints: IComponentData
    {
        public int Value;
    }
    // holds the remaining action points for the turn.
}
