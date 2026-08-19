using System.Collections.Generic;
using System.Threading.Tasks;
using CeremonialBeast.CeremonialBeastCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class ToughRoots : CeremonialBeastCard
{
    protected override HashSet<CardTag> CanonicalTags => [CustomTags.Ringing];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<RingingPower>(),
        HoverTipFactory.FromPower<ToughRootsPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    public ToughRoots()
        : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ToughRootsPower? power = await PowerCmd.Apply<ToughRootsPower>(
            base.Owner.Creature,
            base.DynamicVars.Cards.BaseValue,
            base.Owner.Creature,
            this);

        if (power != null)
        {
            power.EnergyNextTurn += 1m;
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
