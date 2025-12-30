using Unity.Entities;

namespace DBUS.Core.Components.Turns
{
    public struct TurnCounter : IComponentData
    {
        public int CurrentTurn;
    }
    
}