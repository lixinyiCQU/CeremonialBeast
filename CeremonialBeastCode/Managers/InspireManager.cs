using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Commands;
using CeremonialBeast.CeremonialBeastCode.Enchantments;
using System.Threading.Tasks;
using BaseLib.Utils;

namespace CeremonialBeast.CeremonialBeastCode.Managers
{
    public class InspireManager : CustomSingletonModel
    {
        public InspireManager() : base(HookType.Combat) { }

        // 使用 SpireField 追踪这张牌在本场战斗中是否已经触发过“启迪”
        public static readonly SpireField<CardModel, bool> HasInspiredThisCombat = new(() => false);

        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var card = cardPlay.Card;

            // 1. 检查这张牌是否有“启迪”附魔
            bool hasInspire = card.Enchantment is InspireEnchantment;

            // 2. 如果有附魔，且本场战斗是第一次打出
            if (hasInspire && !HasInspiredThisCombat.Get(card))
            {
                // 立即将状态置为 true，防止被双发等效果重复触发
                HasInspiredThisCombat.Set(card, true);

                // 3. 触发核心效果：抽 1 张牌
                await CardPileCmd.Draw(choiceContext, 1, card.Owner);

                // ==========================================
                // ✨ 核心修复：使用你找到的官方加费 API！
                // 传入 1m 满足 decimal 的类型要求，并直接传入 card.Owner (Player实体)
                // ==========================================
                await PlayerCmd.GainEnergy(1m, card.Owner); 
            }
        }
    }
}