using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Commands;
using CeremonialBeast.CeremonialBeastCode.Enchantments;
using CeremonialBeast.CeremonialBeastCode.Powers;

using System.Threading.Tasks;

namespace CeremonialBeast.CeremonialBeastCode.Managers
{
    public class CultivateManager : CustomSingletonModel
    {
        public CultivateManager() : base(HookType.Combat) { }

        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var card = cardPlay.Card;

            // 检查这张牌是否有“耕耘”附魔
            if (card.Enchantment is CultivateEnchantment)
            {
                // 触发核心效果：玩家获得 2 层 Plow
                await PowerCmd.Apply<PlowPower>(
                    card.Owner.Creature, 
                    2m, 
                    card.Owner.Creature, 
                    null
                );
            }
        }
    }
}