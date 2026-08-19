using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Commands;
using CeremonialBeast.CeremonialBeastCode.Powers;
using CeremonialBeast.CeremonialBeastCode.Enchantments;

using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;

namespace CeremonialBeast.CeremonialBeastCode.Managers
{
    // CustomSingletonModel 的构造函数参数 (true, false) 表示只接收 Combat hooks，不接收 Run hooks
    public class AccumulateManager : CustomSingletonModel
    {
        public AccumulateManager() : base(HookType.Combat) { }

        // 使用 SpireField 为 CardModel 动态绑定一个 bool 值，默认值为 false
        public static readonly SpireField<CardModel, bool> HasAccumulatedThisCombat = new(() => false);

        // 全局监听：任何卡牌被打出后都会触发此钩子
        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var card = cardPlay.Card;

            // 1. 检查这张牌是否有“积蓄”附魔
            bool hasAccumulate = card.Enchantment is AccumulateEnchantment;

            // 2. 如果有附魔，且本场战斗还没触发过
            if (hasAccumulate && !HasAccumulatedThisCombat.Get(card))
            {
                // 立即将状态置为 true，防止多次触发
                HasAccumulatedThisCombat.Set(card, true);

                // 触发核心效果：获得 1 层 Plow
                await PowerCmd.Apply<PlowPower>(
                    card.Owner.Creature, 
                    1, 
                    card.Owner.Creature, 
                    null
                );

            }
        }
    }
}