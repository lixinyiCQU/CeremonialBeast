using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer; // 确保引入了 PlayerChoiceContext
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public class BlessedHarvestPower : CeremonialBeastPower
{
    public override PowerType Type => PowerType.Buff;
    
    // 使用 Intensity，代表层数等于每次触发抽牌的数量
    public override PowerStackType StackType => PowerStackType.Counter;

    // ==========================================
    // ✨ 核心机制：修正 CardPileCmd.Draw 的参数顺序
    // ==========================================
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        // 1. 确保抽上来的这张牌属于当前能力的拥有者（即玩家自己）
        // 2. 确保这张牌身上带有附魔 (Enchantment != null)
        if (card.Owner?.Creature == base.Owner && card.Enchantment != null)
        {
            // 闪烁状态图标给予反馈
            Flash();

            await CardPileCmd.Draw(choiceContext, (int)base.Amount, card.Owner);
        }
    }
}
