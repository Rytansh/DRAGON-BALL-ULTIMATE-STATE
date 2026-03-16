using Unity.Entities;

public struct ChainedBattleEvent : IBufferElementData
{
    public BattleEvent Event;
}
