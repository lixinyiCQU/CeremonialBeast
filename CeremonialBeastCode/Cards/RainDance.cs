using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class RainDance : CeremonialBeastCard
{
    // 1. 注册悬停提示：当玩家鼠标放在卡牌上时，自动显示力量和敏捷的说明黑框
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<DexterityPower>()
    };

    // 2. 弃用 MagicVar，严格采用官方的 PowerVar 规范
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<StrengthPower>(3m),
        new PowerVar<DexterityPower>(3m)
    };

    public RainDance()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 由于力量和敏捷的层数在设计中是绑定的（都是 3 或 4），
        // 我们可以安全地将 Strength 的数值传递给延时 Power 作为它的层数
        await PowerCmd.Apply<RainDancePower>(base.Owner.Creature, base.DynamicVars.Strength.BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级时，明确提升两个专属变量的值
        base.DynamicVars.Strength.UpgradeValueBy(1m);
        base.DynamicVars.Dexterity.UpgradeValueBy(1m);
    }
}