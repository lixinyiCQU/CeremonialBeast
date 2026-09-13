using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using CeremonialBeast.CeremonialBeastCode.Powers;
using MegaCrit.Sts2.Core.Models;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class Trample() : CeremonialBeastCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PlowPower>()
    ];

    // 1. 声明底层专用的动态计算变量组合 (完美复刻 BodySlam 的逻辑)
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        // 基础伤害 (Base): 6
        new CalculationBaseVar(12m),
        
        // 额外伤害 (Extra/Bonus): 每层 Plow 造成 2 点
        new ExtraDamageVar(2m),     
        
        // 动态计算核心 (Calculated): 
        // 引擎底层公式：CalculatedDamage = CalculationBase + (ExtraDamage * Multiplier)
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((CardModel card, Creature? _) => 
                card.Owner?.Creature?.GetPowerAmount<PlowPower>() ?? 0)
    ];

    // 2. 卡牌打出逻辑
    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 直接将引擎已经计算好的 CalculatedDamage 喂给攻击指令
        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCard(this, play)
            .Targeting(play.Target!)
            .Execute(choiceContext);
    }

    // 3. 升级逻辑
    protected override void OnUpgrade()
    {
        // 升级只提升额外伤害的乘数 (2 -> 3)
        DynamicVars.ExtraDamage.UpgradeValueBy(1m);
    }
}
