using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class SurvivalInstinct() : CeremonialBeastCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6m, ValueProp.Move),
        new BlockVar("LowHealthBlock", 12m, ValueProp.Move)
    ];

    public bool IsHpBelowHalf => Owner?.Creature != null
        && (decimal)Owner.Creature.CurrentHp <= (decimal)Owner.Creature.MaxHp / 2m;

    protected override bool ShouldGlowGoldInternal => IsHpBelowHalf;

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var block = IsHpBelowHalf ? (BlockVar)DynamicVars["LowHealthBlock"] : DynamicVars.Block;
        await CreatureCmd.GainBlock(Owner.Creature, block, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["LowHealthBlock"].UpgradeValueBy(4m);
    }
}
