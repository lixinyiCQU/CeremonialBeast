using System.Collections.Generic;
using System.Linq; // ✨ 核心修复：引入 LINQ 用于查询
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries; // ✨ 核心修复：引入历史记录实体
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class ShieldTackle : CeremonialBeastCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<RingingPower>()
    ];

    public override bool GainsBlock => true;

    protected override HashSet<CardTag> CanonicalTags => [CustomTags.Ringing];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move)
    ];

    public ShieldTackle()
        : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int priorDamageEntryCount = CombatManager.Instance.History.Entries
            .OfType<DamageReceivedEntry>()
            .Count();

        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(base.CombatState!)
            .Execute(choiceContext);

        decimal totalUnblockedDamage = CombatManager.Instance.History.Entries
            .OfType<DamageReceivedEntry>()
            .Skip(priorDamageEntryCount)
            .Where(e => e.CardSource == this && e.Dealer == base.Owner.Creature)
            .Sum(e => e.Result.UnblockedDamage);

        if (totalUnblockedDamage > 0)
        {
            await CreatureCmd.GainBlock(base.Owner.Creature, totalUnblockedDamage, ValueProp.Unpowered, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
