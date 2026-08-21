using UnityEngine;

namespace Archeus.Battle.Presentation.Commands.Facts
{
    public struct PresentationFactPayload
    {
        PresentationHitPayload Hit;
        PresentationDeathPayload Death;
    }

    public struct PresentationHitPayload
    {
        public float Damage;
        public bool IsCrit;
        public bool IsFatal;
    }

    public struct PresentationDeathPayload
    {
        public bool WillRevive;
    }
}
