using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class Vent() : CeremonialBeastCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    // 悬停提示：展示 Plow 的解释黑框
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PlowPower>()];

    // 声明变量：10点伤害，触发条件的 Plow 层数 (3层)，连击次数 (2次)
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(10m, ValueProp.Move),
        new DynamicVar("PlowCost", 3m),
        new RepeatVar(2)
    ];

    // 视觉反馈：如果身上的 Plow 大于等于 3 层，发金光提示玩家可以打出爆发
    protected override bool ShouldGlowGoldInternal => (base.Owner?.Creature?.GetPower<PlowPower>()?.Amount ?? 0) >= DynamicVars["PlowCost"].IntValue;

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (play.Target != null)
        {
            // 1. 获取当前 Plow 层数
            int plowStacks = base.Owner.Creature.GetPower<PlowPower>()?.Amount ?? 0;
            int cost = DynamicVars["PlowCost"].IntValue;
            
            // 默认只打 1 次
            int hitCount = 1;

            // 2. 如果满足触发条件
            if (plowStacks >= cost)
            {
                // 攻击次数变为 2 次（等效于再打出 1 次）
                hitCount = DynamicVars.Repeat.IntValue;

                // 失去对应层数的 Plow (传入负数)
                await PowerCmd.Apply<PlowPower>(
                    base.Owner.Creature, 
                    -DynamicVars["PlowCost"].BaseValue, 
                    base.Owner.Creature, 
                    this
                );
            }

            // 3. 执行攻击指令
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .WithHitCount(hitCount)
            .FromCard(this, play)
                .Targeting(play.Target)
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后伤害提升 2 点 (10 -> 12)
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
