using Archeus.Battle.Buffers.Events;
using Archeus.Battle.Components.Ownership;
using Archeus.Battle.Data.Events;
using Archeus.Battle.Data.VM;
using Archeus.Battle.Events.Factory;
using Archeus.Battle.Events.Payloads;
using Archeus.Core.Debugging;
using Unity.Collections;
using Unity.Entities;

namespace Archeus.Battle.VM.Execution
{
    public static class AbilityInterpreter
    {
        private const int MAX_VM_STEPS = 256;

        public static void Execute(
            ref AbilityExecutionFrame frame,
            ref AbilityExecutionContext context,
            ref BattleEvent evt
        )
        {
            int safetyCounter = 0;
            ref var program = ref context.ContentRegistry.Value.AbilityPrograms[frame.ProgramIndex];
            FixedList512Bytes<Entity> targets = new FixedList512Bytes<Entity> { frame.Target };

            while (frame.InstructionPointer < program.Instructions.Length)
            {
                if (++safetyCounter > MAX_VM_STEPS)
                {
                    Logging.Info(LogCategory.VM, "VM exceeded maximum instruction count.");
                    return;
                }

                ref var instruction = ref program.Instructions[frame.InstructionPointer];

                switch (instruction.Opcode)
                {
                    // VALUE INTRODUCTION OPCODES //
                    case AbilityOpcode.PushConst:
                    {
                        float value = VMEncoding.DecodeFloat(instruction.A);
                        Push(ref frame, value);
                        break;
                    }
                    case AbilityOpcode.PushStat:
                    {
                        var stats = context.CharacterStatsLookup[frame.Source];
                        float value = instruction.A switch
                        {
                            0 => stats.Attack,
                            1 => stats.Defense,
                            2 => stats.MaxHealth,
                            _ => 0,
                        };
                        Push(ref frame, value);
                        break;
                    }
                    case AbilityOpcode.PushEventValue:
                    {
                        var type = (EventValueType)instruction.A;

                        float value = type switch
                        {
                            EventValueType.DamageBase => evt.Payload.Damage.BaseDamage,
                            EventValueType.DamageFinal => evt.Payload.Damage.FinalDamage,
                            EventValueType.DamageMultiplier => evt.Payload.Damage.AttackMultiplier,
                            _ => 0f,
                        };

                        Push(ref frame, value);
                        break;
                    }

                    case AbilityOpcode.LoadState:
                    {
                        int index = instruction.A;

                        var state = context.StateBuffer[context.StateIndex];

                        float value = (index < state.Memory.Length) ? state.Memory[index] : 0f;

                        Push(ref frame, value);
                        break;
                    }

                    case AbilityOpcode.StoreState:
                    {
                        int index = instruction.A;
                        float value = Pop(ref frame);

                        var state = context.StateBuffer[context.StateIndex];

                        while (state.Memory.Length <= index)
                        {
                            state.Memory.Add(0f);
                        }

                        state.Memory[index] = value;
                        context.StateBuffer[context.StateIndex] = state;

                        break;
                    }

                    case AbilityOpcode.ModifyEventValue:
                    {
                        var type = (EventValueType)instruction.A;

                        float value = Pop(ref frame);

                        switch (type)
                        {
                            case EventValueType.DamageFinal:
                                evt.Payload.Damage.FinalDamage = value;
                                break;

                            case EventValueType.DamageBase:
                                evt.Payload.Damage.BaseDamage = value;
                                break;

                            case EventValueType.DamageMultiplier:
                                evt.Payload.Damage.AttackMultiplier = value;
                                break;
                        }

                        break;
                    }

                    case AbilityOpcode.SelectTarget:
                    {
                        TargetSelectionType type = (TargetSelectionType)instruction.A;

                        SelectTargets(ref targets, ref frame, ref context, type);

                        break;
                    }

                    // MATH OPERATION OPCODES //
                    case AbilityOpcode.Add:
                    {
                        float b = Pop(ref frame);
                        float a = Pop(ref frame);

                        Push(ref frame, a + b);
                        break;
                    }

                    case AbilityOpcode.Sub:
                    {
                        float b = Pop(ref frame);
                        float a = Pop(ref frame);

                        Push(ref frame, a - b);
                        break;
                    }

                    case AbilityOpcode.Mul:
                    {
                        float b = Pop(ref frame);
                        float a = Pop(ref frame);

                        Push(ref frame, a * b);
                        break;
                    }

                    case AbilityOpcode.Div:
                    {
                        float b = Pop(ref frame);
                        float a = Pop(ref frame);

                        Push(ref frame, a / b);
                        break;
                    }

                    // COMPARISON OPCODES //
                    case AbilityOpcode.Equal:
                    {
                        float b = Pop(ref frame);
                        float a = Pop(ref frame);

                        Push(ref frame, a == b ? 1f : 0f);
                        break;
                    }

                    case AbilityOpcode.Greater:
                    {
                        float b = Pop(ref frame);
                        float a = Pop(ref frame);

                        Push(ref frame, a > b ? 1f : 0f);
                        break;
                    }

                    case AbilityOpcode.GreaterEqual:
                    {
                        float b = Pop(ref frame);
                        float a = Pop(ref frame);

                        float result = a >= b ? 1f : 0f;

                        Push(ref frame, result);
                        break;
                    }

                    case AbilityOpcode.Less:
                    {
                        float b = Pop(ref frame);
                        float a = Pop(ref frame);

                        Push(ref frame, a < b ? 1f : 0f);
                        break;
                    }

                    case AbilityOpcode.LessEqual:
                    {
                        float b = Pop(ref frame);
                        float a = Pop(ref frame);

                        Push(ref frame, a <= b ? 1f : 0f);
                        break;
                    }

                    // GAMEPLAY OPCODES //

                    case AbilityOpcode.DealDamage:
                    {
                        float multiplier = Pop(ref frame);

                        foreach (Entity target in targets)
                        {
                            BattleEventEmitter.EmitContinuationEvent(
                                new BattleEvent
                                {
                                    Type = BattleEventType.DamageRequested,
                                    Scope = BattleEventScope.Targeted,
                                    Source = frame.Source,
                                    Target = target,
                                    Payload = new EventPayload
                                    {
                                        Damage = new DamagePayload
                                        {
                                            AttackMultiplier = multiplier,
                                        },
                                    },
                                },
                                ref context.ChainedEventQueue,
                                in context.EmissionContext
                            );
                        }
                        break;
                    }

                    case AbilityOpcode.ApplyEffect:
                    {
                        int effectIndex = instruction.A;

                        bool isPermanent = false;

                        float strength = Pop(ref frame);
                        int duration = (int)Pop(ref frame);

                        if (duration == -1)
                        {
                            isPermanent = true;
                        }

                        foreach (Entity target in targets)
                        {
                            BattleEventEmitter.EmitContinuationEvent(
                                new BattleEvent
                                {
                                    Type = BattleEventType.EffectApplicationRequested,
                                    Scope = BattleEventScope.Targeted,
                                    Source = frame.Source,
                                    Target = target,
                                    Payload = new EventPayload
                                    {
                                        Effect = new EffectPayload
                                        {
                                            EffectIndex = effectIndex,
                                            Strength = strength,
                                            Duration = duration,
                                            IsPermanent = isPermanent,
                                        },
                                    },
                                },
                                ref context.ChainedEventQueue,
                                in context.EmissionContext
                            );
                        }
                        break;
                    }

                    // VM FLOW OPCODES //
                    case AbilityOpcode.Jump:
                    {
                        frame.InstructionPointer = instruction.A;
                        continue;
                    }

                    case AbilityOpcode.JumpIfFalse:
                    {
                        float cond = Pop(ref frame);
                        if (cond == 0f)
                        {
                            frame.InstructionPointer = instruction.A;
                            continue;
                        }
                        break;
                    }

                    case AbilityOpcode.JumpIfTrue:
                    {
                        float cond = Pop(ref frame);

                        if (cond != 0f)
                        {
                            frame.InstructionPointer = instruction.A;
                            continue;
                        }

                        break;
                    }

                    case AbilityOpcode.End:
                    {
                        return;
                    }

                    // DEFAULT
                    default:
                    {
                        Logging.Warn(
                            LogCategory.VM,
                            $"Unknown opcode {instruction.Opcode}. Cancelling execution."
                        );
                        return;
                    }
                }

                frame.InstructionPointer++;
            }
        }

        private static void Push(ref AbilityExecutionFrame frame, float value)
        {
            if (frame.Stack.Length >= 32)
            {
                Logging.Warn(LogCategory.VM, "VM stack overflow.");
                return;
            }
            frame.Stack.Add(value);
        }

        private static float Pop(ref AbilityExecutionFrame frame)
        {
            if (frame.Stack.Length == 0)
            {
                Logging.Warn(LogCategory.VM, "VM stack underflow.");
                return 0;
            }

            int last = frame.Stack.Length - 1;
            float value = frame.Stack[last];
            frame.Stack.RemoveAt(last);

            return value;
        }

        private static float Peek(ref AbilityExecutionFrame frame)
        {
            if (frame.Stack.Length == 0)
            {
                Logging.Warn(LogCategory.VM, "VM stack underflow.");
                return 0;
            }

            return frame.Stack[frame.Stack.Length - 1];
        }

        private static void SelectTargets(
            ref FixedList512Bytes<Entity> targets,
            ref AbilityExecutionFrame frame,
            ref AbilityExecutionContext context,
            TargetSelectionType type
        )
        {
            targets.Clear();

            switch (type)
            {
                case TargetSelectionType.PrimaryTarget:
                {
                    if (frame.Target != Entity.Null)
                        targets.Add(frame.Target);

                    break;
                }

                case TargetSelectionType.Self:
                {
                    if (frame.BehaviourOwner != Entity.Null)
                        targets.Add(frame.BehaviourOwner);

                    break;
                }

                case TargetSelectionType.AllEnemies:
                {
                    SelectTeamTargets(ref targets, ref frame, ref context, selectSameTeam: false);

                    break;
                }

                case TargetSelectionType.AllAllies:
                {
                    SelectTeamTargets(ref targets, ref frame, ref context, selectSameTeam: true);

                    break;
                }
            }
        }

        private static void SelectTeamTargets(
            ref FixedList512Bytes<Entity> targets,
            ref AbilityExecutionFrame frame,
            ref AbilityExecutionContext context,
            bool selectSameTeam
        )
        {
            Entity referenceEntity = frame.BehaviourOwner;

            if (!context.TeamLookup.HasComponent(referenceEntity))
                return;

            BattleSide referenceSide = context.TeamLookup[referenceEntity].Side;

            for (int i = 0; i < context.BattleParticipants.Length; i++)
            {
                Entity candidate = context.BattleParticipants[i].Participant;

                // Participant must currently have a Team.
                if (!context.TeamLookup.HasComponent(candidate))
                    continue;

                // For our current architecture, HP > 0 means alive/targetable.
                if (!context.CurrentHealthLookup.HasComponent(candidate))
                    continue;

                if (context.CurrentHealthLookup[candidate].Value <= 0f)
                    continue;

                BattleSide candidateSide = context.TeamLookup[candidate].Side;

                bool sameTeam = candidateSide == referenceSide;

                if (sameTeam != selectSameTeam)
                    continue;

                if (targets.Length >= targets.Capacity)
                {
                    Logging.Warn(LogCategory.VM, "VM target collection exceeded capacity.");

                    return;
                }

                targets.Add(candidate);
            }
        }
    }
}
