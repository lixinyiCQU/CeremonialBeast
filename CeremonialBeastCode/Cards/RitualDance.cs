using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class RitualDance : CeremonialBeastCard
{
    // 自定义额外格挡的变量 Key
    private const string _extraBlock = "ExtraBlock";

    public override bool GainsBlock => true;

    // 注册基础格挡 (BlockVar) 和额外格挡 (DynamicVar)
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(7m, ValueProp.Move),
        new BlockVar(_extraBlock, 7m, ValueProp.Move)
    };

    public RitualDance()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    // ==========================================
    // ✨ 核心机制：前置条件判定（使用原方案）
    // ==========================================
    private bool IsFirstCardPlayThisTurn
    {
        get
        {
            // 防空保护：如果在图鉴、主菜单等非战斗场景下，直接返回 false 不触发高亮
            if (base.Owner == null || CombatManager.Instance?.History == null) 
            {
                return false;
            }

            // 使用原方案全局单例 CombatManager 获取出牌历史
            return CombatManager.Instance.History.CardPlaysFinished.Count(
                (CardPlayFinishedEntry e) => e.CardPlay.Card.Owner == base.Owner && e.HappenedThisTurn(base.CombatState)
            ) == 0;
        }
    }

    // ==========================================
    // ✨ UI 交互：接管金边高亮提示
    // ==========================================
    protected override bool ShouldGlowGoldInternal => IsFirstCardPlayThisTurn;

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 无条件获得基础格挡
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        // 2. 如果满足条件（UI 上此时卡牌正亮着金边），给予额外格挡
        if (IsFirstCardPlayThisTurn)
        {
            // 利用当前 DynamicVar 的数值，临时构造一个 BlockVar 传递给指令
            BlockVar extraBlock = (BlockVar)base.DynamicVars[_extraBlock];
            await CreatureCmd.GainBlock(base.Owner.Creature, extraBlock, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后，基础和额外格挡均提升至 9 点（即各增加 2 点）
        base.DynamicVars.Block.UpgradeValueBy(2m);
        base.DynamicVars[_extraBlock].UpgradeValueBy(2m);
    }
}
