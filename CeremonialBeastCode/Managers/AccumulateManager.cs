using System.Threading.Tasks;
using BaseLib.Abstracts;
using CeremonialBeast.CeremonialBeastCode.Enchantments;
using CeremonialBeast.CeremonialBeastCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CeremonialBeast.CeremonialBeastCode.Managers;

public sealed class AccumulateManager : CustomSingletonModel
{
    public AccumulateManager() : base(HookType.Combat)
    {
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Enchantment is not AccumulateEnchantment)
        {
            return;
        }

        await PowerCmd.Apply<PlowPower>(
            cardPlay.Card.Owner.Creature,
            1m,
            cardPlay.Card.Owner.Creature,
            null);
    }
}
