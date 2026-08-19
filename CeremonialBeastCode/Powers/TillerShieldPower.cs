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

public sealed class TillerShieldPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    
    // 使用 Counter 类型，允许多次打出卡牌时，每层 Plow 提供的格挡额度叠加
    public override PowerStackType StackType => PowerStackType.Counter;

    // 采用你在截图中定位到的最新 API 钩子
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // 核心判定：
        // 1. 发生改变的状态必须是 PlowPower
        // 2. 该状态的所有者必须是你自己 (power.Owner == base.Owner)
        // 3. 必须是获得状态 (amount > 0)
        if (power is PlowPower && power.Owner == base.Owner && amount > 0)
        {
            // 触发能力闪烁特效
            Flash();

            // 计算格挡：获得的 Plow 层数 (amount) * 每层应得格挡 (base.Amount)
            // 按照你在开发日志阶段二记录的经验，在 Power 中使用纯数值格挡重载调用
            await CreatureCmd.GainBlock(base.Owner, amount * base.Amount, ValueProp.Unpowered, null);
        }

        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
    }
}
