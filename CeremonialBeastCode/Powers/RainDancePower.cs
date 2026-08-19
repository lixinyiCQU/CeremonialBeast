using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Entities.Multiplayer; // 确保引入 Player 所在的命名空间
using MegaCrit.Sts2.Core.ValueProps; // 用于 CombatSide
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Combat;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public sealed class RainDancePower : CustomPowerModel
{
    // 修复 CS0534: 显式声明这是一个正面增益 (Buff)
    public override PowerType Type => PowerType.Buff;

    // 正常显示层数（即 3 或 4）
    public override PowerStackType StackType => PowerStackType.Counter;

    // 状态机标记：记录是否已经给过属性了，防止逻辑死锁或跨回合白嫖
    private bool _hasTriggered = false;

    // 修复 CS0115: 补全 Player player 参数，并加上 async
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        // 只有当状态还没触发过时才给属性（严格保证只触发一次）
        if (!_hasTriggered)
        {
            // 获得真实的属性层数
            await PowerCmd.Apply<StrengthPower>(base.Owner, base.Amount, base.Owner, null);
            await PowerCmd.Apply<DexterityPower>(base.Owner, base.Amount, base.Owner, null);

            // 标记已触发，准备在回合结束时没收
            _hasTriggered = true;
        }
    }

    // 结合你在开发日志第四阶段“甩尾”中摸索出的最新钩子，处理临时属性的回收
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        // 确保是自己的回合结束，并且本回合确实发过奖励了
        if (_hasTriggered && side == base.Owner.Side)
        {
            // 核心技巧：向 Apply 传入负数 ( -base.Amount ) 即可完美扣除对应的属性
            await PowerCmd.Apply<StrengthPower>(base.Owner, -base.Amount, base.Owner, null);
            await PowerCmd.Apply<DexterityPower>(base.Owner, -base.Amount, base.Owner, null);

            // 临时属性扣除完毕，销毁祈雨状态本身，完成闭环
            await PowerCmd.Remove(this);
        }
    }
}
