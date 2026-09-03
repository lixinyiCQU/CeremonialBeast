using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class BloodSacrifice : CeremonialBeastCard
{
    // 注册 消耗(Exhaust) 关键字
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new CardsVar(2),
        new EnergyVar(2)
    ];

    public BloodSacrifice()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardPile drawPile = PileType.Draw.GetPile(base.Owner);
        int selectionCount = int.Min(base.DynamicVars.Cards.IntValue, drawPile.Cards.Count);

        if (selectionCount > 0)
        {
            IEnumerable<CardModel> selectedCards = await CardSelectCmd.FromCombatPile(
                choiceContext,
                drawPile,
                base.Owner,
                new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, selectionCount));

            foreach (CardModel card in selectedCards)
            {
                await CardCmd.Exhaust(choiceContext, card);
            }
        }

        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1m);
        base.DynamicVars.Energy.UpgradeValueBy(1m);
    }
}
