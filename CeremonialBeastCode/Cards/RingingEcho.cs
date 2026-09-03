using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils; // 必须引入以使用 SpireField
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using CeremonialBeast.CeremonialBeastCode.Powers;
using MegaCrit.Sts2.Core.Models; // 引入以读取 RingingCountThisCombat

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class RingingEcho() : CeremonialBeastCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<RingingPower>()
    ];

    // 核心：使用官方的 5 变量结构来构建动态计算乘区
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6m, ValueProp.Move), // 基础伤害 6
        new RepeatVar(1), 
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        // 动态计算：基础 1 次 + 本场战斗进入 Ringing 的次数
        new CalculatedVar("CalculatedHits").WithMultiplier((CardModel card, Creature? _) => 
            RingingPower.RingingCountThisCombat.Get(card.Owner.Creature.CombatState!)
        )
    ];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (play.Target != null)
        {
            // 提取出当前动态计算好的攻击次数
            int hits = (int)((CalculatedVar)base.DynamicVars["CalculatedHits"]).Calculate(play.Target);

            // 执行多段攻击指令
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .WithHitCount(hits)
            .FromCard(this, play)
                .Targeting(play.Target)
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后基础伤害提升 2 点 (6 -> 8)
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
