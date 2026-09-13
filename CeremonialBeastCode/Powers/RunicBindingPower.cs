using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils; 
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public class RunicBindingPower : CeremonialBeastPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // ==========================================
    // ✨ 为每个怪物独立开设的“力量欠条”
    // 记录它们在本回合被扣除了多少力量，防止归还时出错
    // ==========================================
    public static readonly SpireField<Creature, decimal> StrengthLostThisTurn = new(() => 0m);

    // ==========================================
    // 钩子 1：打出附魔牌时，真实扣除所有敌人的力量
    // ==========================================
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (play.Card.Owner?.Creature == base.Owner && play.Card.Enchantment != null)
        {
            Flash(); // 遗物/状态闪烁

            var enemies = base.CombatState.HittableEnemies;
            foreach (var enemy in enemies)
            {
                // 1. 在该怪物的账本上累加被扣除的力量数值
                decimal currentLost = StrengthLostThisTurn.Get(enemy);
                StrengthLostThisTurn.Set(enemy, currentLost + base.Amount);

                // 2. 真实下发扣除力量的指令（注意传负数）
                await PowerCmd.Apply<StrengthPower>(enemy, -base.Amount, base.Owner, null);
            }
        }
    }

    // ==========================================
    // 钩子 2：玩家回合结束时，精准归还力量
    // ==========================================
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        await base.AfterSideTurnStart(side, participants, combatState);

        // 确保是在玩家的回合结束时结算
        if (side == base.Owner.Side)
        {
            var enemies = base.CombatState.HittableEnemies;
            foreach (var enemy in enemies)
            {
                // 读取该怪物的欠条
                decimal toRestore = StrengthLostThisTurn.Get(enemy);
                
                if (toRestore > 0m)
                {
                    // 归还相应的力量
                    await PowerCmd.Apply<StrengthPower>(enemy, toRestore, base.Owner, null);
                    
                    // 账本清零
                    StrengthLostThisTurn.Set(enemy, 0m);
                }
            }
        }
    }
}
