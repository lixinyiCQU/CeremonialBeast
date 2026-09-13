using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public sealed class RitualRelayPower : CeremonialBeastPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (card.Owner.Creature != base.Owner)
        {
            return playCount;
        }

        return playCount + (int)base.Amount;
    }

    public override async Task AfterModifyingCardPlayCount(CardModel card)
    {
        await base.AfterModifyingCardPlayCount(card);

        if (card.Owner.Creature != base.Owner)
        {
            return;
        }

        Flash();
        await PowerCmd.Apply<RingingPower>(base.Owner, 1m, base.Owner, card);
        await PowerCmd.Remove(this);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);

        if (side == base.Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
