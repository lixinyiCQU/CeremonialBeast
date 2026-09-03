using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class BeastCry : CeremonialBeastCard
{
    // 注册自定义状态【昏眩】的悬停提示，方便玩家鼠标放上去查看说明
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StunMarkPower>()
    ];

    // 注册伤害(12) 和 昏眩层数(1) 两个动态变量
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12m, ValueProp.Move),
        new PowerVar<StunMarkPower>(1m)
    ];

    public BeastCry()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 对目标造成伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);

        // 2. 将【昏眩】状态施加给目标敌人 (cardPlay.Target)
        await PowerCmd.Apply<StunMarkPower>(cardPlay.Target!, base.DynamicVars["StunMarkPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级后伤害提升 4 点 (12 -> 16)
        base.DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
