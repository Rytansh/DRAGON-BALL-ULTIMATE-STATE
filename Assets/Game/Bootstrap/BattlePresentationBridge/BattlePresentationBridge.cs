using System.Collections.Generic;
using Archeus.Battle.Buffers.Presentation;

namespace Archeus.Game.Bootstrap
{
    public sealed class BattlePresentationBridge
    {
        private readonly Queue<PresentationFact> pendingFacts = new();

        public int PendingCount => pendingFacts.Count;

        public void Publish(in PresentationFact fact)
        {
            // Copies the struct into bridge-owned storage.
            pendingFacts.Enqueue(fact);
        }

        public bool TryConsume(out PresentationFact fact)
        {
            if (pendingFacts.Count > 0)
            {
                fact = pendingFacts.Dequeue();
                return true;
            }

            fact = default;
            return false;
        }

        public void Clear()
        {
            pendingFacts.Clear();
        }
    }
}
