using System;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

// ==========================================
// 升级前的状态：下 N 张牌减 1 费
// ==========================================
public sealed class CeremonialPrepPower : CeremonialBeastPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // ✨ 核心修复 1：使用官方专属的费用覆盖机制
    // 它只会在引擎“询问”卡牌费用时动态减费，绝对不会污染卡牌底层的真实费用（完美解决 Retain 和临时升级 Bug）！
    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        
        // 确保只修改状态拥有者的卡牌（也顺便解决了对其他牌不起作用的 Bug）
        if (card.Owner?.Creature != base.Owner)
        {
            return false;
        }

        // 严谨起见，只对拿在手里或正在打出区的牌进行减费
        bool inValidPile = false;
        switch (card.Pile?.Type)
        {
            case PileType.Hand:
            case PileType.Play:
                inValidPile = true;
                break;
        }

        if (inValidPile)
        {
            // 减费 1 点，并用 Math.Max 保底，防止费用变成负数导致引擎报错
            modifiedCost = Math.Max(0m, originalCost - 1m);
            return true;
        }

        return false;
    }

    // ✨ 核心修复 2：将消耗层数的时机提前到 BeforeCardPlayed！
    // 因为你的 CeremonialPrep 是在 OnPlayCard（打出中）挂载此状态的。
    // 此时 BeforeCardPlayed（打出前）已经彻底执行完毕了！
    // 所以它完美地跳过了“自己消耗自己”的逻辑漏洞！
    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature == base.Owner)
        {
            // 下一张牌一旦确认打出并扣除能量，就立刻消耗一层状态
            await PowerCmd.Decrement(this);
        }
    }
}

// ==========================================
// 升级后的状态：下 N 张牌减 2 费
// ==========================================
public sealed class CeremonialPrepUpgradedPower : CeremonialBeastPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        
        if (card.Owner?.Creature != base.Owner) return false;

        bool inValidPile = false;
        switch (card.Pile?.Type)
        {
            case PileType.Hand:
            case PileType.Play:
                inValidPile = true;
                break;
        }

        if (inValidPile)
        {
            // 升级版：减费 2 点
            modifiedCost = Math.Max(0m, originalCost - 2m);
            return true;
        }

        return false;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature == base.Owner)
        {
            await PowerCmd.Decrement(this);
        }
    }
}
