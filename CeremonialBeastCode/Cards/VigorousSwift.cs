using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class VigorousSwift : CeremonialBeastCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<RingingPower>()
    ];

    // 注册抽牌变量，初始值为 2
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(2)
    };

    public VigorousSwift()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 获取当前的抽牌数量
        int drawCount = (int)base.DynamicVars["Cards"].BaseValue;

        // 2. 执行抽牌指令
        if (drawCount > 0)
        {
            await CardPileCmd.Draw(choiceContext, drawCount, base.Owner);
        }
    }

    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.AfterCardPlayedLate(choiceContext, cardPlay);

        if (cardPlay.Card == this && cardPlay.Card.Owner == base.Owner)
        {
            await PowerCmd.Apply<VigorousSwiftPower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后初始抽牌数从 2 提升至 3
        base.DynamicVars["Cards"].UpgradeValueBy(1m);
    }
}
