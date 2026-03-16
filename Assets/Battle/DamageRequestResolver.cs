using Unity.Entities;
using System;
using DBUS.Battle.Components.Combat;

namespace DBUS.Battle.Resolvers
{
    public static class DamageRequestResolver
    {
        public static void Resolve(ref BattleSimulationContext ctx, BattleEvent evt)
        {
            var attacker = evt.Source;
            var target = evt.Target;

            if (!ctx.StatsLookup.HasComponent(attacker) ||
                !ctx.StatsLookup.HasComponent(target))
                return;

            var attackerStats = ctx.StatsLookup[attacker];

            float multiplier = evt.Payload.Damage.AttackMultiplier;

            int damage = (int)(attackerStats.Attack * multiplier);

            var targetHP = ctx.HealthLookup.GetRefRW(target);

            float hpBefore = targetHP.ValueRO.Value;

            targetHP.ValueRW.Value -= damage;
            targetHP.ValueRW.Value = Math.Max(0, targetHP.ValueRW.Value);

            float hpAfter = targetHP.ValueRW.Value;

            Logging.System(
                $"DamageRequestResolver | {attacker.Index} → {target.Index} | " +
                $"DMG: {damage} | HP {hpBefore} → {hpAfter}"
            );

            ctx.ChainBuffer.Add(new ChainedBattleEvent
            {
                Event = new BattleEvent
                {
                    Type = BattleEventType.DamageResolved,
                    Source = attacker,
                    Target = target,
                    Payload = new EventPayload
                    {
                        Damage = new DamagePayload
                        {
                            FinalDamage = damage
                        }
                    }
                }
            });
        }
    }
}