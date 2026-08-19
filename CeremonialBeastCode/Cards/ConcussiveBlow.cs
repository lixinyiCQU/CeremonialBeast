using System.Collections.Generic;
using System.Linq; // 必须引入 LINQ 用于历史记录查询
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.MonsterMoves.Intents; // 用于获取击晕的悬停提示
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class ConcussiveBlow : CeremonialBeastCard
{
    // 注册 消耗(Exhaust) 关键字
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] 
    { 
        CardKeyword.Exhaust 
    };

    // 注册 击晕(Stun) 的悬停提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        StunIntent.GetStaticHoverTip()
    };

    // 注册伤害变量，初始为 10
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(12m, ValueProp.Move)
    };

    public ConcussiveBlow()
        : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 执行攻击指令
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);

        // 2. 查询战斗历史记录，获取这张牌刚刚造成的伤害数据
        var damageEntry = CombatManager.Instance.History.Entries
            .OfType<DamageReceivedEntry>()
            .LastOrDefault(e => e.HappenedThisTurn(base.CombatState) && e.CardSource == this);

        // 3. 判断未被格挡的伤害是否达到或超过 30 点，并且敌人还没有死
        if (damageEntry != null && damageEntry.Result.UnblockedDamage >= 30m && !cardPlay.Target!.IsDead)
        {
            // 满足条件，追加击晕指令
            await CreatureCmd.Stun(cardPlay.Target!);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后伤害提升 4 点 (12 -> 16)
        base.DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
