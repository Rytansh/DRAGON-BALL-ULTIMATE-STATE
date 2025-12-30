using Unity.Entities;

namespace DBUS.Core.Components.Effects
{
    public enum DOTEffectType
    {
        Burn,
        Poison,
        Bleed,
        Curse,
        Shock,
        Wither,
        Chain
    }

    public struct DOTEffectInstance : IBufferElementData
    {
        public DOTEffectType Type;
        public int TurnsRemaining;
        public int Stacks;
        public float Multiplier;
    }
}