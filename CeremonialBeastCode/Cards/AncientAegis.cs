using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class AncientAegis : CeremonialBeastCard
{
    // 注册格挡变量
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(18m, ValueProp.Move)
    ];

    public AncientAegis()
        : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 修复1：去掉 .BaseValue，直接将 base.DynamicVars.Block (即 BlockVar 对象) 喂给指令
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        // 修复2：调用专门的战斗级减费 API，引擎会自动处理后续的边界情况
        base.EnergyCost.AddThisCombat(-1);
    }

    protected override void OnUpgrade()
    {
        // 升级后格挡增加 4 点 (18 -> 22)
        base.DynamicVars.Block.UpgradeValueBy(4m);
    }
}