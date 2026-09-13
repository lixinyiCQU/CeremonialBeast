using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class Silence : CeremonialBeastCard
{
    // 告知引擎这张牌会提供格挡
    public override bool GainsBlock => true;

    // 💡 新增：注册悬停提示，展示衍生牌《NOPE》的原型预览
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Soberize>(base.IsUpgraded)
    ];

    // 使用 BlockVar 注册格挡，初始为 8
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move)
    ];

    public Silence()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 获得格挡
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        // 2. ✨ 核心优化：调用工厂方法一键印卡！
        // 传入 base.IsUpgraded，这样如果《消音》升级了，发到手里的《NOPE》也会自动是升级版
        await Soberize.CreateInHand(base.Owner, base.CombatState!, base.IsUpgraded);
    }

    protected override void OnUpgrade()
    {
        // 升级后格挡提升 1 点 (8 -> 9)
        base.DynamicVars.Block.UpgradeValueBy(1m);
    }
}
