using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

// 💡 优化 1：使用 sealed 关键字并应用 Dynamic 架构
public sealed class HoldBreath() : CeremonialBeastCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // ==========================================
    // 💡 优化 2：声明为 Dynamic 类型
    public override RingingBehavior RingingInteractBehavior => RingingBehavior.Dynamic;

    // 💡 优化 3：移除标签自动挂载逻辑
    protected override HashSet<CardTag> CanonicalTags => [];

    // ✨ 核心修复：升级后返回 Retain，升级前返回空！
    public override IEnumerable<CardKeyword> CanonicalKeywords => base.IsUpgraded ? [CardKeyword.Retain] : [];

    // 💡 优化 5：完善悬停提示，让玩家看清两种状态
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromPower<CeremonialBeast.CeremonialBeastCode.Powers.RingingPower>(),
        HoverTipFactory.FromPower<RetainHandPower>()
    ];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 判定打出瞬间玩家的状态
        bool alreadyRinging = base.Owner.Creature.HasPower<CeremonialBeast.CeremonialBeastCode.Powers.RingingPower>();

        if (alreadyRinging)
        {
            // 情况 A：如果在 Ringing 状态下打出，效果切换为“保留手牌”
            // 提示：RetainHandPower 是官方底层状态，1m 代表本回合生效
            await PowerCmd.Apply<RetainHandPower>(
                base.Owner.Creature, 
                1m, 
                base.Owner.Creature, 
                this
            );
        }
        else
        {
            // 情况 B：在非 Ringing 状态下打出，进入 Ringing
            await PowerCmd.Apply<CeremonialBeast.CeremonialBeastCode.Powers.RingingPower>(
                base.Owner.Creature, 
                1m, 
                base.Owner.Creature, 
                this
            );
        }

        // 播放通用的施法动画
        
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
