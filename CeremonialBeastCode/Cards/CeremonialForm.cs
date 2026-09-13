using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class CeremonialForm : CeremonialBeastCard
{
    // 注册悬停提示：展示 Ringing 状态说明 和《横冲直撞》的卡牌预览
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<RingingPower>(),
        HoverTipFactory.FromCard<Plow>(base.IsUpgraded)
    ];

    public CeremonialForm()
        : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

        // 1. 施加《仪式形态》状态
        await PowerCmd.Apply<CeremonialFormPower>(base.Owner.Creature, 1m, base.Owner.Creature, this);

        // 2. 💡 核心赋值逻辑：找到刚挂上去的状态，把升级标记传给它！
        var appliedPower = base.Owner.Creature.GetPower<CeremonialFormPower>();
        if (appliedPower != null)
        {
            if (base.IsUpgraded)
            {
                appliedPower.UpgradedStacks++;
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 形态牌的升级逻辑：通过设置 isUpgraded，让每回合给的衍生卡变成升级版
        // 底层数值本身不需要变化，因为机制在 Power 里处理了
    }
}
