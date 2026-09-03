using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class EndurePain : CeremonialBeastCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<CeremonialBeast.CeremonialBeastCode.Powers.PlowPower>()
    ];

    private const string _plowKey = "PlowAmount";

    // 告诉底层这会产生格挡（用于意图显示等）
    public override bool GainsBlock => true;

    // 注册格挡值 (13点) 和 Plow层数 (1层)
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(13m, ValueProp.Move),
        new DynamicVar(_plowKey, 1m)
    };

    public EndurePain()
        : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 获得格挡
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        // 2. 挂载本回合专属的“痛苦忍耐”状态，层数传入我们定义的 PlowAmount (即 1m)
        await PowerCmd.Apply<EndurePainPower>(base.Owner.Creature, base.DynamicVars[_plowKey].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级后格挡增加 3 点 (13 -> 16)
        base.DynamicVars.Block.UpgradeValueBy(3m);
        
        // ✨ 修复：直接传入 _plowKey 变量本身即可，去掉 nameof()
        base.DynamicVars[_plowKey].UpgradeValueBy(1m);
    }
}
