using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public sealed class DeterrencePower : CeremonialBeastPower
{
    public override PowerType Type => PowerType.Buff;
    
    public override PowerStackType StackType => PowerStackType.Counter;

    // ==========================================
    // ✨ 核心修复：将 AfterTurnEnd 改为 BeforeTurnEnd
    // 这样它会在玩家点击“结束回合”的瞬间最先触发判定！
    // ==========================================
    public override async Task BeforeSideTurnEndVeryEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.BeforeSideTurnEndVeryEarly(choiceContext, side, participants);

        // 确保是自己的回合即将结束
        if (side == base.Owner.Side)
        {
            // 1. 判定条件：此时（遗物生效前）玩家没有任何格挡
            if (base.Owner.Block == 0)
            {
                // 图标闪烁视觉反馈
                Flash();

                // 获取所有可以被选中的敌人
                var enemies = base.CombatState.HittableEnemies;
                
                foreach (var enemy in enemies)
                {
                    // 2. 削减力量：给敌人施加“负数”的 StrengthPower
                    await PowerCmd.Apply<StrengthPower>(enemy, -base.Amount, base.Owner, null);
                }
            }

            // 3. ✨ 核心清理：因为是技能牌赋予的一回合临时能力，结算完立刻销毁自身！
            await PowerCmd.Remove(this);
        }
    }
}
