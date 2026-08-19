using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Patches.Content; 
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using CeremonialBeast.CeremonialBeastCode.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models; 

namespace CeremonialBeast.CeremonialBeastCode.Relics;

public sealed class CeremonialSoil : CeremonialBeastRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("CardsCount", 1m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromCardWithCardHoverTips<CeremonialPlowing>();

    // ✨ 在这里告诉引擎升级后的遗物是谁
    public override RelicModel? GetUpgradeReplacement()
    {
        return ModelDb.Relic<FortifiedCeremonialSoil>().ToMutable();
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player == base.Owner && combatState.RoundNumber <= 1)
        {
            Flash();
            CardModel cardToAdd = ModelDb.Card<CeremonialPlowing>().ToMutable();
            combatState.AddCard(cardToAdd, player);
            await CardPileCmd.Add(cardToAdd, PileType.Hand, CardPilePosition.Top, this);
        }
    }
}
