using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using CeremonialBeast.CeremonialBeastCode.Enchantments;
using MegaCrit.Sts2.Core.CardSelection;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class Plowing() : CeremonialBeastCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    // ✨ 注册动态变量：基础选择 1 张牌
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    // ✨ 注册悬停提示：让玩家鼠标放上去时能看到《耕耘》的效果说明
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        ..HoverTipFactory.FromEnchantment<CultivateEnchantment>()
    ];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 1. 获取当前需要选择的数量（未升级 1，升级后 2）
        int amount = (int)base.DynamicVars.Cards.BaseValue;

        // ==========================================
        // ✨ 核心修复：使用官方的带参构造函数，并调用专属的附魔文本！
        // 这样在游戏界面上方会自动显示完美适配本地化的“选择X张牌进行附魔”
        // ==========================================
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, amount);

        // 3. 呼出选牌界面，自带 filter 过滤器（灰掉已有附魔的牌和自己）
        var selectedCards = await CardSelectCmd.FromHand(
            choiceContext, 
            base.Owner, 
            prefs, 
            CanEnchant,
            this
        );

        // 4. 遍历选出的所有卡牌，进行附魔
        if (selectedCards != null)
        {
            foreach (var card in selectedCards)
            {
                // 双重保险
                if (card.Enchantment == null)
                {
                    CardCmd.Enchant<CultivateEnchantment>(card, 1m);
                    
                    // 弹出卡面放大并闪烁的视觉特效
                    CardCmd.Preview(card);
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        // ✨ 升级后选择的卡牌数量增加 1 张
        DynamicVars.Cards.UpgradeValueBy(1m);
    }

    private bool CanEnchant(CardModel card)
    {
        return card != this
            && card.Enchantment == null
            && card.Rarity is CardRarity.Basic or CardRarity.Common or CardRarity.Uncommon or CardRarity.Rare or CardRarity.Ancient or CardRarity.Token;
    }
}
