using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class Digging() : CeremonialBeastCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // 1. 定义动态变量：抽 1 张牌，获得 2 层 Plow
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new CardsVar(2),
        new PowerVar<PlowPower>(1m)
    ];

    // 2. 注册悬停提示：当玩家鼠标悬停时，弹出 Plow 状态的说明
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromPower<PlowPower>()
    ];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 第一步：获得 Plow 状态
        await PowerCmd.Apply<PlowPower>(
            Owner.Creature, 
            DynamicVars[nameof(PlowPower)].BaseValue, 
            Owner.Creature, 
            this
        );

        // 第二步：执行抽牌指令
        // 注意：参考官方源码，Draw 方法的第三个参数直接传入 Owner (Player实体)，而不是 Owner.Creature
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(PlowPower)].UpgradeValueBy(1m);
    }
}
