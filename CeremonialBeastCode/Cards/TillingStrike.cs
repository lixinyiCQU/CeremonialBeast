using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class TillingStrike : CeremonialBeastCard
{
    // 注册悬停提示：展示《仪式犁地》的卡牌预览
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard(ModelDb.Card<CeremonialPlowing>())
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
        // 这里的 base.IsUpgraded 会完美实现“升级后加入【犁地+】”的需求！
        await CeremonialPlowing.CreateInHand(base.Owner, base.CombatState!, base.IsUpgraded);

        // 3. 将自身放入抽牌堆
        await CardPileCmd.Add(this, PileType.Draw);
    }

    protected override void OnUpgrade()
    {
        // 升级后伤害提升 2 点 (6 -> 8)
        base.DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
