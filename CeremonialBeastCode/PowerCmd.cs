using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CeremonialBeast.CeremonialBeastCode;

public static class PowerCmd
{
    public static Task<IReadOnlyList<T>> Apply<T>(
        IEnumerable<Creature> targets,
        decimal amount,
        Creature? applier,
        CardModel? cardSource,
        bool silent = false) where T : PowerModel
    {
        return MegaCrit.Sts2.Core.Commands.PowerCmd.Apply<T>(
            new ThrowingPlayerChoiceContext(),
            targets,
            amount,
            applier,
            cardSource,
            silent);
    }

    public static Task<T?> Apply<T>(
        Creature target,
        decimal amount,
        Creature? applier,
        CardModel? cardSource,
        bool silent = false) where T : PowerModel
    {
        return MegaCrit.Sts2.Core.Commands.PowerCmd.Apply<T>(
            new ThrowingPlayerChoiceContext(),
            target,
            amount,
            applier,
            cardSource,
            silent);
    }

    public static Task<T?> Apply<T>(
        Creature target,
        decimal amount,
        Creature? applier) where T : PowerModel
    {
        return Apply<T>(target, amount, applier, null);
    }

    public static Task Apply(
        PowerModel power,
        Creature target,
        decimal amount,
        Creature? applier,
        CardModel? cardSource,
        bool silent = false)
    {
        return MegaCrit.Sts2.Core.Commands.PowerCmd.Apply(
            new ThrowingPlayerChoiceContext(),
            power,
            target,
            amount,
            applier,
            cardSource,
            silent);
    }

    public static Task Decrement(PowerModel power)
    {
        return MegaCrit.Sts2.Core.Commands.PowerCmd.Decrement(power);
    }

    public static Task Remove<T>(Creature creature) where T : PowerModel
    {
        return MegaCrit.Sts2.Core.Commands.PowerCmd.Remove<T>(creature);
    }

    public static Task Remove(PowerModel? power)
    {
        return MegaCrit.Sts2.Core.Commands.PowerCmd.Remove(power);
    }
}
