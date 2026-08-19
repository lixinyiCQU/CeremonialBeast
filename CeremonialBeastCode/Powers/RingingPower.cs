using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using CeremonialBeast.CeremonialBeastCode.Relics;
using CeremonialBeast.CeremonialBeastCode.Cards; 
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards; 
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using BeastCharacter = CeremonialBeast.CeremonialBeastCode.Character.CeremonialBeast;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public class RingingPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.None;
    
    public static readonly SpireField<ICombatState, int> RingingCountThisCombat = new(() => 0);

    public override bool ShouldPlay(CardModel card, AutoPlayType _)
    {
        if (card.Owner.Creature != base.Owner)
        {
            return true;
        }

        if (card is CeremonialBeastCard)
        {
            return true;
        }

        return false; 
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await base.AfterApplied(applier, cardSource);

        bool appliedByRingingCard = cardSource is CeremonialBeastCard card
            && card.Tags.Contains(CustomTags.Ringing);

        if (base.Owner != null && !appliedByRingingCard)
        {
            SfxCmd.Play(BeastCharacter.StunSfx);
            await CreatureCmd.TriggerAnim(base.Owner, "Stun", 0f);
            await Cmd.Wait(0.4f);
        }

        int currentCount = RingingCountThisCombat.Get(base.CombatState);
        RingingCountThisCombat.Set(base.CombatState, currentCount + 1);
        
        var player = base.CombatState.Players.FirstOrDefault(p => p.Creature == base.Owner);
        if (player != null)
        {
            var dagger = player.Relics.FirstOrDefault(r => r is CeremonialDagger) as CeremonialDagger;
            if (dagger != null)
            {
                await dagger.TriggerDaggerEffect(null, base.CombatState);
            }
        }
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        await base.AfterRemoved(oldOwner);

        if (oldOwner != null)
        {
            await CreatureCmd.TriggerAnim(oldOwner, "Unstun", 0f);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);

        if (side == base.Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }

    public override string? CustomPackedIconPath => "res://CeremonialBeast/images/powers/RingingPower.png";
    public override string? CustomBigIconPath => "res://CeremonialBeast/images/powers/RingingPower.png";
}
