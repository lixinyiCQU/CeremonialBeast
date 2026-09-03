using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Creatures;
using BaseLib.Hooks; // 可能需要引入以识别 Creature
// 注意：你可能需要让 IDE 自动引入 IHealAmountModifier 所在的命名空间 (例如 BaseLib.Hooks)

namespace CeremonialBeast.CeremonialBeastCode.Powers;

// 核心改造：在继承 CustomPowerModel 的同时，实现 IHealAmountModifier 接口
public class SoulOfWhiteStagPower : CeremonialBeastPower, IHealAmountModifier
{
    public override PowerType Type => PowerType.Buff;
    
    public override PowerStackType StackType => PowerStackType.Counter;

    // --- 接口实现区 ---

    // 1. 乘区修正 (Multiplicative)
    // 这里的参数列表 (Creature target, decimal amount) 是最常见的形式，具体请以你的 IDE 自动补全为准
    public decimal ModifyHealMultiplicative(Creature target, decimal amount)
    {
        // 校验：只增幅玩家自己的回复效果
        if (target != base.Owner)
        {
            return 1m; // 社区提示：返回 1m 代表中立（乘数为 1，不改变原奶量）
        }

        // 计算逻辑：基础乘数 1m + (提升百分比 / 100m)
        // 例如：打出一张牌，Amount 是 50，则返回 1m + 0.5m = 1.5m (提升 50%)
        return 1m + ((decimal)base.Amount / 100m);
    }

    // 2. 加区修正 (Additive)
    // 因为接口通常要求必须实现所有声明的方法，即使我们不需要固定加成，也要写出来。
    public decimal ModifyHealAdditive(Creature target, decimal amount)
    {
        // 加法计算的中立值是 0m（即不额外增加固定血量）
        return 0m; 
    }
}
