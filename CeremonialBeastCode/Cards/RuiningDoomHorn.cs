using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class RuiningDoomHornTotalDamageVar : DynamicVar
{
    public RuiningDoomHornTotalDamageVar() : base("TotalDamage", 0m) { }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        if (target == null || card.CombatState == null)
        {
            PreviewValue = 0m;
            return;
        }

        int x = Hook.ModifyXValue(card.CombatState, card, card.EnergyCost.GetAmountToSpend());
        decimal adjustedBaseDamage = CalculateAdjustedBaseDamage(card, x);

        PreviewValue = Math.Max(0m, Hook.ModifyDamage(
            card.Owner.RunState,
            card.CombatState,
            target,
            card.Owner.Creature,
            adjustedBaseDamage,
            ValueProp.Move,
            card,
            null,
            ModifyDamageHookType.All,
            previewMode,
            out IEnumerable<AbstractModel> _));
    }

    public static decimal CalculateAdjustedBaseDamage(CardModel card, int x)
    {
        if (x <= 0)
        {
            return 0m;
        }

        decimal hitCount = card.DynamicVars["Factor"].BaseValue * x;
        decimal strength = card.Owner.Creature.GetPowerAmount<StrengthPower>();

        // The normal damage hook adds Strength once. Pre-add the remaining copies
        // so Strength contributes once for every point of Factor * X damage.
        return Math.Max(0m, hitCount + strength * (hitCount - 1m));
    }
}

public sealed class RuiningDoomHorn : CeremonialBeastCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    private const string FactorKey = "Factor";

    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(FactorKey, 2m),
        new DamageVar(2m, ValueProp.Move),
        new RuiningDoomHornTotalDamageVar()
    ];

    public RuiningDoomHorn()
        : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        int x = ResolveEnergyXValue();
        if (x <= 0)
        {
            return;
        }

        decimal finalBaseDamage = RuiningDoomHornTotalDamageVar.CalculateAdjustedBaseDamage(this, x);

        await DamageCmd.Attack(finalBaseDamage)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars[FactorKey].UpgradeValueBy(1m);
        base.DynamicVars.Damage.UpgradeValueBy(1m);
    }
}
