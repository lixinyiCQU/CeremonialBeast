using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models.Powers;
using CeremonialBeast.CeremonialBeastCode.Powers;
using MegaCrit.Sts2.Core.Models;
using BaseLib.Utils;

namespace CeremonialBeast.CeremonialBeastCode.Cards;
[Pool(typeof(MegaCrit.Sts2.Core.Models.CardPools.TokenCardPool))]

public sealed class Rampage : CeremonialBeastCard
{
    public override bool GainsBlock => false;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override RingingBehavior RingingInteractBehavior => RingingBehavior.Require;
    protected override bool UsesPlowAnimationWhileRinging => true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromPower<CeremonialBeast.CeremonialBeastCode.Powers.RingingPower>(),
        HoverTipFactory.FromPower<CeremonialBeast.CeremonialBeastCode.Powers.PlowPower>(),
        HoverTipFactory.FromPower<PlatingPower>() 
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(20m, ValueProp.Move),
        new PowerVar<CeremonialBeast.CeremonialBeastCode.Powers.PlowPower>(2m),
        new PowerVar<PlatingPower>(4m) 
    ];

    public Rampage()
        : base(1, CardType.Attack, CardRarity.Token, TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var targets = base.CombatState!.HittableEnemies;

        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(
            NHorizontalLinesVfx.Create(new Color("BFFFC880"), 1.2000000476837158, movingRightwards: true));

        await Cmd.Wait(0.5f);

        NCombatRoom.Instance?.RadialBlur(VfxPosition.Left);
        VfxCmd.PlayOnCreatureCenters(targets, "vfx/vfx_attack_blunt");

        var firstTarget = targets.FirstOrDefault();
        if (firstTarget != null)
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NLineBurstVfx.Create(firstTarget));
        }

        NGame.Instance?.ScreenShake(
            ShakeStrength.Strong,
            ShakeDuration.Normal,
            180f + Rng.Chaotic.NextFloat(-10f, 10f));

        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithNoAttackerAnim()
            .TargetingAllOpponents(base.CombatState!)
            .Execute(choiceContext);

        NGame.Instance?.DoHitStop(ShakeStrength.Strong, ShakeDuration.Normal);

        await PowerCmd.Apply<CeremonialBeast.CeremonialBeastCode.Powers.PlowPower>(base.Owner.Creature!, base.DynamicVars["PlowPower"].BaseValue, base.Owner.Creature, this);

        await PowerCmd.Apply<PlatingPower>(
            base.Owner.Creature!, 
            base.DynamicVars["PlatingPower"].BaseValue, 
            base.Owner.Creature, 
            this
        );

    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
        DynamicVars["PlowPower"].UpgradeValueBy(1m);
        DynamicVars[nameof(PlatingPower)].UpgradeValueBy(1m);
    }

    public static async Task<CardModel?> CreateInHand(Player owner, ICombatState combatState, bool upgraded = false)
    {
        return (await CreateInHand(owner, 1, combatState, upgraded)).FirstOrDefault();
    }

    public static async Task<IEnumerable<CardModel>> CreateInHand(Player owner, int count, ICombatState combatState, bool upgraded = false)
    {
        if (count <= 0 || CombatManager.Instance.IsOverOrEnding) return Array.Empty<CardModel>();

        List<CardModel> generatedCards = new List<CardModel>();
        for (int i = 0; i < count; i++)
        {
            CardModel newCard = combatState.CreateCard<Rampage>(owner);
            if (upgraded) CardCmd.Upgrade(newCard);
            generatedCards.Add(newCard);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(generatedCards, PileType.Hand, owner);
        return generatedCards;
    }

    public static async Task<CardModel?> CreateInDrawPile(Player owner, ICombatState combatState, bool upgraded = false)
    {
        return (await CreateInDrawPile(owner, 1, combatState, upgraded)).FirstOrDefault();
    }

    public static async Task<IEnumerable<CardModel>> CreateInDrawPile(Player owner, int count, ICombatState combatState, bool upgraded = false)
    {
        if (count <= 0 || CombatManager.Instance.IsOverOrEnding) return Array.Empty<CardModel>();

        List<CardModel> generatedCards = new List<CardModel>();
        for (int i = 0; i < count; i++)
        {
            CardModel newCard = combatState.CreateCard<Rampage>(owner);
            if (upgraded) CardCmd.Upgrade(newCard);
            generatedCards.Add(newCard);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(generatedCards, PileType.Draw, owner);
        return generatedCards;
    }
}
