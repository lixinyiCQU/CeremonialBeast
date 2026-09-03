using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public class ResoundingEchoPower : CeremonialBeastPower
{
    public override PowerType Type => PowerType.Buff;
    
    public override PowerStackType StackType => PowerStackType.Counter;

    // ==========================================
    // ✨ 完美契合官方 OutbreakPower 的监听机制
    // ==========================================
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // 1. 确保是玩家自己（base.Owner）身上的状态发生变动
        // 2. 确保状态是 RingingPower
        // 3. 确保是在“获得”状态（即使 Ringing 没有层数，底层在挂载时依然会传入 > 0 的 amount 广播）
        if (power.Owner == base.Owner && power is RingingPower && amount > 0m)
        {
            var enemies = base.CombatState.HittableEnemies;

            if (enemies.Any())
            {
                // 触发遗物/状态图标闪烁
                Flash(); 

                // ✨ 核心机制：使用官方专属的 ThrowingPlayerChoiceContext 解决无上下文报错的痛点！
                await CreatureCmd.Damage(
                    choiceContext, 
                    enemies, 
                    base.Amount, 
                    ValueProp.Unpowered, 
                    base.Owner, 
                    null,
                    null
                );
            }
        }
    }
}
