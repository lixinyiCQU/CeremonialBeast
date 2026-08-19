using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class Reverberance() : CeremonialBeastCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    // 1. 定义伤害变量：基础 12 点
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12m, ValueProp.Move)];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 攻击逻辑
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target!)
            .Execute(choiceContext);

        // 施加我们的自定义状态：下一张 Ringing 牌免费
        await PowerCmd.Apply<FreeRingingPower>(Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 伤害提升 4 点 (12 -> 16)
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
