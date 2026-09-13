using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class DesperateRevival : CeremonialBeastCard
{
    public override bool CanBeGeneratedInCombat => false;

    // 注册消耗关键字
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    // 注册再生词条的悬停提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<RegenPower>()
    };

    // 使用 PowerVar 传递再生层数，初始为 5
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<RegenPower>(5m)
    };

    public DesperateRevival()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    // ✨ 核心机制：只有当生命值低于或等于一半时才能打出
    protected override bool IsPlayable 
    {
        get 
        {
            // 防空判定：如果卡牌不在玩家手里（如在牌库总览界面），则不允许打出
            if (base.Owner?.Creature == null)
            {
                return false;
            }
            
            // 采用你提供的半血判定逻辑
            return base.IsPlayable
                && (decimal)base.Owner.Creature.CurrentHp <= (decimal)base.Owner.Creature.MaxHp / 2m;
        }
    }

    // 满足打出条件时，卡牌边框发出金光提示玩家处于“绝境”可施放状态
    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 播放施法动画
        

        // 获得再生状态
        await PowerCmd.Apply<RegenPower>(base.Owner.Creature, base.DynamicVars["RegenPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级后再生层数提升 1 层 (5 -> 6)
        base.DynamicVars["RegenPower"].UpgradeValueBy(1m);
    }
}
