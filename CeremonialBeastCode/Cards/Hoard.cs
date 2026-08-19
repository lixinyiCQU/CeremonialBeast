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

public class Hoard() : CeremonialBeastCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // 提供悬停时“保留”关键字的黑框解释
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Retain)];

    // 声明动态变量：伤害与保留数量
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(8m, ValueProp.Move),
        new DynamicVar("RetainAmount", 2m)
    ];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (play.Target != null)
        {
            // 1. 结算伤害
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
                .Targeting(play.Target)
                .Execute(choiceContext);

            // 2. 挂载我们重写后的保留状态
            await PowerCmd.Apply<CeremonialHoardPower>(
                Owner.Creature, 
                DynamicVars["RetainAmount"].BaseValue, 
                Owner.Creature, 
                this
            );
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["RetainAmount"].UpgradeValueBy(1m);
    }
}
