using System.Threading.Tasks;
using BaseLib.Abstracts;
using CeremonialBeast.CeremonialBeastCode.Enchantments;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CeremonialBeast.CeremonialBeastCode.Managers;

public sealed class InspireManager : CustomSingletonModel
{
    public InspireManager() : base(HookType.Combat)
    {
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Enchantment is not InspireEnchantment inspire || !inspire.TryConsume())
        {
            return;
        }

        await CardPileCmd.Draw(choiceContext, 1, cardPlay.Card.Owner);
        await PlayerCmd.GainEnergy(1m, cardPlay.Card.Owner);
    }
}
