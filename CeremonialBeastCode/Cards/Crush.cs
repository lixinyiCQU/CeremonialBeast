using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using CeremonialBeast.CeremonialBeastCode.Powers; // 确保引入 PlowPower

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class Crush() : CeremonialBeastCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) // 使用 AnyEnemy 指代单体敌人
{
    // 1. 定义动态变量：基础伤害 8，Plow层数 1
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(8, ValueProp.Move),
        new PowerVar<PlowPower>(2) // PowerVar 需要传入对应泛型类型和初始值
    ];

    // 2. 卡牌打出逻辑
    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 第一步：执行攻击指令 (流式指令，必须带 .Execute)
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target!) // 显式声明 Target 不为空
            .Execute(choiceContext);

        // 第二步：获得 Plow 状态 (直接指令，直接 await 即可)
        await PowerCmd.Apply<PlowPower>(
            Owner.Creature, 
            DynamicVars[nameof(PlowPower)].BaseValue, 
            Owner.Creature, 
            this
        );
    }

    // 3. 升级逻辑
    protected override void OnUpgrade()
    {
        // 伤害提升 1 (8 -> 9)
        DynamicVars.Damage.UpgradeValueBy(1m);
        
        // Plow 层数提升 1 (1 -> 2)
        DynamicVars[nameof(PlowPower)].UpgradeValueBy(1m);
    }
}
