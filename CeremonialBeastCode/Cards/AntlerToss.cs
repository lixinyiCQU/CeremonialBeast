using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers; // 必须引入以使用 WeakPower
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class AntlerToss() : CeremonialBeastCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // 悬停提示：展示“虚弱”的官方解释黑框
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<WeakPower>()];

    // 声明变量：12点伤害，2层虚弱
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(12m, ValueProp.Move),
        new PowerVar<WeakPower>(2m)
    ];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (play.Target != null)
        {
            // 1. 结算高额物理伤害
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
                .Targeting(play.Target)
                // 使用重型打击特效和音效，表现出被角顶飞的力度
                .Execute(choiceContext);

            // 2. 施加虚弱状态
            await PowerCmd.Apply<WeakPower>(
                play.Target, 
                DynamicVars["WeakPower"].BaseValue, 
                base.Owner.Creature, 
                this
            );
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后伤害提升 2 点 (12 -> 14)
        DynamicVars.Damage.UpgradeValueBy(2m);
        // 升级后虚弱提升 1 层 (2 -> 3)
        DynamicVars["WeakPower"].UpgradeValueBy(1m);
    }
}
