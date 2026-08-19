using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class BehemothPhysique : CeremonialBeastCard
{
    // 基础减少失去的生命值 (升级前3，升级后4)
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("ReductionAmount", 3m)
    };

    public BehemothPhysique()
        : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 将减伤数值累加到隐藏存储器中
        decimal reductionToAdd = base.DynamicVars["ReductionAmount"].BaseValue;
        decimal currentReduction = BehemothPhysiquePower.StoredReduction.Get(base.Owner.Creature);
        BehemothPhysiquePower.StoredReduction.Set(base.Owner.Creature, currentReduction + reductionToAdd);

        // 挂载状态：层数永远传 1
        await PowerCmd.Apply<BehemothPhysiquePower>(
            base.Owner.Creature, 
            1m, 
            base.Owner.Creature, 
            this
        );
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["ReductionAmount"].UpgradeValueBy(1m);
    }
}