using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class ResoluteAssault() : CeremonialBeastCard(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // 关键属性声明：强制告诉引擎这张攻击牌含有格挡效果，使其正常受到敏捷（Dexterity）和脆弱（Frail）的加成
    public override bool GainsBlock => true;

    // 声明伤害和格挡变量，基础值均为 4
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(4m, ValueProp.Move),
        new BlockVar(4m, ValueProp.Move)
    ];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (play.Target != null)
        {
            // 1. 结算格挡：调用底层 GainBlock 指令，传入封装好的 BlockVar
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
            
            // 2. 结算伤害：链式攻击指令
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
                .Targeting(play.Target)
                // 借用原版铁斩波的飞斩特效，非常有突击感
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后，伤害和格挡各自提升 2 点 (变为 6/6)
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}
