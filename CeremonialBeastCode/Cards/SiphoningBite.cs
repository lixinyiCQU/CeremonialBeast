using System.Collections.Generic;
using System.Linq; // ✨ 核心修复：引入 LINQ 用于查询
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries; // ✨ 核心修复：引入历史记录实体
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class SiphoningBite : CeremonialBeastCard
{
    public override bool GainsBlock => true;

    protected override HashSet<CardTag> CanonicalTags => [CustomTags.Ringing];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move)
    ];

    public SiphoningBite()
        : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 正常执行攻击指令，不需要强转返回值
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);

        // 2. 学以致用：利用历史记录系统查询刚刚造成的伤害结果！
        // 查找本回合 (RoundNumber)、由这张牌 (this) 造成的最新一条受击记录
        var damageEntry = CombatManager.Instance.History.Entries
            .OfType<DamageReceivedEntry>()
            .LastOrDefault(e => e.HappenedThisTurn(base.CombatState) && e.CardSource == this);

        // 3. 将未被格挡的伤害转化为格挡
        if (damageEntry != null && damageEntry.Result.UnblockedDamage > 0)
        {
            // 使用 ValueProp.Unpowered 确保不吃敏捷二次加成，实现等额吸取
            await CreatureCmd.GainBlock(base.Owner.Creature, damageEntry.Result.UnblockedDamage, ValueProp.Unpowered, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
