using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using CeremonialBeast.CeremonialBeastCode.Enchantments;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public sealed class AncientBlessingPower : CeremonialBeastPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await base.AfterPlayerTurnStart(choiceContext, player);

        if (player.Creature != base.Owner)
        {
            return;
        }

        if (!PileType.Hand.GetPile(player).Cards.Any(CanEnchant))
        {
            return;
        }

        Flash();

        var selectedCards = await CardSelectCmd.FromHand(
            choiceContext,
            player,
            new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1),
            CanEnchant,
            this);

        CardModel? card = selectedCards?.FirstOrDefault();
        if (card != null && CanEnchant(card))
        {
            CardCmd.Enchant<BlessingEnchantment>(card, 1m);
            CardCmd.Preview(card);
        }
    }

    private static bool CanEnchant(CardModel card)
    {
        return ModelDb.Enchantment<BlessingEnchantment>().CanEnchant(card);
    }
}
