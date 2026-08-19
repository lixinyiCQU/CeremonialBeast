using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using CeremonialBeast.CeremonialBeastCode.Enchantments;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public sealed class DarkRevelationPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    
    // 支持层数叠加，玩家打出多张暗黑启示后，每回合能附魔更多牌
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        await base.AfterSideTurnStart(side, participants, combatState);

        // 确保是玩家回合开始
        if (side == base.Owner.Side)
        {
            var player = combatState.Players.FirstOrDefault(p => p.Creature == base.Owner);
            if (player == null) return;

            // 获取手牌列表
            var handCards = PileType.Hand.GetPile(player).Cards;

            // ==========================================
            // ✨ 核心修复 1：仅限“攻击牌”且“严格没有任何附魔 (c.Enchantment == null)”
            // ==========================================
            var validCards = handCards.Where((CardModel c) => 
                c.Type == CardType.Attack && c.Enchantment == null
            ).ToList();

            // ==========================================
            // ✨ 核心修复 2：删除了让步防呆机制
            // 如果手里连一张满足条件的牌都没有，严格禁止附魔，直接结束当前效果
            // ==========================================
            if (!validCards.Any()) return;

            // 根据状态层数决定附魔数量
            int cardsToEnchant = (int)base.Amount;
            
            // 状态栏图标闪烁，给予正向视觉反馈
            Flash();

            // 随机抽样
            validCards.StableShuffle(player.RunState.Rng.Shuffle);
            var shuffledCards = validCards.Take(cardsToEnchant);

            foreach (CardModel card in shuffledCards)
            {
                // ✨ 仅在战斗中生效的附魔：不触碰 DeckVersion
                CardCmd.Enchant<Corrupted>(card, 1m);
                
                // 强制刷新牌面UI，弹出附魔特效
                CardCmd.Preview(card);
            }
        }
    }
}
