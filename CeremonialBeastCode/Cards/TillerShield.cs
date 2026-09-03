using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using CeremonialBeast.CeremonialBeastCode.Powers;
using PlowPower = CeremonialBeast.CeremonialBeastCode.Powers.PlowPower;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class TillerShield : CeremonialBeastCard
{
    // 注册 Plow 词条的悬停提示，方便玩家查看机制
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PlowPower>(),
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    // 使用 PowerVar 规范，初始数值为 2
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<TillerShieldPower>(1m)
    ];

    public TillerShield()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        
        
        // 挂载耕耘护盾状态
        await PowerCmd.Apply<TillerShieldPower>(base.Owner.Creature, base.DynamicVars["TillerShieldPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级后获得的格挡提升至 3 点（增加 1 点）
        base.DynamicVars["TillerShieldPower"].UpgradeValueBy(1m);
    }
}
