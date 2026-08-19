using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers; // 必须引入，以使用官方的下回合状态
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class SwiftHooves() : CeremonialBeastCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // 定义动态变量：3点伤害，1点能量，1张牌
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3m, ValueProp.Move),
        new EnergyVar(1),
        new CardsVar(1)
    ];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (play.Target != null)
        {
            // 1. 结算即时伤害
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
                .Targeting(play.Target)
                .Execute(choiceContext);

            // 2. 挂载延迟状态：下回合获得能量
            // 注意：EnergyVar 比较特殊，它的值必须通过 IntValue 取出
            await PowerCmd.Apply<EnergyNextTurnPower>(
                Owner.Creature, 
                DynamicVars.Energy.IntValue, 
                Owner.Creature, 
                this
            );

            // 3. 挂载延迟状态：下回合抽牌
            await PowerCmd.Apply<DrawCardsNextTurnPower>(
                Owner.Creature, 
                DynamicVars.Cards.BaseValue, 
                Owner.Creature, 
                this
            );
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后伤害提升 3 点 (变为 6 点)
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
