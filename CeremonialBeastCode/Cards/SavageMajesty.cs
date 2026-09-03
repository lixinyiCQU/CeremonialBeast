using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class SavageMajesty : CeremonialBeastCard
{
    // 注册 Ringing 标签
    protected override HashSet<CardTag> CanonicalTags => [CustomTags.Ringing];
    // 注册悬停提示：自动显示虚弱和易伤的词条解释
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<CeremonialBeast.CeremonialBeastCode.Powers.RingingPower>()
    };

    // 注册三个动态变量：伤害(18)、虚弱(2)、易伤(3)
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(20m, ValueProp.Move),
        new PowerVar<WeakPower>(2m),
        new PowerVar<VulnerablePower>(3m)
    };

    public SavageMajesty()
        : base(2, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 执行群体攻击
        // 使用 .TargetingAllOpponents 自动锁定场上所有敌人
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(base.CombatState!)
            .Execute(choiceContext);

        // 2. 施加群体虚弱
        // 使用 base.CombatState.HittableEnemies 确保只命中存活且可被选中的目标
        await PowerCmd.Apply<WeakPower>(
            base.CombatState!.HittableEnemies, 
            base.DynamicVars["WeakPower"].BaseValue, 
            base.Owner.Creature, 
            this
        );

        // 3. 施加群体易伤
        await PowerCmd.Apply<VulnerablePower>(
            base.CombatState.HittableEnemies, 
            base.DynamicVars["VulnerablePower"].BaseValue, 
            base.Owner.Creature, 
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(7m);
    }
}
