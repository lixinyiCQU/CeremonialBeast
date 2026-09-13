using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class TillingStrike : CeremonialBeastCard
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    // 注册悬停提示：展示《仪式犁地》的卡牌预览
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<SacrificialStone>()
    ];

    // 注册伤害变量，初始为 6
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move)
    ];

    public TillingStrike()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 执行攻击指令
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);

        // 2. 生成《仪式犁地》并加入手牌
        // 💡 修复：第一参数需要传入 Player 类型，所以使用 base.Owner.Player
        // Tilling Strike+ still creates the unupgraded token.
        await SacrificialStone.CreateInHand(base.Owner, base.CombatState!);

    }

    protected override void OnUpgrade()
    {
        // 升级后伤害提升 1 点 (6 -> 7)
        base.DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
