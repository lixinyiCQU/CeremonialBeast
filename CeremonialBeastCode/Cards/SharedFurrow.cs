using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CeremonialBeast.CeremonialBeastCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class SharedFurrow() : CeremonialBeastCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PlowPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<PlowPower>(2m)];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        IEnumerable<Creature> teammates = from c in base.CombatState!.GetTeammatesOf(base.Owner.Creature)
            where c != null && c.IsAlive && c.IsPlayer
            select c;

        foreach (Creature teammate in teammates)
        {
            await PowerCmd.Apply<PlowPower>(
                teammate,
                base.DynamicVars[nameof(PlowPower)].BaseValue,
                base.Owner.Creature,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars[nameof(PlowPower)].UpgradeValueBy(1m);
    }
}
