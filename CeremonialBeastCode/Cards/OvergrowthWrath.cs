using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class OvergrowthWrath() : CeremonialBeastCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    // ✨ 核心机制 1：注册“消耗”关键字，引擎会自动处理打出后进入消耗堆的逻辑
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    // 悬停提示：展示 Plow 的解释黑框
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PlowPower>()];

    // 声明变量：基础 7 点伤害
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(7m, ValueProp.Move)
    ];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (play.Target != null)
        {
            // 1. 结算基础物理伤害
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
                .Targeting(play.Target)
                .Execute(choiceContext);

            // 2. 核心翻倍逻辑：获取玩家自己当前的 Plow 层数
            int currentPlowStacks = base.Owner.Creature.GetPowerAmount<PlowPower>();
            
            // 3. 如果有 Plow 层数，则再赋予等量的层数 (相当于翻倍)
            if (currentPlowStacks > 0)
            {
                await PowerCmd.Apply<PlowPower>(
                    base.Owner.Creature, 
                    currentPlowStacks, 
                    base.Owner.Creature, 
                    this
                );
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后伤害提升 3 点 (7 -> 10)
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
