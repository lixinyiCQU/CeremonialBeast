using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Relics;

public class RustedCopperBell : CeremonialBeastRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(8m, ValueProp.Unpowered)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.Static(StaticHoverTip.Block)];

    public static readonly SpireField<ICombatState, bool> HasTriggeredThisCombat = new(() => false);

    public async Task TriggerBellEffect(PlayerChoiceContext? choiceContext, ICombatState combatState)
    {
        if (HasTriggeredThisCombat.Get(combatState)) return;

        HasTriggeredThisCombat.Set(combatState, true);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
    }
}
