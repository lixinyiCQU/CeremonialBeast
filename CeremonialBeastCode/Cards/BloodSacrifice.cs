using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using CeremonialBeast.CeremonialBeastCode.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Localization;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class BloodSacrifice : CeremonialBeastCard
{
    // 注册 消耗(Exhaust) 关键字
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    // ==========================================
    // 💡 优化点 2：注册官方的 CardsVar
    // ==========================================
    // 用于控制选牌的数量上限，这里设为 10
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new CardsVar(10)
    ];

    public BloodSacrifice()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 呼出选牌界面
        // ✨ 核心修复：显式使用 LocString 绑定特定的本地化提示文本
        var selectedCards = await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(
                new LocString("cards", "CEREMONIALBEAST-BLOOD_SACRIFICE_PROMPT"), // 绑定提示文本的 Key
                0, 
                base.DynamicVars.Cards.IntValue
            ), 
            context: choiceContext, 
            player: base.Owner, 
            filter: null, 
            source: this
        );

        // 防空判断：如果玩家取消了选择，或没选牌直接点了确认
        if (selectedCards == null || !selectedCards.Any())
        {
            return;
        }

        // 2. 消耗选中的牌并记录数量
        int exhaustedCount = 0;
        foreach (CardModel item in selectedCards)
        {
            await CardCmd.Exhaust(choiceContext, item);
            exhaustedCount++;
        }

        // 3. 核心效果：每消耗 1 张牌，获得 1 费
        if (exhaustedCount > 0)
        {
            await PlayerCmd.GainEnergy(exhaustedCount, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后费用降低至 0 费
        EnergyCost.UpgradeBy(-1);
    }
}