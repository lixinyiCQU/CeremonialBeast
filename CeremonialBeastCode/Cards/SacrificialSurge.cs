using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class SacrificialSurge : CeremonialBeastCard
{
    // 注册悬停提示：显示能量说明和 Ringing 词条
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        base.EnergyHoverTip,
        HoverTipFactory.FromPower<RingingPower>() 
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new EnergyVar(3)
    ];

    public SacrificialSurge()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        

        // 1. 获得能量
        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner);

        // 2. 挂载状态（层数为 1，代表下 1 张牌生效）
        await PowerCmd.Apply<SacrificialSurgePower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级后能量提升 1 点 (3 -> 4)
        base.DynamicVars.Energy.UpgradeValueBy(1m);
    }
}