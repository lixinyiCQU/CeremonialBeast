using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures; // ✨ 必须引入以支持 Creature
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars; // ✨ 必须引入以支持 DynamicVar
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

// ==========================================
// ✨ 新增：专门用于实时计算“已损失生命值”的动态变量
// ==========================================
public class PainReversalHpVar : DynamicVar
{
    public PainReversalHpVar() : base("LostHp", 0m) { }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        if (card.Owner?.Creature != null)
        {
            // 实时计算：最大生命值 - 当前生命值
            base.BaseValue = card.Owner.Creature.MaxHp - card.Owner.Creature.CurrentHp;
        }
        
        base.UpdateCardPreview(card, previewMode, target, runGlobalHooks);
    }
}

// ==========================================

public class PainReversal() : CeremonialBeastCard(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    // ✨ 注册我们刚刚写好的动态变量
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PainReversalHpVar()
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // ✨ 直接读取动态变量算好的数值，既简洁又保证了 UI 与实际效果绝对统一！
        decimal lostHp = base.DynamicVars["LostHp"].BaseValue;

        if (lostHp > 0m && play.Target != null)
        {
            await CreatureCmd.Damage(
                choiceContext,
                play.Target,
                lostHp,
                ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
                this,
                play);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
