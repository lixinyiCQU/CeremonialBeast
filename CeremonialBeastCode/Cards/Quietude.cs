using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class Quietude : CeremonialBeastCard
{
    public override bool CanBeGeneratedInCombat => false;

    // 注册 消耗(Exhaust) 关键字
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    // 💡 补充悬停提示：虽然是回血，但我们最好把官方的回血提示加上（如果底层有的话）
    // 或者直接悬停展示状态图标，不过这里逻辑直观，留空或加特定提示皆可
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [];

    public Quietude()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 播放施法动画，并给屏幕加一点柔和的颜色特效（选配，提升宁静感）

        // 为玩家施加一个只持续到本回合结束的“宁静之息”监听状态
        // 传递的数值为 3m，代表每剩余 1 费回复 3 点生命
        await PowerCmd.Apply<QuietudePower>(base.Owner.Creature, 3m, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级后费用从 1 降低至 0
        EnergyCost.UpgradeBy(-1);
    }
}
