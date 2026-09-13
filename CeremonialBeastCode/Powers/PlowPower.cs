using System.Collections.Generic;
using System.Linq; 
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using CeremonialBeast.CeremonialBeastCode.Relics; 
using System.Threading.Tasks;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public class PlowPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    private decimal _strBonusThisTurn = 0m;
    private decimal _dexPenaltyThisTurn = 0m;
    private decimal _dexBonusThisTurn = 0m;
    private decimal _strPenaltyThisTurn = 0m;

    private bool HasActiveTurnModifiers =>
        _strBonusThisTurn > 0m ||
        _dexPenaltyThisTurn > 0m ||
        _dexBonusThisTurn > 0m ||
        _strPenaltyThisTurn > 0m;
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        await base.BeforeSideTurnStart(choiceContext, side, participants, combatState);

        if (side == Owner.Side)
        {
            decimal stacks = Owner.GetPowerAmount<PlowPower>();
            
            if (stacks > 0)
            {
                var paleAntler = base.Owner.Player?.Relics.OfType<PaleAntlers>().FirstOrDefault();

                if (paleAntler != null)
                {
                    paleAntler.Flash();

                    _dexBonusThisTurn = stacks;
                    await PowerCmd.Apply<DexterityPower>(Owner, stacks, Owner, null);

                    if (!Owner.HasPower<AdaptiveBodyPower>())
                    {
                        _strPenaltyThisTurn = stacks;
                        await PowerCmd.Apply<StrengthPower>(Owner, -stacks, Owner, null);
                    }
                }
                else
                {
                    _strBonusThisTurn = stacks;
                    await PowerCmd.Apply<StrengthPower>(Owner, stacks, Owner, null);

                    if (!Owner.HasPower<AdaptiveBodyPower>())
                    {
                        _dexPenaltyThisTurn = stacks;
                        await PowerCmd.Apply<DexterityPower>(Owner, -stacks, Owner, null);
                    }
                }
            }
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);

        if (side == Owner.Side)
        {
            await RestoreActiveTurnModifiers(Owner);
        }
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);

        if (power == this && amount < 0m && HasActiveTurnModifiers)
        {
            await RestoreLostPlowModifiers(-amount);
        }
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        await base.AfterRemoved(oldOwner);

        await RestoreActiveTurnModifiers(oldOwner);
    }

    private async Task RestoreLostPlowModifiers(decimal lostStacks)
    {
        decimal remainingLoss = lostStacks;

        if (_strBonusThisTurn > 0)
        {
            decimal amountToRestore = decimal.Min(_strBonusThisTurn, remainingLoss);
            await PowerCmd.Apply<StrengthPower>(Owner, -amountToRestore, Owner, null);
            _strBonusThisTurn -= amountToRestore;
            remainingLoss -= amountToRestore;
        }

        remainingLoss = lostStacks;

        if (_dexPenaltyThisTurn > 0)
        {
            decimal amountToRestore = decimal.Min(_dexPenaltyThisTurn, remainingLoss);
            await PowerCmd.Apply<DexterityPower>(Owner, amountToRestore, Owner, null);
            _dexPenaltyThisTurn -= amountToRestore;
            remainingLoss -= amountToRestore;
        }

        remainingLoss = lostStacks;

        if (_dexBonusThisTurn > 0)
        {
            decimal amountToRestore = decimal.Min(_dexBonusThisTurn, remainingLoss);
            await PowerCmd.Apply<DexterityPower>(Owner, -amountToRestore, Owner, null);
            _dexBonusThisTurn -= amountToRestore;
            remainingLoss -= amountToRestore;
        }

        remainingLoss = lostStacks;

        if (_strPenaltyThisTurn > 0)
        {
            decimal amountToRestore = decimal.Min(_strPenaltyThisTurn, remainingLoss);
            await PowerCmd.Apply<StrengthPower>(Owner, amountToRestore, Owner, null);
            _strPenaltyThisTurn -= amountToRestore;
        }
    }

    private async Task RestoreActiveTurnModifiers(Creature owner)
    {
        if (_strBonusThisTurn > 0)
        {
            await PowerCmd.Apply<StrengthPower>(owner, -_strBonusThisTurn, owner, null);
            _strBonusThisTurn = 0m; 
        }

        if (_dexPenaltyThisTurn > 0)
        {
            await PowerCmd.Apply<DexterityPower>(owner, _dexPenaltyThisTurn, owner, null);
            _dexPenaltyThisTurn = 0m; 
        }

        if (_dexBonusThisTurn > 0)
        {
            await PowerCmd.Apply<DexterityPower>(owner, -_dexBonusThisTurn, owner, null);
            _dexBonusThisTurn = 0m;
        }

        if (_strPenaltyThisTurn > 0)
        {
            await PowerCmd.Apply<StrengthPower>(owner, _strPenaltyThisTurn, owner, null);
            _strPenaltyThisTurn = 0m;
        }
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        await base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);

        if (target == base.Owner && result.UnblockedDamage > 0) 
        {
            if (dealer == null || dealer.Side == base.Owner.Side)
            {
                return; 
            }

            if (props.HasFlag(ValueProp.Unpowered))
            {
                return; 
            }

            await PowerCmd.Apply<PlowPower>(base.Owner, -1m, base.Owner, null);
        }
    }

    public override string? CustomPackedIconPath => "res://CeremonialBeast/images/powers/PlowPower.png";
    public override string? CustomBigIconPath => "res://CeremonialBeast/images/powers/PlowPower.png";
}
