using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries; // ✨ 核心修复：引入历史记录实体类型
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class VengefulCounter : CeremonialBeastCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(17m, ValueProp.Move)
    ];

    public VengefulCounter()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        await base.AfterCardDrawn(choiceContext, card, fromHandDraw);
        if (card == this)
        {
            ApplyCostReductionIfNeeded();
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await base.AfterPlayerTurnStart(choiceContext, player);
        if (player == base.Owner)
        {
            ApplyCostReductionIfNeeded();
        }
    }

    private void ApplyCostReductionIfNeeded()
    {
        // 💡 核心机制：跨回合历史记录查询的终极形态！
        // 1. 调用底层的 Entries 集合
        // 2. 筛选出“受伤记录” (DamageReceivedEntry)
        // 3. 判断受击者是自己 (Actor)，发生在上一回合，且实际掉血大于 0
        bool tookDamageLastTurn = CombatManager.Instance.History.Entries
            .OfType<DamageReceivedEntry>()
            .Any(e => e.Actor == base.Owner.Creature && 
                      e.HappenedLastPlayerTurn(base.Owner) && 
                      e.Result.UnblockedDamage > 0);

        if (tookDamageLastTurn)
        {
            base.EnergyCost.SetThisTurn(0);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(5m);
    }
}
