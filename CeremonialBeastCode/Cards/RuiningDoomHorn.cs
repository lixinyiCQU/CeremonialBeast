using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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
        int x = card.EnergyCost.CapturedXValue;
        if (x <= 0)
        {
            x = card.Owner?.PlayerCombatState?.Energy ?? 0;
        }

        decimal multiplier = card.DynamicVars["Factor"].BaseValue * x;
        decimal strength = card.Owner?.Creature.GetPowerAmount<StrengthPower>() ?? 0m;
        base.BaseValue = Math.Max(0m, multiplier + strength * multiplier);

        base.UpdateCardPreview(card, previewMode, target, runGlobalHooks);
    }
}

public sealed class RuiningDoomHorn : CeremonialBeastCard
{
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

        decimal factor = base.DynamicVars[FactorKey].BaseValue;
        decimal multiplier = factor * x;
        decimal strength = base.Owner.Creature.GetPowerAmount<StrengthPower>();
        decimal finalBaseDamage = Math.Max(0m, multiplier + strength * (multiplier - 1m));

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
