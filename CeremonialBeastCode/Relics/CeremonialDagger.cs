using BaseLib.Utils;
using CeremonialBeast.CeremonialBeastCode.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Threading.Tasks;

namespace CeremonialBeast.CeremonialBeastCode.Relics;

public class CeremonialDagger : CeremonialBeastRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public static readonly SpireField<ICombatState, bool> HasTriggeredThisCombat = new(() => false);

    public async Task TriggerDaggerEffect(PlayerChoiceContext? choiceContext, ICombatState combatState)
    {
        if (HasTriggeredThisCombat.Get(combatState)) return;

        HasTriggeredThisCombat.Set(combatState, true);

        // ✨ 核心修复：保持使用 BlockVar，但将其标签改为 Unpowered！
        await CreatureCmd.GainBlock(
            Owner.Creature, 
            new BlockVar(8m, ValueProp.Unpowered), // 8m 代表 8 点无加成的基础格挡
            null             
        );
    }
    
    internal async Task TriggerDaggerEffect(object value, ICombatState combatState)
    {
        throw new NotImplementedException();
    }
}
