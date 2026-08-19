using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CeremonialBeast.CeremonialBeastCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class RitualRelay() : CeremonialBeastCard(2, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
{
    private const string PlayCount = "PlayCount";

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override HashSet<CardTag> CanonicalTags => [CustomTags.Ringing];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<RingingPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar(PlayCount, 3m)];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, nameof(play.Target));

        decimal extraPlays = base.DynamicVars[PlayCount].BaseValue - 1m;
        await PowerCmd.Apply<RitualRelayPower>(
            play.Target,
            extraPlays,
            base.Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars[PlayCount].UpgradeValueBy(1m);
    }
}
