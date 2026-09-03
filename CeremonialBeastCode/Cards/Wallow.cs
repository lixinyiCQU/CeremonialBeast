using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class Wallow : CeremonialBeastCard
{
    public override bool CanBeGeneratedInCombat => false;

    public override bool GainsBlock => false;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<RegenPower>(),
        HoverTipFactory.FromPower<PlatingPower>()
    ];

    // 💡 调整 3：将 BlockVar 替换为 PowerVar<PlatingPower>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<RegenPower>(1m),
        new PowerVar<PlatingPower>(2m)
    ];

    public Wallow()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 获得再生状态
        await PowerCmd.Apply<RegenPower>(
            base.Owner.Creature, 
            base.DynamicVars["RegenPower"].BaseValue, 
            base.Owner.Creature, 
            this
        );

        // 2. 💡 调整 4：施加覆甲状态，使用 PowerCmd 而非 CreatureCmd.GainBlock
        await PowerCmd.Apply<PlatingPower>(
            base.Owner.Creature, 
            base.DynamicVars["PlatingPower"].BaseValue, 
            base.Owner.Creature, 
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 💡 调整 5：升级逻辑同步修改，这里演示将覆甲提升 1 点 (2 -> 3)
        base.DynamicVars["PlatingPower"].UpgradeValueBy(1m);
        
        // 如果你也想升级再生层数，可以取消下面这行的注释：
        base.DynamicVars["RegenPower"].UpgradeValueBy(1m);
    }
}
