using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using CeremonialBeast.CeremonialBeastCode.Character;
using CeremonialBeast.CeremonialBeastCode.Extensions;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Commands; 
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer; 
using CeremonialBeast.CeremonialBeastCode.Powers;
using Godot;
using BeastCharacter = CeremonialBeast.CeremonialBeastCode.Character.CeremonialBeast;

namespace CeremonialBeast.CeremonialBeastCode.Cards;
public enum RingingBehavior
{
    Blocked,
    Require,
    Exempt,
    Dynamic
}

[Pool(typeof(CeremonialBeastCardPool))]
public abstract class CeremonialBeastCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    public virtual RingingBehavior RingingInteractBehavior => RingingBehavior.Blocked;

    protected virtual bool UsesPlowAnimationWhileRinging => false;

    protected override bool IsPlayable
    {
        get
        {
            bool basePlayable = base.IsPlayable;

            if (Owner?.Creature == null)
            {
                return basePlayable;
            }

            bool hasRinging = Owner.Creature.HasPower<RingingPower>();
            bool hasProfaneRitual = Owner.Creature.HasPower<ProfaneRitualPower>(); 

            switch (RingingInteractBehavior)
            {
                case RingingBehavior.Require:
                    return basePlayable && hasRinging;

                case RingingBehavior.Exempt:
                case RingingBehavior.Dynamic:
                    return basePlayable;

                case RingingBehavior.Blocked:
                default:
                    if (hasProfaneRitual) return basePlayable;
                    return basePlayable && !hasRinging;
            }
        }
    }
    protected override bool ShouldGlowGoldInternal => 
        RingingInteractBehavior == RingingBehavior.Require ? IsPlayable : base.ShouldGlowGoldInternal;

    protected sealed override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool alreadyInRinging = Owner?.Creature?.HasPower<RingingPower>() ?? false;
        bool entersRingingFromThisCard = !alreadyInRinging && this.Tags.Contains(CustomTags.Ringing);
        bool usePlowAnimation = alreadyInRinging && UsesPlowAnimationWhileRinging;

        if (this.Tags.Contains(CustomTags.Ringing))
        {
            await PowerCmd.Apply<RingingPower>(
                Owner!.Creature, 
                1m, 
                Owner.Creature, 
                this
            );
        }

        if (Owner?.Creature != null && Owner.Character != null)
        {
            if (usePlowAnimation)
            {
                SfxCmd.Play(BeastCharacter.PlowSfx);
                await CreatureCmd.TriggerAnim(Owner.Creature, "Plow", 0f);
                await Cmd.Wait(0.5f); 
            }
            else
            {
                switch (this.Type)
                {
                    case CardType.Power:
                        SfxCmd.Play(BeastCharacter.ShrillSfx);
                        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
                        break;
                    case CardType.Skill:
                    case CardType.Attack:
                        break;
                }
            }
        }

        await OnPlayCard(choiceContext, cardPlay);

        if (Owner?.Creature != null && entersRingingFromThisCard)
        {
            SfxCmd.Play(BeastCharacter.StunSfx);
            await CreatureCmd.TriggerAnim(Owner.Creature, "Stun", 0.6f);
        }
        if (Owner?.Creature != null && alreadyInRinging)
        {
            if (usePlowAnimation)
            {
                await Cmd.Wait(0.2f);
                SfxCmd.Play(BeastCharacter.PlowEndSfx);
                await CreatureCmd.TriggerAnim(Owner.Creature, "EndPlow", 0f);
                await Cmd.Wait(0.65f);
            }
            if (Owner.Creature.HasPower<RingingPower>())
            {
                await CreatureCmd.TriggerAnim(Owner.Creature, "Stun", 0.6f);
            }
        }
    }

    protected virtual Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        return Task.CompletedTask;
    }
}
