using System;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public class GigantificationPower : CeremonialBeastPower
{
    public static readonly SpireField<Creature, decimal> BonusHpTracker = new(() => 0m);

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        await base.AfterCombatEnd(room);
        await RevertHp(base.Owner);
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        await base.AfterRemoved(oldOwner);
        await RevertHp(oldOwner);
    }

    private static async Task RevertHp(Creature creature)
    {
        decimal totalBonus = BonusHpTracker.Get(creature);
        if (totalBonus <= 0m)
        {
            return;
        }

        BonusHpTracker.Set(creature, 0m);

        decimal newMaxHp = Math.Max(1m, creature.MaxHp - totalBonus);
        decimal multiplier = creature.MaxHp / newMaxHp;
        decimal newCurrentHp = Math.Max(1m, Math.Ceiling(creature.CurrentHp / multiplier));

        await CreatureCmd.SetMaxHp(creature, newMaxHp);
        await CreatureCmd.SetCurrentHp(creature, Math.Min(newCurrentHp, newMaxHp));
    }
}
