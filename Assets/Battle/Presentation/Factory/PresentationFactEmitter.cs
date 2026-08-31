using Archeus.Battle.Buffers.Events;
using Archeus.Battle.Buffers.Presentation;
using Archeus.Battle.Components.Core;
using Archeus.Battle.Components.Presentation;
using Archeus.Battle.Events.Context;
using Archeus.Battle.Presentation.Facts;
using Archeus.Core.Debugging;
using Unity.Entities;

namespace Archeus.Battle.Presentation.Factory
{
    public static class PresentationFactEmitter
    {
        public static void EmitDamageAppliedFact(
            PresentationHitPayload hitPayload,
            PresentationFactContext context,
            ref DynamicBuffer<PresentationFact> factQueue,
            RefRW<PresentationSequenceCounter> sequenceCounter
        )
        {
            PresentationFact DamageAppliedFact = new PresentationFact
            {
                FactType = PresentationFactType.DamageApplied,
                FactMetadata = ConstructFactMetadata(context, sequenceCounter),
                FactPayload = new PresentationFactPayload { HitPayload = hitPayload },
            };

            EmitFinalFact(DamageAppliedFact, ref factQueue);
        }

        private static void EmitFinalFact(
            PresentationFact fact,
            ref DynamicBuffer<PresentationFact> factQueue
        )
        {
            factQueue.Add(fact);
            LogFact(in fact);
        }

        private static PresentationFactMetadata ConstructFactMetadata(
            PresentationFactContext context,
            RefRW<PresentationSequenceCounter> sequenceCounter
        )
        {
            return new PresentationFactMetadata
            {
                BattleRuntimeID = context.BattleRuntimeID,

                SourceRuntimeID = context.SourceRuntimeID,
                TargetRuntimeID = context.TargetRuntimeID,

                Sequence = RetrieveNextSequence(sequenceCounter),

                ActionDefinitionID = context.ActionDefinitionID,
                ActionInstanceID = context.ActionInstanceID,
                ActionResultIndex = context.ActionResultIndex,

                GroupID = context.GroupID,
                Generation = context.Generation,
            };
        }

        private static uint RetrieveNextSequence(RefRW<PresentationSequenceCounter> sequenceCounter)
        {
            uint nextSequence = sequenceCounter.ValueRO.NextSequence;

            sequenceCounter.ValueRW.NextSequence++;

            return nextSequence;
        }

        private static void LogFact(in PresentationFact fact)
        {
            PresentationFactMetadata metadata = fact.FactMetadata;

            switch (fact.FactType)
            {
                case PresentationFactType.DamageApplied:
                {
                    PresentationHitPayload hit = fact.FactPayload.HitPayload;

                    string actionResult =
                        metadata.ActionResultIndex == PresentationFactMetadata.NoActionResult
                            ? "None"
                            : metadata.ActionResultIndex.ToString();

                    Logging.Info(
                        LogCategory.Presentation,
                        $"[PRESENTATION FACT] "
                            + $"Seq={metadata.Sequence} | "
                            + $"Type={fact.FactType} | "
                            + $"Battle={metadata.BattleRuntimeID} | "
                            + $"Source={metadata.SourceRuntimeID} | "
                            + $"Target={metadata.TargetRuntimeID} | "
                            + $"Group={metadata.GroupID} | "
                            + $"Gen={metadata.Generation} | "
                            + $"Action={metadata.ActionInstanceID} | "
                            + $"Result={actionResult} | "
                            + $"Damage={hit.Damage} | "
                            + $"Crit={hit.IsCrit}"
                    );

                    break;
                }

                default:
                {
                    Logging.Info(
                        LogCategory.Presentation,
                        $"[PRESENTATION FACT] "
                            + $"Seq={metadata.Sequence} | "
                            + $"Type={fact.FactType} | "
                            + $"Battle={metadata.BattleRuntimeID} | "
                            + $"Source={metadata.SourceRuntimeID} | "
                            + $"Target={metadata.TargetRuntimeID} | "
                            + $"Group={metadata.GroupID} | "
                            + $"Gen={metadata.Generation}"
                    );

                    break;
                }
            }
        }
    }
}
