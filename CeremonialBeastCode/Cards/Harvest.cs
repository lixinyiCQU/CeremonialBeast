using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures; // 需要引入以支持 Creature
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars; // 需要引入以支持 DynamicVar
using CeremonialBeast.CeremonialBeastCode.Powers;
using MegaCrit.Sts2.Core.Models;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

// ==========================================
// ✨ 新增：专门用于实时计算手牌数量的动态变量
// ==========================================
public class HarvestPlowVar : DynamicVar
{
    public HarvestPlowVar() : base("PlowAmount", 1m) { }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        if (card.Owner != null && card.CombatState != null)
        {
            // 获取当前手牌的真实数量
            int handSize = PileType.Hand.GetPile(card.Owner).Cards.Count;
            
            // 细节打磨：如果在手牌里看，手牌数已经包含它自己；
            // 如果在商店、选牌界面、抽牌堆看，我们要模拟“它来到手里”的情况，所以 +1。
            if (card.Pile?.Type != PileType.Hand)
            {
                handSize += 1;
            }
            
            // 直接更新底层 BaseValue
            base.BaseValue = handSize;
        }
        else
        {
            base.BaseValue = 1m;
        }
        
        base.UpdateCardPreview(card, previewMode, target, runGlobalHooks);
    }
}

// ==========================================

public sealed class Harvest : CeremonialBeastCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PlowPower>()
    ];

    // ✨ 注册我们刚刚写好的动态变量
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new HarvestPlowVar()
    };

    public Harvest()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int handSize = PileType.Hand.GetPile(base.Owner).Cards.Count;
        if (base.Pile?.Type != PileType.Hand)
        {
            handSize++;
        }

        decimal amount = handSize;
        base.DynamicVars["PlowAmount"].BaseValue = amount;

        if (amount > 0)
        {
            await PowerCmd.Apply<PlowPower>(base.Owner.Creature, amount, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
