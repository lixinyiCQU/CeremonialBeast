using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class ClawFlurry() : CeremonialBeastCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<RingingPower>()
    ];

    // ✨ 直接贴上 Ringing 标签，利用我们之前在基类里写好的自动挂载逻辑
    protected override HashSet<CardTag> CanonicalTags => [CustomTags.Ringing];

    // 核心：使用官方的 4 变量结构来构建基于手牌类型的动态乘区
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(7m, ValueProp.Move), // 基础伤害 7
        new CalculationBaseVar(0m),        // 基础攻击次数 0（靠手牌数量往上加）
        new CalculationExtraVar(1m),       // 每有1张牌，乘数 +1
        // 动态计算：抓取当前拥有者的手牌堆，统计其中 Type 为 Attack 的卡牌数量
        new CalculatedVar("CalculatedHits").WithMultiplier((CardModel card, Creature? _) => 
            PileType.Hand.GetPile(card.Owner).Cards.Count(c => c.Type == CardType.Attack)
        )
    ];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (play.Target != null)
        {
            // 提取出当前动态计算好的攻击次数
            int hits = (int)((CalculatedVar)base.DynamicVars["CalculatedHits"]).Calculate(play.Target);
            if (!PileType.Hand.GetPile(base.Owner).Cards.Contains(this))
            {
                hits++;
            }

            // 执行多段连击指令
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .WithHitCount(hits)
            .FromCard(this, play)
                .Targeting(play.Target)
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后基础伤害提升 2 点 (7 -> 9)
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
