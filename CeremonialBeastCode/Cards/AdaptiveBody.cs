using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class AdaptiveBody : CeremonialBeastCard
{
    // 注册悬停提示，方便玩家查看 Plow 词条
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PlowPower>()
    ];

    public AdaptiveBody()
        : base(3, CardType.Power, CardRarity.Rare, TargetType.Self) // 费用 3，稀有能力牌
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 播放施法动画
        
        
        // 挂载适应躯体状态（传 1m 即可，因为层数会被隐藏）
        await PowerCmd.Apply<AdaptiveBodyPower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级后费用降低 1 点 (3 -> 2)
        base.EnergyCost.UpgradeBy(-1);
    }
}