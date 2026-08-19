using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers; 
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public class StunMarkPower : CustomPowerModel
{
    // 定位为负面状态 (Debuff)
    public override PowerType Type => PowerType.Debuff;
    
    // 这是一个可以叠加的印记，所以使用 Counter
    public override PowerStackType StackType => PowerStackType.Counter; 

    // ==========================================
    // 任务 1 & 2：层数监控与触发眩晕
    // ==========================================
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // 惯例：先执行底层逻辑，把所有参数原封不动传给 base
        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);

        // 获取当前的昏眩印记层数（此时 Owner 就是被挂上印记的敌人）
        int currentStacks = Owner.GetPowerAmount<StunMarkPower>();
        
        if (currentStacks >= 3)
        {
            // 1. 消耗 3 层印记（传入 -3 扣除层数）
            await PowerCmd.Apply<StunMarkPower>([Owner], -3, Owner, null);

            // 2. 呼叫官方接口，直接击晕该敌人！
            await CreatureCmd.Stun(Owner);
        }
    }
}
