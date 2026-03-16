using Unity.Entities;

namespace DBUS.Battle.VM.Data
{
    public struct VMTrigger : IBufferElementData
    {
        public BattleEventType EventType;
        public int Priority;
        public Entity Owner;
        public int BehaviourID;
        public int RegistrationIndex;
    }
}
