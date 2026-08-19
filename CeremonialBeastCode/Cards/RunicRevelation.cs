using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using CeremonialBeast.CeremonialBeastCode.Enchantments;
using MegaCrit.Sts2.Core.CardSelection; // ✨ 必须引入附魔的命名空间
// 确保引入 CustomTags 所在的命名空间
// using CeremonialBeast.CeremonialBeastCode.Enums;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class RunicRevelation : CeremonialBeastCard
{
    protected override HashSet<CardTag> CanonicalTags => [CustomTags.Ringing];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1) 
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        ..HoverTipFactory.FromEnchantment<InspireEnchantment>()
    ];

    public RunicRevelation()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 抽牌 (顺手加上 (int) 强转以保证底层类型安全)
        await CardPileCmd.Draw(choiceContext, (int)base.DynamicVars.Cards.BaseValue, base.Owner);

        // 获取手里所有“没有任何附魔”的牌
        var handCards = PileType.Hand.GetPile(base.Owner).Cards;
        var validCards = handCards.Where(CanEnchant).ToList();

        // 2. 升级逻辑：群体附魔
        if (base.IsUpgraded)
        {
            if (!validCards.Any()) return;

            foreach (CardModel item in validCards)
            {
                // 赋予临时附魔，并弹出视觉特效
                CardCmd.Enchant<InspireEnchantment>(item, 1m);
            }
            return;
        }

        var prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        var selectedCards = await CardSelectCmd.FromHand(
            choiceContext, 
            base.Owner, 
            prefs, 
            CanEnchant,
            this
        );
        CardModel? cardModel = selectedCards?.FirstOrDefault();
        if (cardModel != null && CanEnchant(cardModel))
        {
            CardCmd.Enchant<InspireEnchantment>(cardModel, 1m);
            CardCmd.Preview(cardModel);
        }
    }

    protected override void OnUpgrade()
    {
    }

    private bool CanEnchant(CardModel card)
    {
        return card != this
            && card.Enchantment == null
            && card.Rarity is CardRarity.Basic or CardRarity.Common or CardRarity.Uncommon or CardRarity.Rare or CardRarity.Ancient or CardRarity.Token;
    }
}
