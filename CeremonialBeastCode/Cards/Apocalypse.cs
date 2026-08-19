using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class Apocalypse() : CeremonialBeastCard(4, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    // 声明变量：极高的基础伤害 38 点
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(38m, ValueProp.Move)
    ];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 执行群体毁灭攻击
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .TargetingAllOpponents(base.CombatState!)
            // 使用最重型的打击特效和音效
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // 升级后伤害大幅提升 10 点 (38 -> 48)
        DynamicVars.Damage.UpgradeValueBy(10m);
    }
}
