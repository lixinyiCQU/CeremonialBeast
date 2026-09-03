using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class SurvivalInstinctBlockVar : BlockVar
{
    // 不再需要私有的 _baseValue，直接利用基类的构造函数即可
    public SurvivalInstinctBlockVar(decimal baseValue) : base(baseValue, ValueProp.Move) { }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        var c = (SurvivalInstinct)card;
        
        // ✨ 修复 1：直接读取并保存底层真实的 base.BaseValue
        decimal originalBase = base.BaseValue;
        
        // 半血时翻倍基础格挡；升级后自然从 12 提升到 16。
        if (c.IsHpBelowHalf)
        {
            base.BaseValue += originalBase;
        }
        
        // 调用底层逻辑，引擎会用修改后的 BaseValue 算出带有敏捷/虚弱影响的最终结果
        base.UpdateCardPreview(card, previewMode, target, runGlobalHooks);
        
        // ✨ 完美还原基础值，触发 UI 绿字 diff 显示！
        base.BaseValue = originalBase;
    }
}

// ==========================================

public class SurvivalInstinct() : CeremonialBeastCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new SurvivalInstinctBlockVar(6m)
    ];

    public bool IsHpBelowHalf => Owner?.Creature != null && (decimal)base.Owner.Creature.CurrentHp <= (decimal)base.Owner.Creature.MaxHp / 2m;

    protected override bool ShouldGlowGoldInternal => IsHpBelowHalf;

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // ✨ 修复 2：在打出瞬间，手动计算真实的终极基础格挡值
        decimal finalBaseBlock = DynamicVars.Block.BaseValue;
        
        if (IsHpBelowHalf)
        {
            finalBaseBlock *= 2m;
        }

        // 动态生成一个包含最终数值的 BlockVar 传给底层。
        // 因为贴上了 ValueProp.Move 标签，这 12 点格挡（假设触发条件）会完美享受 1 次完整的敏捷加成！
        await CreatureCmd.GainBlock(base.Owner.Creature, new BlockVar(finalBaseBlock, ValueProp.Move), play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}
