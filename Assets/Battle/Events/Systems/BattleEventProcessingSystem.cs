using Unity.Entities;
using Unity.Collections;
using DBUS.Battle.Components.Events;
using DBUS.Battle.Components.Combat;
using DBUS.Battle.Components.Ownership;
using DBUS.Battle.Components.Determinism;
using DBUS.Battle.VM.Data;
using DBUS.Battle.VM.Systems;
using DBUS.Battle.Resolvers;

[UpdateInGroup(typeof(BattleSimulationGroup))]
public partial struct BattleEventProcessingSystem : ISystem
{
    private const int MAX_EXECUTIONS = 10000;
    private ComponentLookup<CharacterStats> characterStatsLookup;
    private ComponentLookup<CurrentHealth> characterHPLookup;

    public void OnCreate(ref SystemState state)
    {
        characterStatsLookup = state.GetComponentLookup<CharacterStats>(true);
        characterHPLookup = state.GetComponentLookup<CurrentHealth>();
    }
    public void OnUpdate(ref SystemState state)
    {
        characterStatsLookup.Update(ref state);
        characterHPLookup.Update(ref state);

        foreach (var (mainEventQueue, chainedEventQueue, executionRequestQueue, vmTriggerQueue, battle) in SystemAPI.Query<DynamicBuffer<BattleEvent>, DynamicBuffer<ChainedBattleEvent>, DynamicBuffer<BehaviourExecutionRequest>, DynamicBuffer<VMTrigger>>().WithAll<BattleTag>().WithEntityAccess())
        {
            BattleSimulationContext ctx = new BattleSimulationContext
            {
                Battle = battle,
                ChainBuffer = chainedEventQueue,
                StatsLookup = characterStatsLookup,
                HealthLookup = characterHPLookup
            };
            int safetyCounter = 0;

            while (chainedEventQueue.Length > 0 || mainEventQueue.Length > 0 || executionRequestQueue.Length > 0)
            {
                if (++safetyCounter > MAX_EXECUTIONS)
                {
                    Logging.Warning($"[Battle] Fatal error - TOO MANY EVENT EXECUTIONS - clearing event queue for {battle}.");
                    mainEventQueue.Clear();
                    chainedEventQueue.Clear();
                    executionRequestQueue.Clear();
                    break;
                }

                while (chainedEventQueue.Length > 0)
                {
                    var evt = chainedEventQueue[0].Event;
                    chainedEventQueue.RemoveAt(0);

                    Dispatch(ref state, ctx, evt, vmTriggerQueue, executionRequestQueue);
                }

                while (mainEventQueue.Length > 0)
                {
                    var evt = mainEventQueue[0];
                    mainEventQueue.RemoveAt(0);

                    Dispatch(ref state, ctx, evt, vmTriggerQueue, executionRequestQueue);
                }

                while (executionRequestQueue.Length > 0)
                {
                    var request = executionRequestQueue[0];
                    executionRequestQueue.RemoveAt(0);

                    ExecuteBehaviour(ref state, battle, chainedEventQueue, request);
                }
            }
        }
    }


    private void Dispatch(ref SystemState state, BattleSimulationContext ctx, BattleEvent evt, DynamicBuffer<VMTrigger> vmTriggerQueue, DynamicBuffer<BehaviourExecutionRequest> executionRequestQueue)
    {
        var tempList = new NativeList<BehaviourExecutionRequest>(Allocator.Temp);

        ResolveEvent(ref state, ref ctx, evt);

        for (int i = 0; i < vmTriggerQueue.Length; i++)
        {
            var trigger = vmTriggerQueue[i];

            if (trigger.EventType != evt.Type)
                continue;

            tempList.Add(new BehaviourExecutionRequest
            {
                BehaviourID = trigger.BehaviourID,
                Owner = trigger.Owner,
                SourceEvent = evt,
                Priority = trigger.Priority,
                RegistrationIndex = trigger.RegistrationIndex
            });
        }

        tempList.Sort(new BehaviourExecutionComparer());

        for (int i = 0; i < tempList.Length; i++)
        {
            executionRequestQueue.Add(tempList[i]);
        }

        tempList.Dispose();
    }

    private void ExecuteBehaviour(ref SystemState state, Entity battle, DynamicBuffer<ChainedBattleEvent> chainedEventQueue, BehaviourExecutionRequest request)
    {
        LogExecution(ref state, battle, request);

        var program = AbilityProgramRegistry.Get(request.BehaviourID);

        AbilityExecutionFrame frame = new AbilityExecutionFrame
        {
            Program = program,
            Source = request.SourceEvent.Source,
            Target = request.SourceEvent.Target,
            InstructionPointer = 0
        };

        AbilityExecutionContext context = new AbilityExecutionContext
        {
            ChainedEventQueue = chainedEventQueue,
            CharacterStatsLookup = characterStatsLookup
        };

        AbilityInterpreter.Execute(ref frame, ref context);

        if (frame.Stack.Length > 0)
        {
            Logging.System($"VM result: {frame.Stack[frame.Stack.Length - 1]}");
        }
    }

    private void ResolveEvent(ref SystemState state, ref BattleSimulationContext ctx, BattleEvent evt)
    {
        LogExecution(ref state, ctx.Battle, evt);
        switch (evt.Type)
        {
            case BattleEventType.DamageRequested:
                DamageRequestResolver.Resolve(ref ctx, evt);
                break;

            // future resolvers
            // case BattleEventType.HealRequested:
            // case BattleEventType.ApplyBuff:
        }
    }

    private void LogExecution(ref SystemState state, Entity battle, BehaviourExecutionRequest request)
    {
        var counter = SystemAPI.GetComponentRW<BattleExecutionCounter>(battle);
        var rng = SystemAPI.GetComponent<BattleRNG>(battle);
        var logQueue = SystemAPI.GetBuffer<BattleExecutionLog>(battle);

        logQueue.Add(new BattleExecutionLog
        {
            StepIndex = counter.ValueRO.Value,
            EventType = request.SourceEvent.Type,
            BehaviourID = request.BehaviourID,
            RngStateA = rng.StateA,
            RngStateB = rng.StateB
        });

        counter.ValueRW.Value++;

        Logging.System($"[Battle {battle.Index}] Step {counter.ValueRO.Value} | " + $"Event:{request.SourceEvent.Type} | " + $"Behaviour:{request.BehaviourID} | " + $"RNG:({rng.StateA},{rng.StateB})");
    }
    private void LogExecution(ref SystemState state, Entity battle, BattleEvent evt)
    {
        var counter = SystemAPI.GetComponentRW<BattleExecutionCounter>(battle);
        var logQueue = SystemAPI.GetBuffer<BattleExecutionLog>(battle);

        logQueue.Add(new BattleExecutionLog
        {
            StepIndex = counter.ValueRO.Value,
            EventType = evt.Type,
            BehaviourID = 0,
            RngStateA = 0,
            RngStateB = 0
        });

        counter.ValueRW.Value++;

        Logging.System(
            $"[Battle {battle.Index}] Step {counter.ValueRO.Value} | " +
            $"Event:{evt.Type}"
        );
    }
}
