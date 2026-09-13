using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public sealed class QuietudePower : CeremonialBeastPower
{
    public override PowerType Type => PowerType.Buff;
    
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);

        // 确保是状态拥有者的回合结束
        if (side == base.Owner.Side)
        {
            // 1. 获取当前状态拥有者对应的“玩家”实体
            var player = base.CombatState.Players.FirstOrDefault(p => p.Creature == base.Owner);

            if (player != null)
            {
                // 2. ✨ 核心修改：完美套用官方卡牌的能量获取 API！
                int remainingEnergy = player.PlayerCombatState!.Energy;

                // 3. 计算总回复量：剩余费用 * 基础回复量(3)
                int healAmount = remainingEnergy * (int)base.Amount;

                if (healAmount > 0)
                {
                    // 触发时图标闪烁
                    Flash();

                    // 执行回复生命值指令 (注意：在 Power 中，base.Owner 本身就是 Creature 类型)
                    await CreatureCmd.Heal(base.Owner, healAmount);
                }
            }

            // 4. 功成身退：结算完立刻销毁自身
            await PowerCmd.Remove(this);
        }
    }
}
