using Unity.Entities;
using DBUS.Battle.Components.Combat;

public struct BattleSimulationContext
{
    public Entity Battle;
    public DynamicBuffer<ChainedBattleEvent> ChainBuffer;

    public ComponentLookup<CharacterStats> StatsLookup;
    public ComponentLookup<CurrentHealth> HealthLookup;
}