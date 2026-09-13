using System.Linq;
using System.Threading.Tasks;
using CeremonialBeast.CeremonialBeastCode.Enchantments;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace CeremonialBeast.CeremonialBeastCode.Relics;

public class CenserOfEnlightenment : CeremonialBeastRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner || player.Creature?.CombatState?.RoundNumber != 1)
        {
            return Task.CompletedTask;
        }

        var validCards = PileType.Draw.GetPile(base.Owner).Cards
            .Where(CanEnchant)
            .ToList();

        if (validCards.Count == 0)
        {
            return Task.CompletedTask;
        }

        Flash();

        validCards.StableShuffle(base.Owner.RunState.Rng.Shuffle);
        var targets = validCards.Take(2).ToList();

        foreach (CardModel card in targets)
        {
            var inspire = (EnchantmentModel)ModelDb.Enchantment<InspireEnchantment>().ToMutable();
            card.EnchantInternal(inspire, 1m);
        }

        return Task.CompletedTask;
    }

    private static bool CanEnchant(CardModel card)
    {
        return ModelDb.Enchantment<InspireEnchantment>().CanEnchant(card);
    }
}
