using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class SoulSiphon() : CeremonialBeastCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override bool CanBeGeneratedInCombat => false;

    // 声明变量：基础伤害 9，基础回复 2
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(9m, ValueProp.Move),
        new HealVar(3m)
    ];

    // 关键：注册悬停提示黑框，这样玩家把鼠标放在牌上时，会解释“斩杀”是什么意思
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Fatal)];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (play.Target != null)
        {
            // 1. 防逃课校验：检查目标身上是否有“死亡不触发斩杀”的状态（例如某些特殊召唤物或黑暗史莱姆）
            bool shouldTriggerFatal = play.Target.Powers.All((PowerModel p) => p.ShouldOwnerDeathTriggerFatal());

            // 2. 执行攻击指令，并将这个指令的执行结果存入 attackCommand 变量中
            AttackCommand attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
                .Targeting(play.Target)
                // 借用原版吞噬的咬噬特效和音效，非常符合“吞食”的主题
                .Execute(choiceContext);

            // 3. 结算斩杀：如果目标允许触发斩杀，并且刚才的攻击结果中显示目标已被击杀
            if (shouldTriggerFatal && attackCommand.Results.SelectMany(r => r).Any(r => r.WasTargetKilled))
            {
                // 执行生命回复
                await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后伤害提升 3 (9 -> 12)
        DynamicVars.Damage.UpgradeValueBy(3m);
        // 回复提升 1 (2 -> 3)
        DynamicVars.Heal.UpgradeValueBy(1m);
    }
}
