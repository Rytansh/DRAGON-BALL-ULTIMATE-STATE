using Unity.Entities;

namespace DBUS.Core.Components.Turns
{
    public enum TurnPhase : byte
    {
        Start,
        Playing,
        End
    }

    public struct TurnState : IComponentData
    {
        public TurnPhase Current;
    }

}