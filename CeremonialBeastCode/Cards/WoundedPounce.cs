using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class WoundedPounce() : CeremonialBeastCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // 定义动态变量：基础伤害 6
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];

    protected override bool ShouldGlowGoldInternal => (decimal)Owner.Creature.CurrentHp <= (decimal)Owner.Creature.MaxHp / 2m;

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (play.Target != null)
        {
            // 核心逻辑：动态计算攻击次数
            int hitCount = 1;
            
            // 经典的半血检测（与我们的狂暴姿态和顽强生命保持一致）
            if ((decimal)Owner.Creature.CurrentHp <= (decimal)Owner.Creature.MaxHp / 2m)
            {
                hitCount = 2; // 满足条件，攻击次数翻倍，等效于“再打出1次”
            }

            // 执行攻击指令
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .WithHitCount(hitCount) // 传入动态计算的攻击次数
            .FromCard(this, play)
                .Targeting(play.Target)
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后伤害提升 2 (6 -> 8)
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
