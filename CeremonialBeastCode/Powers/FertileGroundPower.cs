using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public sealed class FertileGroundPower : CeremonialBeastPower
{
    public override PowerType Type => PowerType.Buff;
    
    // 使用 Counter 类型。这样如果玩家打出多张“沃土”，层数会叠加，每次额外获得的 Plow 也会随之变多
    public override PowerStackType StackType => PowerStackType.Counter;

    // 神级 API：直接拦截并修改即将获得的层数
    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        // 1. 核心判定逻辑保持不变
        if (canonicalPower is PlowPower && target == base.Owner && amount > 0)
        {
            // 触发特效
            Flash();

            // 2. 将修改后的数值赋值给 out 参数
            // 这里的 base.Amount 是 FertileGroundPower 自身的层数
            modifiedAmount = amount + base.Amount;

            // 3. 返回 true，告诉引擎：“我修改了这个数值，请使用 modifiedAmount”
            return true;
        }
        // 4. 如果不满足条件，必须给 out 参数赋一个默认值（通常是原始 amount）
        // 并返回 false，告诉引擎：“我没动这个数值，按原来的来”
        return base.TryModifyPowerAmountReceived(canonicalPower, target, amount, applier, out modifiedAmount);
    }
}
