using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using CeremonialBeast.CeremonialBeastCode.Powers;
using MegaCrit.Sts2.Core.Models;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class HarvestFestival : CeremonialBeastCard
{
    // 1. 声明为 X 费卡牌
    protected override bool HasEnergyCostX => true;

    // 注册悬停提示：展示《仪式犁地》和下回合抽牌的状态说明
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard(ModelDb.Card<CeremonialPlowing>()), 
        HoverTipFactory.FromPower<HarvestFestivalPower>()
    ];

    public HarvestFestival()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 2. 解析 X 的数值 
        int x = ResolveEnergyXValue();

        // 3. 处理升级逻辑：X + 1
        int finalAmount = base.IsUpgraded ? x + 1 : x;

        if (finalAmount > 0)
        {
            // ✨ 核心修复：使用官方 AddToCombatAndPreview 泛型方法
            // 它会自动完成：生成卡牌 -> 插入抽牌堆的随机位置（洗入） -> 播放生成动画 -> 刷新 UI 数量！
            await CardPileCmd.AddToCombatAndPreview<CeremonialPlowing>(
                base.Owner.Creature, 
                PileType.Draw, 
                finalAmount, 
                creator: base.Owner, 
                position: CardPilePosition.Random // 随机位置插入，即实现了“洗入”
            );

            // B. 挂载“下回合抽牌”的状态
            await PowerCmd.Apply<HarvestFestivalPower>(
                base.Owner.Creature, 
                (decimal)finalAmount, 
                base.Owner.Creature, 
                this
            );
        }
    }

    protected override void OnUpgrade()
    {
        // 升级逻辑已在 OnPlayCard 的 finalAmount 中处理
    }
}
