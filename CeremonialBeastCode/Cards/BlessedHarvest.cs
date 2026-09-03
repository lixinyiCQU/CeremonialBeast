using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class BlessedHarvest : CeremonialBeastCard
{
    // ==========================================
    // ✨ 注册能力变量：基础给 1 层（即抽 1 张牌）
    // ==========================================
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<BlessedHarvestPower>(1m)
    };

    // 初始费用 1 费
    public BlessedHarvest()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 动态获取层数变量 (1m)
        decimal powerAmount = base.DynamicVars.Values.OfType<PowerVar<BlessedHarvestPower>>().First().BaseValue;

        // 挂载能力状态
        await PowerCmd.Apply<BlessedHarvestPower>(
            base.Owner.Creature, 
            powerAmount, 
            base.Owner.Creature, 
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级逻辑：费用减少 1 点 (2费 -> 1费)
        base.EnergyCost.UpgradeBy(-1);
    }
}
