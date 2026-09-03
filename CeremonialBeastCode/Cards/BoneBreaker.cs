using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers; // 必须引入以获取 StrengthPower
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class BoneBreaker() : CeremonialBeastCard(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    // 1. 定义动态变量：伤害 4，力量减少量 1
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(4, ValueProp.Move),
        new PowerVar<StrengthPower>(1) // PowerVar 会以 Power 的类名为键名
    ];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 第一步：攻击敌人
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target!)
            .Execute(choiceContext);
            
        // 第二步：减少敌人力量
        await PowerCmd.Apply<StrengthPower>(
            play.Target!, 
            -DynamicVars[nameof(StrengthPower)].BaseValue, // 取变量的负值
            Owner.Creature, 
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级提升伤害 2 点 (4 -> 6)
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars[nameof(StrengthPower)].UpgradeValueBy(1m);
    }
}
