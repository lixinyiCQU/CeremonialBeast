using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using CeremonialBeast.CeremonialBeastCode.Powers;
// 💡 引入附魔命名空间（由于目前使用占位符，编译时请确保占位符类存在或先注释掉悬停提示）
using CeremonialBeast.CeremonialBeastCode.Enchantments;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class AncientBlessing : CeremonialBeastCard
{
    // 架构接入：声明为 Dynamic 行为，确保无视鸣响封印
    public override RingingBehavior RingingInteractBehavior => RingingBehavior.Dynamic;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromEnchantment<BlessingEnchantment>();

    public AncientBlessing()
        : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) // 初始 2 费，稀有能力
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 挂载远古神恩状态，默认 1 层（每回合附魔 1 张）
        await PowerCmd.Apply<AncientBlessingPower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级后费用降低至 1 费
        EnergyCost.UpgradeBy(-1);
    }
}