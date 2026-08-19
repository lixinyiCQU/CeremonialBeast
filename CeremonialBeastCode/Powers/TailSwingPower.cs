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

public class TailSwingPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 核心逻辑 1：每当打出攻击牌，给目标上易伤
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Attack && cardPlay.Target != null)
        {
            // 施加原版的 VulnerablePower
            await PowerCmd.Apply<VulnerablePower>(
                cardPlay.Target, 
                base.Amount, 
                base.Owner, 
                null
            );
        }
    }

    // 修复后的核心逻辑 2：使用最新的 AfterTurnEnd 钩子
    // 实现“在本回合”的效果，在玩家回合结束时移除此状态
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        // 判定如果是玩家回合结束
        if (side == CombatSide.Player)
        {
            // 移除该状态
            await PowerCmd.Remove(this);
        }
    }
}
