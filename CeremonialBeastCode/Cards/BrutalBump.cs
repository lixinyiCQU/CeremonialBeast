using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers; // 必须引入以获取 StrengthPower
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class BrutalBump() : CeremonialBeastCard(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    // 1. 定义动态变量
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(5, ValueProp.Move),
        new DamageVar("EnhancedDamage", 9m, ValueProp.Move)
    ];

    // 2. 卡牌打出逻辑
    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 提取基础伤害预备值
        decimal finalDamage = DynamicVars.Damage.BaseValue;

        // 检查玩家身上的力量 (StrengthPower) 层数
        int strengthAmount = Owner.Creature.GetPowerAmount<StrengthPower>();

        // 判定条件：力量大于 0 时，将额外伤害累加到基础伤害中
        if (strengthAmount > 0)
        {
            finalDamage = DynamicVars["EnhancedDamage"].BaseValue;
        }

        // 执行攻击指令
        // 游戏底层在处理 DamageCmd.Attack 时，会自动读取这名角色的力量并进行最终的乘法或加法计算
        await DamageCmd.Attack(finalDamage)
            .FromCard(this, play)
            .TargetingAllOpponents(base.CombatState!)
            .Execute(choiceContext);
    }

    protected override bool ShouldGlowGoldInternal => Owner?.Creature?.GetPowerAmount<StrengthPower>() > 0;

    // 3. 升级逻辑
    protected override void OnUpgrade()
    {
        // 基础伤害提升 2 (5 -> 7)
        DynamicVars.Damage.UpgradeValueBy(2m);
        
        DynamicVars["EnhancedDamage"].UpgradeValueBy(4m);
    }
}
