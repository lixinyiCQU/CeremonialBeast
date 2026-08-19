using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using CeremonialBeast.CeremonialBeastCode.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Entities.Powers;
using BaseLib.Abstracts;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public sealed class BreakingChainsPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    
    // 使用 Counter 类型，多打出几张就能每回合多塞几张 NOPE
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        await base.AfterSideTurnStart(side, participants, combatState);

        // 确保是玩家自己的回合开始
        if (side == base.Owner.Side)
        {
            // 图标闪烁提示生效
            Flash();

            // ✨ 极简优化：直接调用 NOPE 的批量生成工厂方法！
            // 传入的层数 (base.Amount) 决定了给几张，不需要自己写 for 循环了
            await Nope.CreateInHand(base.Owner.Player!, (int)base.Amount, base.CombatState, false);
        }
    }
}
