using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class EnchantedDeterrence : CeremonialBeastCard
{
    // ==========================================
    // ✨ 注册能力变量：基础 1 层
    // ==========================================
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<EnchantedDeterrencePower>(1m)
    };

    public EnchantedDeterrence()
        : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal powerAmount = base.DynamicVars.Values.OfType<PowerVar<EnchantedDeterrencePower>>().First().BaseValue;

        await PowerCmd.Apply<EnchantedDeterrencePower>(
            base.Owner.Creature, 
            powerAmount, 
            base.Owner.Creature, 
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级逻辑：费用减少 1 点 (2 -> 1)
        base.EnergyCost.UpgradeBy(-1);
    }
}