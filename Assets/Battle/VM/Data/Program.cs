using Unity.Entities;

namespace DBUS.Battle.VM.Data
{
    public struct AbilityProgram
    {
        public BlobArray<AbilityInstruction> Instructions;
    }
}
