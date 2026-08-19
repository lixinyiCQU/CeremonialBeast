using System.Threading.Tasks;
using BaseLib.Abstracts;
using CeremonialBeast.CeremonialBeastCode.Enchantments;
using CeremonialBeast.CeremonialBeastCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CeremonialBeast.CeremonialBeastCode.Managers;

public class BlessingManager : CustomSingletonModel
{
    public BlessingManager() : base(HookType.Combat) { }

    // ==========================================
    // ✨ 核心修复 1：使用官方专属的费用覆盖机制
    // ==========================================
    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        // 默认将输出值设为原费用（防止未命中时出错）
        modifiedCost = originalCost;

        // 检查卡牌是否具有“赐福”附魔
        if (card.Enchantment is BlessingEnchantment)
        {
            // 严谨起见，学习官方的做法，只修改位于手牌或出牌区的卡牌费用
            bool inValidPile = false;
            switch (card.Pile?.Type)
            {
                case PileType.Hand:
                case PileType.Play:
                    inValidPile = true;
                    break;
                default:
                    inValidPile = false;
                    break;
            }

            if (inValidPile)
            {
                // 将最终费用强制覆写为 0m (即官方源码中的 default(decimal))
                modifiedCost = 0m;
                return true; // 告诉引擎我们成功干预了费用的结算
            }
        }

        return false; // 不满足条件，不作干预
    }

    // ==========================================
    // ✨ 核心修复 2：打出后挂载 Ringing 状态
    // ==========================================
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var card = cardPlay.Card;

        if (card.Enchantment is BlessingEnchantment)
        {
            // 给玩家自身赋予一层 Ringing 状态，记得写 1m 满足底层参数要求
            await PowerCmd.Apply<RingingPower>(
                card.Owner.Creature, 
                1m, 
                card.Owner.Creature, 
                null
            );
        }
    }
}