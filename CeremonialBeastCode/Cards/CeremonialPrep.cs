using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class CeremonialPrep : CeremonialBeastCard
{
    private const string _discount = "Discount";
    private const string _plays = "Plays";

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar(_discount, 1m),
        new DynamicVar(_plays, 2m) 
    };

    public CeremonialPrep()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) //
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 根据是否升级挂载对应的 Power
        if (base.IsUpgraded)
        {
            await PowerCmd.Apply<CeremonialPrepUpgradedPower>(base.Owner.Creature, base.DynamicVars[_plays].BaseValue, base.Owner.Creature, this);
        }
        else
        {
            await PowerCmd.Apply<CeremonialPrepPower>(base.Owner.Creature, base.DynamicVars[_plays].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars[_discount].UpgradeValueBy(1m);
    }
}