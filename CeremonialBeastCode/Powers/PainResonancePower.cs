using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public sealed class PainResonancePower : CeremonialBeastPower
{
    public override PowerType Type => PowerType.Buff;
    
    // Counter 类型：允许多次打出时叠加每次反击的伤害额度
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 核心判定：
        // 1. 挨打的目标必须是自己 (target == base.Owner)
        // 2. 产生的未被格挡伤害必须大于 0，即真正的“失去生命” (result.UnblockedDamage > 0)
        // （去除了官方 Inferno 限制只能在自己回合触发的条件）
        if (target == base.Owner && result.UnblockedDamage > 0)
        {
            // 能力触发闪烁特效
            Flash();

            // 对所有敌人造成等同于层数 (Amount) 的间接伤害
            await CreatureCmd.Damage(choiceContext, base.CombatState.HittableEnemies, base.Amount, ValueProp.Unpowered, base.Owner, null, null);
        }

        // 继续执行底层的后续伤害处理逻辑
        await base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);
    }
}
