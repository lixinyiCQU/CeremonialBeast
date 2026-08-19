using System.Collections.Generic;
using System.Threading.Tasks;
using CeremonialBeast.CeremonialBeastCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class BeastGrace : CeremonialBeastCard
{
    public override RingingBehavior RingingInteractBehavior => RingingBehavior.Require;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<RingingPower>(),
        StunIntent.GetStaticHoverTip()
    ];

    public BeastGrace()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (Creature enemy in base.CombatState!.HittableEnemies)
        {
            await CreatureCmd.Stun(enemy);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
