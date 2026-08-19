using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class FlurryOfHooves() : CeremonialBeastCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // 注册“保留 (Retain)”关键字
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    // 定义两个动态变量：伤害值为 3，攻击次数(Repeat)为 3
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3, ValueProp.Move),
        new RepeatVar(3)
    ];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 目标安全校验
        if (play.Target != null)
        {
            // 优雅的链式攻击指令
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .WithHitCount(DynamicVars.Repeat.IntValue) // 传入攻击次数
            .FromCard(this, play)
                .Targeting(play.Target)
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后，攻击次数增加 1 次 (变为 4 次)
        DynamicVars.Repeat.UpgradeValueBy(1m);
    }
}
