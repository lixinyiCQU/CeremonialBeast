using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class SacrilegiousCeremony : CeremonialBeastCard
{
    // ==========================================
    // 💡 终极优化：自我豁免声明！
    // ==========================================
    // 声明为 Exempt，确保这张牌自身可以无视鸣响的封印被打出，
    // 否则就会陷入“需要打出它来解封，但因为被封印所以打不出它”的死循环。
    public override RingingBehavior RingingInteractBehavior => RingingBehavior.Exempt;

    // 注册 Ringing 词条的悬停提示，方便玩家理解规则
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<RingingPower>()
    ];

    public SacrilegiousCeremony()
        : base(3, CardType.Power, CardRarity.Ancient, TargetType.Self) // 初始 3 费，先古稀有度
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 💡 视觉优化：作为“先古”卡牌，打出时加入屏幕震动，拉满视觉张力！
        
        
        // 挂载亵渎仪式状态（标记型 Power）
        await PowerCmd.Apply<SacrilegiousCeremonyPower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}