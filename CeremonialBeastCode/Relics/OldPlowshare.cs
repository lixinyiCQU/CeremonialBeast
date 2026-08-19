using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace CeremonialBeast.CeremonialBeastCode.Relics;

public class OldPlowshare : CeremonialBeastRelic
{
    // 设置稀有度为稀有 (Rare)
    public override RelicRarity Rarity => RelicRarity.Rare;

    // ==========================================
    // ✨ 核心机制：拦截回合结束阶段
    // ==========================================
    // 使用 BeforeTurnEnd 钩子，它会在玩家点击“结束回合”后、手牌被移入弃牌堆之前触发
    public override Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        // 确保是当前玩家的回合结束
        if (side == base.Owner.Creature.Side)
        {
            bool hasActivated = false;

            // 1. 获取玩家当前手牌堆的所有卡牌
            var handCards = PileType.Hand.GetPile(base.Owner).Cards;

            // 2. 遍历检查手牌
            foreach (var card in handCards)
            {
                // 如果卡牌拥有附魔，并且当前还没有被标记为保留
                if (card.Enchantment != null && !card.ShouldRetainThisTurn)
                {
                    // 调用官方 API，赋予卡牌单回合保留效果
                    card.GiveSingleTurnRetain();
                    hasActivated = true;
                }
            }

            // 3. 如果成功保留了至少一张牌，遗物闪烁给予反馈
            if (hasActivated)
            {
                Flash();
            }
        }

        return Task.CompletedTask;
    }
}
