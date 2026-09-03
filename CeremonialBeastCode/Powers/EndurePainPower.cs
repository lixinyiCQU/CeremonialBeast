using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Powers;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public sealed class EndurePainPower : CeremonialBeastPower
{
    public override PowerType Type => PowerType.Buff;
    
    // 隐藏状态层数，因为每挨打一次都是固定给 1 层 Plow，不涉及自身效果的叠加
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 核心判断：
        // 1. 确保挨打的人确实是自己
        // 2. 确保伤害来源是正规的物理“攻击” (排除毒伤、自伤等)
        if (target == base.Owner && props.IsPoweredAttack())
        {
            // 💡 视觉优化：挨打触发时让状态图标闪烁，增加反馈感
            Flash();
            
            // 给自己施加 Plow 状态
            await PowerCmd.Apply<PlowPower>(base.Owner, base.Amount, base.Owner, null);
        }
        
        // 将所有参数原封不动地传递给基类
        await base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);
    }

    // ==========================================
    // ✨ 核心修复：清理时机改为“回合开始”
    // ==========================================
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        await base.AfterSideTurnStart(side, participants, combatState);

        // 确保是玩家的新回合开始了（这意味着怪物的回合已经结束）
        if (side == base.Owner.Side)
        {
            // 此时功成身退，安全移除该状态
            await PowerCmd.Remove(this);
        }
    }
}
