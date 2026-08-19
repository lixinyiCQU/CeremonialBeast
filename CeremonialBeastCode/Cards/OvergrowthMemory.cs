using System.Collections.Generic;
using System.Linq; // 必须引入，用于使用 .FirstOrDefault()
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection; // 必须引入，用于 CardSelectorPrefs
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class OvergrowthMemory : CeremonialBeastCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    // 注册基础关键字：消耗 (Exhaust)
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust 
    ];

    public OvergrowthMemory()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 设置选择偏好：标题提示 (SelectionScreenPrompt) 与选择数量 (1)
        CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, 0, base.DynamicVars.Cards.IntValue);
        
        // 2. 获取当前玩家的弃牌堆
        CardPile pile = PileType.Discard.GetPile(base.Owner);
        
        // 3. 呼出网格选择界面，等待玩家选择。加上 ? 处理可空引用
        var selectedCards = await CardSelectCmd.FromSimpleGrid(choiceContext, pile.Cards, base.Owner, prefs);
        
        // 4. 如果玩家确实选择了一张牌（而不是弃牌堆为空或触发了取消操作）
        foreach (CardModel cardModel in selectedCards)
        {
            // 将选中的卡牌移动到手牌堆
            await CardPileCmd.Add(cardModel, PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
