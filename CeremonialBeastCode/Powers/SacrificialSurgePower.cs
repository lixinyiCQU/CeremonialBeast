using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public sealed class SacrificialSurgePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    
    public override PowerStackType StackType => PowerStackType.Counter;

    // ✨ 核心修复：引入“刚刚施加”标记，避免自我触发
    private bool _justApplied = true;

    // ✨ 核心机制：在卡牌打出“之后”触发效果
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        await base.AfterCardPlayed(context, cardPlay);

        // 1. 如果这是状态被挂上后听到的“第一次”出牌事件
        // 那么这必然是《献祭涌动》自身的打出结算。我们忽略它，并关闭标记。
        if (_justApplied)
        {
            _justApplied = false;
            return;
        }

        // 2. 监听真正的“下一张牌”：确保是我们自己打出的牌，并且该状态还有剩余层数
        if (cardPlay.Card.Owner == base.Owner.Player && base.Amount > 0)
        {
            // 图标闪烁视觉反馈
            Flash();

            // 强制进入 Ringing 状态（层数为 1）
            await PowerCmd.Apply<RingingPower>(base.Owner, 1m, base.Owner, null);

            // 消耗掉自身 1 层状态（如果减到 0，引擎底层会自动移除该状态）
            await PowerCmd.Apply<SacrificialSurgePower>(base.Owner, -1m, base.Owner, null);
        }
    }

    // ✨ 回合结束清理
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);
        
        // 如果是玩家回合结束，且该状态还没被消耗掉，则移除它
        if (side == base.Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
