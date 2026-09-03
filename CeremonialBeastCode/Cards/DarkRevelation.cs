using System.Collections.Generic;
using System.Linq; // ✨ 引入 Linq 用于安全拉取变量
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars; // ✨ 引入官方动态变量库
using CeremonialBeast.CeremonialBeastCode.Powers;
using CeremonialBeast.CeremonialBeastCode.Enchantments;
using MegaCrit.Sts2.Core.Models.Enchantments;

// using CeremonialBeast.CeremonialBeastCode.Enchantments;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class DarkRevelation : CeremonialBeastCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        // 绑定特定状态的层数变量
        new PowerVar<DarkRevelationPower>(1m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        ..HoverTipFactory.FromEnchantment<Corrupted>()
    ];

    public DarkRevelation()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        

        // ✨ 安全拉取：从 DynamicVars 中精准提取绑定的 PowerVar 的值
        decimal powerAmount = base.DynamicVars.Values.OfType<PowerVar<DarkRevelationPower>>().First().BaseValue;

        await PowerCmd.Apply<DarkRevelationPower>(
            base.Owner.Creature, 
            powerAmount, 
            base.Owner.Creature, 
            this
        );
    }

    protected override void OnUpgrade()
    {
        // ✨ 安全升级：精准锁定该状态变量并提升数值
        base.DynamicVars.Values.OfType<PowerVar<DarkRevelationPower>>().First().UpgradeValueBy(1m);
    }
}
