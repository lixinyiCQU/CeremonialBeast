using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class Tenacity() : CeremonialBeastCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    // 定义一个专属的动态变量来存储格挡值。
    // 我们不使用标准的 BlockVar，因为这张牌打出时并不直接提供格挡，而是传递给状态。
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("BlockValue", 5m)];

    // 使用你最新的架构方法 OnPlayCard
    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 施加专属的能力状态，并将动态变量（5点/升级后7点）作为 Amount 传递给状态
        await PowerCmd.Apply<TenacityPower>(
            Owner.Creature, 
            DynamicVars["BlockValue"].BaseValue, 
            Owner.Creature, 
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级后格挡值提升至 7 (+2)
        DynamicVars["BlockValue"].UpgradeValueBy(2m);
    }
}
