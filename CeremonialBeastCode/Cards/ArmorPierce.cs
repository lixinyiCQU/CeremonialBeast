using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers; // 引入官方状态库（易伤）
using MegaCrit.Sts2.Core.ValueProps;
using CeremonialBeast.CeremonialBeastCode.Powers; // 引入自定义状态库（Plow）

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class ArmorPierce() : CeremonialBeastCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // 声明变量：9点伤害，2层易伤
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(8m, ValueProp.Move),
        new PowerVar<VulnerablePower>(1m)
    ];

    // 悬停提示：展示“易伤”和“Plow”的官方解释黑框
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<CeremonialBeast.CeremonialBeastCode.Powers.PlowPower>()
    ];

    // 🌟 视觉体验优化：当满足触发条件时，卡牌发金光提示玩家
    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            // 安全获取玩家身上的 Plow 层数
            int plowStacks = base.Owner?.Creature?.GetPower<CeremonialBeast.CeremonialBeastCode.Powers.PlowPower>()?.Amount ?? 0;
            return plowStacks >= 3;
        }
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (play.Target != null)
        {
            // 1. 结算基础物理伤害
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
                .Targeting(play.Target)
                .Execute(choiceContext);

            // 2. 检测 Plow 层数是否 >= 3
            int plowStacks = base.Owner.Creature.GetPower<CeremonialBeast.CeremonialBeastCode.Powers.PlowPower>()?.Amount ?? 0;
            
            if (plowStacks >= 3)
            {
                // 满足条件，施加易伤
                await PowerCmd.Apply<VulnerablePower>(
                    play.Target, 
                    DynamicVars["VulnerablePower"].BaseValue, 
                    base.Owner.Creature, 
                    this
                );
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后伤害提升 3 (9 -> 12)
        DynamicVars.Damage.UpgradeValueBy(2m);
        // 升级后易伤层数提升 1 (2 -> 3)
        DynamicVars["VulnerablePower"].UpgradeValueBy(1m);
    }
}
