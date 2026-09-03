using System.Collections.Generic;
using System.Threading.Tasks;
using CeremonialBeast.CeremonialBeastCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class EarthenShield : CeremonialBeastCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    public override bool GainsBlock => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PlowPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(0m),
        new CalculationExtraVar(3m),
        new CalculatedBlockVar(ValueProp.Move)
            .WithMultiplier((CardModel card, Creature? _) =>
                card.Owner?.Creature?.GetPowerAmount<PlowPower>() ?? 0m)
    ];

    public EarthenShield()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.Owner.Creature.GetPowerAmount<PlowPower>() <= 0m)
        {
            return;
        }

        await CreatureCmd.GainBlock(
            base.Owner.Creature,
            base.DynamicVars.CalculatedBlock.Calculate(base.Owner.Creature),
            base.DynamicVars.CalculatedBlock.Props,
            cardPlay);

        await PowerCmd.Remove<PlowPower>(base.Owner.Creature);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.CalculationExtra.UpgradeValueBy(1m);
    }
}
