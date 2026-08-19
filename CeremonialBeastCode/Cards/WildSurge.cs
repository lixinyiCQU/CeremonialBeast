using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using CeremonialBeast.CeremonialBeastCode.Powers; // 引入自定义的 PlowPower 和 WildSurgePower

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class WildSurge() : CeremonialBeastCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    // 悬停提示：展示“力量”和“Plow”的官方解释黑框
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<CeremonialBeast.CeremonialBeastCode.Powers.PlowPower>()
    ];

    // 声明变量：10点伤害。
    // 注意：因为获得的力量层数是动态的（等于 Plow 层数），不是固定值，所以这里不需要声明力量变量。
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(10m, ValueProp.Move)
    ];

    // 视觉反馈：如果有 Plow 状态，则发金光提示玩家可以打出爆发
    protected override bool ShouldGlowGoldInternal => (base.Owner?.Creature?.GetPower<CeremonialBeast.CeremonialBeastCode.Powers.PlowPower>()?.Amount ?? 0) > 0;

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (play.Target != null)
        {
            // 1. 结算基础物理伤害
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
                .Targeting(play.Target)
                .Execute(choiceContext);

            // 2. 动态获取当前的 Plow 层数
            int plowStacks = base.Owner.Creature.GetPower<CeremonialBeast.CeremonialBeastCode.Powers.PlowPower>()?.Amount ?? 0;
            
            // 3. 如果层数大于 0，则获得等量的临时力量
            if (plowStacks > 0)
            {
                await PowerCmd.Apply<WildSurgePower>(
                    base.Owner.Creature, 
                    plowStacks, 
                    base.Owner.Creature, 
                    this
                );
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后伤害提升 3 点 (10 -> 13)
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
