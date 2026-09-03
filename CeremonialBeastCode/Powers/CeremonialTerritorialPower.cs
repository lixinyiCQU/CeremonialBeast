using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models; // ⬅️ 修复：引入了 CardModel 所在的命名空间

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public class CeremonialTerritorialPower : CeremonialBeastPower
{
    public override PowerType Type => PowerType.Buff;
    
    // 使用 Counter 显示层数。如果你打了两张领地意识，挨打一次就会抽 2 张牌。
    public override PowerStackType StackType => PowerStackType.Counter;

    // 核心监听钩子：受到伤害后触发
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 校验条件：
        // 1. 挨打的是玩家自己
        // 2. 实际流失的血量（无视格挡后造成的真实伤害）必须大于 0
        if (target == base.Owner && result.UnblockedDamage > 0)
        {
            Flash(); // 头顶闪烁状态图标
            
            // 安全调用抽牌指令
            if (base.Owner.Player != null)
            {
                await CardPileCmd.Draw(choiceContext, base.Amount, base.Owner.Player);
            }
        }
    }
}
