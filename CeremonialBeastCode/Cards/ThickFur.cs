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

public class ThickFur() : CeremonialBeastCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // 1. 核心规范：声明这张牌提供格挡。
    // 如果没有这一行，卡牌将无法受到“敏捷”属性加成，也无法受到“脆弱 (Frail)”状态的减免！
    public override bool GainsBlock => true;

    // 2. 定义变量：7 点格挡，1 层 Plow
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new BlockVar(8m, ValueProp.Move),
        new PowerVar<PlowPower>(1m)
    ];

    // 3. 注册悬停提示：玩家把鼠标放在卡牌上时，旁边会弹出 Plow 状态的说明面板
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromPower<PlowPower>()
    ];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 获得格挡指令
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        
        // 获得 Plow 状态
        await PowerCmd.Apply<PlowPower>(
            Owner.Creature, 
            DynamicVars[nameof(PlowPower)].BaseValue, 
            Owner.Creature, 
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级后格挡提升至 11；Plow 层数不变。
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
