using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;
using CeremonialBeast.CeremonialBeastCode.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using BaseLib.Abstracts;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public sealed class CeremonialFormPower : CustomPowerModel
{
    public int UpgradedStacks;
    
    public override PowerType Type => PowerType.Buff;
    
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        await base.AfterSideTurnStart(side, participants, combatState);

        // 确保是自己的回合开始
        if (side == base.Owner.Side)
        {
            // 触发时图标闪烁
            Flash();

            // 1. 进入 Ringing 状态
            // 💡 修正：使用官方标准写法 base.Owner.Creature 作为目标和来源
            await PowerCmd.Apply<RingingPower>(base.Owner, 1m, base.Owner, null);

            // 2. 生成《横冲直撞》并加入手牌
            // 💡 极简优化：呼叫工厂方法！传入我们保存好的 isUpgraded 标记
            int upgradedCount = Math.Min(UpgradedStacks, (int)base.Amount);
            int normalCount = (int)base.Amount - upgradedCount;

            await Rampage.CreateInHand(base.Owner.Player!, normalCount, base.CombatState, false);
            await Rampage.CreateInHand(base.Owner.Player!, upgradedCount, base.CombatState, true);
        }
    }
}
