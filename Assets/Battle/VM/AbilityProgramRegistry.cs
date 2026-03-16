using Unity.Entities;
using System.Collections.Generic;
using DBUS.Battle.VM.Data;

namespace DBUS.Battle.VM.Systems
{
    public static class AbilityProgramRegistry
    {
        private static Dictionary<int, BlobAssetReference<AbilityProgram>> programs
            = new();

        public static void Register(int id, BlobAssetReference<AbilityProgram> program)
        {
            programs[id] = program;
        }

        public static BlobAssetReference<AbilityProgram> Get(int id)
        {
            return programs[id];
        }
    }
}