using Unity.Entities;

[UpdateInGroup(typeof(BattleRootGroup))]
[UpdateAfter(typeof(BattleSetupGroup))]
public partial class BattleSimulationGroup : ComponentSystemGroup {}


[UpdateInGroup(typeof(BattleSimulationGroup))]
public partial class TurnFlowGroup : ComponentSystemGroup {}
