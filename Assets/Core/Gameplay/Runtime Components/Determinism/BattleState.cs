using Unity.Entities;

namespace DBUS.Core.Components.Determinism
{
    public struct BattleState: IComponentData
    {        
        public BattlePhase Current;
    }

    public enum BattlePhase : byte
    {
        Bootstrapping = 0,
        Running      = 1,
        Completed    = 2,
    }
}