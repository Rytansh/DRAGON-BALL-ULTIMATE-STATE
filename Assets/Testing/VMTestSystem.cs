using Unity.Entities;
using System;
using Unity.Collections;
using DBUS.Battle.Components.Events;
using DBUS.Battle.Components.Ownership;
using DBUS.Battle.Components.Combat;
using DBUS.Battle.VM.Data;
using DBUS.Battle.VM.Systems;

[UpdateInGroup(typeof(BattleSimulationGroup))]
public partial struct VMTestBootstrapSystem : ISystem
{
    private bool initialized;

    public void OnUpdate(ref SystemState state)
    {
        if (initialized)
            return;

        var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (eventBuffer, triggerBuffer, battle) 
                 in SystemAPI.Query<DynamicBuffer<BattleEvent>, DynamicBuffer<VMTrigger>>()
                 .WithAll<BattleTag>()
                 .WithEntityAccess())
        {
            // -----------------------------
            // Build VM program
            // -----------------------------

            var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<AbilityProgram>();

            var instructions = builder.Allocate(ref root.Instructions, 3);

            instructions[0] = new AbilityInstruction
            {
                Opcode = AbilityOpcode.PushConst,
                A = BitConverter.SingleToInt32Bits(1.5f)
            };

            instructions[1] = new AbilityInstruction
            {
                Opcode = AbilityOpcode.DealDamage
            };

            instructions[2] = new AbilityInstruction
            {
                Opcode = AbilityOpcode.End
            };

            var programBlob = builder.CreateBlobAssetReference<AbilityProgram>(Allocator.Persistent);
            builder.Dispose();

            const int TEST_BEHAVIOUR_ID = 999;

            AbilityProgramRegistry.Register(TEST_BEHAVIOUR_ID, programBlob);

            // -----------------------------
            // Register trigger
            // -----------------------------

            triggerBuffer.Add(new VMTrigger
            {
                EventType = BattleEventType.TestEvent,
                BehaviourID = TEST_BEHAVIOUR_ID,
                Priority = 0,
                Owner = battle,
                RegistrationIndex = 0
            });

            // -----------------------------
            // Spawn test characters safely
            // -----------------------------

            var attacker = ecb.CreateEntity();
            var defender = ecb.CreateEntity();

            ecb.AddComponent(attacker, new Character { Battle = battle });
            ecb.AddComponent(attacker, new CharacterStats { Attack = 10 });

            ecb.AddComponent(defender, new Character { Battle = battle });
            ecb.AddComponent(defender, new CharacterStats { MaxHealth = 100 });
            ecb.AddComponent(defender, new CurrentHealth { Value = 100 });

            ecb.AppendToBuffer(battle, new BattleEvent
            {
                Type = BattleEventType.TestEvent,
                Source = attacker,
                Target = defender
            });

            initialized = true;
            break;
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}