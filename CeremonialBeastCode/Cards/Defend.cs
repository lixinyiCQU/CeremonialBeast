using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

// 1费, 技能牌, 基础稀有度, 目标为自身
public class Defend() : CeremonialBeastCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    // 1. 定义基础格挡变量：基础 5 点格挡
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];

    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Defend };
    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 直接指令，不需要 .Execute()
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }

    // 3. 卡牌升级逻辑
    protected override void OnUpgrade()
    {
        // 格挡提升 3 点 (从 5 提升至 8)
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}