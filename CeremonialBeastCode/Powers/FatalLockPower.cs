using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public sealed class FatalLockPower : CustomPowerModel
{
    public decimal Multiplier { get; set; } = 2m;

    public override PowerType Type => PowerType.Buff;

    // ==========================================
    // ✨ 核心修复：将 Counter 替换为 None！
    // 只有 None 能够真正向玩家隐藏数字，并让底层引擎老老实实地保留 2.5m 的小数精度！
    // ==========================================
    public override PowerStackType StackType => PowerStackType.None;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        // 1. 必须是受到力量加成的物理攻击伤害
        if (!props.IsPoweredAttack())
        {
            return 1m;
        }

        // 2. 伤害必须来源于卡牌
        if (cardSource == null)
        {
            return 1m;
        }

        // 3. 伤害来源必须是当前挂载状态的玩家本体
        if (dealer != base.Owner)
        {
            return 1m;
        }

        // 4. 卡牌类型必须是“攻击牌 (Attack)”
        if (cardSource.Type != CardType.Attack)
        {
            return 1m;
        }

        // 所有条件满足，返回精准的乘区倍率（未升级返回 2m，升级后完美返回 2.5m）
        return Multiplier;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 确保打出的是攻击牌，并且是玩家自己打出的
        if (cardPlay.Card.Type == CardType.Attack && cardPlay.Card.Owner.Creature == base.Owner)
        {
            await PowerCmd.Remove(this);
        }
    }
}
