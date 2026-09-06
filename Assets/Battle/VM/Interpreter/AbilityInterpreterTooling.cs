using Archeus.Battle.Components.Ownership;
using Archeus.Battle.Data.Events;
using Archeus.Battle.Data.VM;
using Archeus.Battle.Events.Context;
using Archeus.Battle.Runtime;
using Archeus.Core.Debugging;
using Unity.Collections;
using Unity.Entities;

namespace Archeus.Battle.VM.Execution
{
    public static class AbilityInterpreterTooling
    {
        public static bool BeginsGameplayOperation(AbilityOpcode opcode)
        {
            return opcode switch
            {
                AbilityOpcode.DealDamage => true,
                AbilityOpcode.ApplyEffect => true,

                //future actions here

                _ => false,
            };
        }

        public static bool TryBeginGameplayOperation(
            AbilityOpcode opcode,
            ref AbilityExecutionFrame frame,
            ref AbilityExecutionContext context,
            out EventEmissionContext operationContext,
            out uint operationID
        )
        {
            operationContext = context.EmissionContext;
            operationID = 0;

            if (!BeginsGameplayOperation(opcode))
                return false;

            if (frame.Targets.Length == 0)
                return false;

            if (operationContext.ActionData.HasActionContext)
            {
                operationContext.ActionData.ActionResultGroupIndex =
                    ActionResultGroupAllocator.Allocate(
                        operationContext.ActionData.ActionExecutionID,
                        context.ActionExecutionStates
                    );
            }

            operationID = OperationIDAllocator.Allocate(context.OperationCounter);
            operationContext.ExecutionData.OperationID = operationID;

            return true;
        }

        public static void SelectTargets(
            ref AbilityExecutionFrame frame,
            ref AbilityExecutionContext context,
            TargetSelectionType type
        )
        {
            frame.Targets.Clear();

            switch (type)
            {
                case TargetSelectionType.PrimaryTarget:
                {
                    if (frame.Target != Entity.Null)
                        frame.Targets.Add(frame.Target);

                    break;
                }

                case TargetSelectionType.Self:
                {
                    if (frame.BehaviourOwner != Entity.Null)
                        frame.Targets.Add(frame.BehaviourOwner);

                    break;
                }

                case TargetSelectionType.AllEnemies:
                {
                    SelectTeamTargets(ref frame, ref context, selectSameTeam: false);

                    break;
                }

                case TargetSelectionType.AllAllies:
                {
                    SelectTeamTargets(ref frame, ref context, selectSameTeam: true);

                    break;
                }
            }
        }

        private static void SelectTeamTargets(
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

                if (!context.TeamLookup.HasComponent(candidate))
                    continue;

                if (!context.CurrentHealthLookup.HasComponent(candidate))
                    continue;

                if (context.CurrentHealthLookup[candidate].Value <= 0f)
                    continue;

                BattleSide candidateSide = context.TeamLookup[candidate].Side;

                if ((candidateSide == referenceSide) != selectSameTeam)
                    continue;

                if (frame.Targets.Length >= frame.Targets.Capacity)
                {
                    Logging.Warn(LogCategory.VM, "VM target collection exceeded capacity.");

                    return;
                }

                frame.Targets.Add(candidate);
            }
        }
    }
}
