using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using CeremonialBeast.CeremonialBeastCode.Powers;
using MegaCrit.Sts2.Core.Models;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class BreakingChains : CeremonialBeastCard
{
    // 注册悬停提示：自动在侧边展示《NOPE》的卡牌预览
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard(ModelDb.Card<Nope>())
    ];

    public BreakingChains()
        : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 播放施法动画
        

        // 挂载《挣脱枷锁》状态，给予 1 层（即每回合 1 张 NOPE）
        await PowerCmd.Apply<BreakingChainsPower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级后费用从 2 降至 1
        // 💡 提示：如果此方法由于底层 API 变动报错，请尝试寻找诸如 UpgradeBaseCost(1) 或修改 base.EnergyCost.BaseValue 的官方属性
        base.EnergyCost.UpgradeBy(-1);
    }
}