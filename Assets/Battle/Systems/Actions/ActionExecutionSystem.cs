using Archeus.Battle.Buffers.Actions;
using Archeus.Battle.Buffers.Events;
using Archeus.Battle.Components.Core;
using Archeus.Battle.Components.Tags;
using Archeus.Battle.Data.Actions;
using Archeus.Battle.Data.Events;
using Archeus.Battle.Events.Context;
using Archeus.Battle.Events.Factory;
using Archeus.Battle.Systems.Events;
using Archeus.Core.Debugging;
using Unity.Entities;

namespace Archeus.Battle.Systems.Actions
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(BattleSimulationGroup))]
    [UpdateBefore(typeof(BattleEventProcessingSystem))]
    public partial struct ActionExecutionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (
                var (requests, actionStates, eventQueue, actionCounter, groupCounter) in SystemAPI
                    .Query<
                        DynamicBuffer<ActionExecutionRequest>,
                        DynamicBuffer<ActionExecutionState>,
                        DynamicBuffer<BattleEvent>,
                        RefRW<BattleActionExecutionCounter>,
                        RefRW<BattleEventGroupIDCounter>
                    >()
                    .WithAll<BattleTag>()
            )
            {
                if (requests.Length == 0)
                    continue;

                if (eventQueue.Length > 0)
                    continue;

                ActionExecutionRequest request = requests[0];

                requests.RemoveAt(0);

                if (!TryGetActionEventType(request.CharacterAction, out BattleEventType eventType))
                {
                    Logging.Warn(
                        LogCategory.Combat,
                        $"Unsupported action type: {request.CharacterAction}"
                    );

                    continue;
                }

                uint executionID = actionCounter.ValueRO.NextID;

                actionCounter.ValueRW.NextID++;

                actionStates.Add(
                    new ActionExecutionState
                    {
                        ActionExecutionID = executionID,
                        NextResultGroupIndex = 0,
                    }
                );

                BattleEvent actionEvent = new BattleEvent
                {
                    Type = eventType,
                    Scope = BattleEventScope.Targeted,

                    Source = request.Source,
                    Target = request.PrimaryTarget,

                    ActionData = new EventActionData
                    {
                        ActionExecutionID = executionID,

                        ActionType = request.CharacterAction,

                        ActionResultGroupIndex = EventActionData.NoActionResultGroup,
                    },
                };

                DynamicBuffer<BattleEvent> writableEventQueue = eventQueue;

                BattleEventEmitter.EmitOriginEvent(
                    actionEvent,
                    ref writableEventQueue,
                    groupCounter
                );

                Logging.Info(
                    LogCategory.Combat,
                    $"[ACTION] Started "
                        + $"Execution={executionID} | "
                        + $"Type={request.CharacterAction} | "
                        + $"Source={request.Source.Index}"
                );
            }
        }

        private static bool TryGetActionEventType(
            CharacterActionType actionType,
            out BattleEventType eventType
        )
        {
            switch (actionType)
            {
                case CharacterActionType.NormalAttack:
                    eventType = BattleEventType.TestEvent;
                    return true;

                // add more later

                default:
                    eventType = default;
                    return false;
            }
        }
    }
}
