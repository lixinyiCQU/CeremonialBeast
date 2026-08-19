using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class EarthenShield : CeremonialBeastCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];
    // 告知引擎这张牌会产生格挡
    public override bool GainsBlock => true;

    // 注册 Plow 词条的悬停提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<PlowPower>()
    };

    // 使用 BlockVar 注册“每层获得的格挡数”，初始为 3
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(3m, ValueProp.Move)
    };

    public EarthenShield()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 获取当前 Plow 层数
        decimal plowCount = base.Owner.Creature.GetPowerAmount<PlowPower>();

        if (plowCount > 0)
        {
            // 2. 计算总格挡：层数 * 每层格挡数值
            // 我们读取 BlockVar 的 BaseValue，确保它能随升级从 3 变为 4
            decimal blockPerLayer = base.DynamicVars.Block.BaseValue;
            decimal totalBlock = plowCount * blockPerLayer;

            // 3. 获得格挡
            // 注意：因为我们是手动计算的数值，直接传入 totalBlock。
            // 使用 ValueProp.Move 确保这部分格挡依然可以吃到敏捷（Dexterity）的加成
            await CreatureCmd.GainBlock(base.Owner.Creature, totalBlock, ValueProp.Move, cardPlay);

            // 4. 失去所有 Plow
            // 根据 STS2 惯例，直接调用 Remove 指令移除该状态
            await PowerCmd.Remove<PlowPower>(base.Owner.Creature);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后每失去 1 层获得的格挡提升 1 点 (3 -> 4)
        base.DynamicVars.Block.UpgradeValueBy(1m);
    }
}