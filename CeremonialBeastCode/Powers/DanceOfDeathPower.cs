using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public class DanceOfDeathPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public static readonly SpireField<Creature, decimal> StoredDamage = new(() => 0m);
    public decimal RealDamage => StoredDamage.Get(base.Owner);
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target != base.Owner) return 1m;
        if (!props.IsPoweredAttack()) return 1m;
        
        return 0.5m;
    }

    // ✨ 修复 1：改回玩家回合开始钩子，引擎会合法下发 choiceContext
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature == base.Owner)
        {
            await DealDamageToAllEnemies(choiceContext);
            
            StoredDamage.Set(base.Owner, 0m);
            await PowerCmd.Remove(this);
        }
    }

    // 接收上下文并传递给伤害指令
    private async Task DealDamageToAllEnemies(PlayerChoiceContext choiceContext)
    {
        Flash(); 
        var enemies = base.CombatState.HittableEnemies;
        
        if (enemies.Any())
        {
            decimal damageToDeal = StoredDamage.Get(base.Owner);

            // ✨ 修复 2：传入合法的 choiceContext，彻底杜绝 NRE 崩溃
            await CreatureCmd.Damage(
                choiceContext, 
                enemies, 
                damageToDeal, 
                ValueProp.Move, 
                base.Owner, 
                null,
                null
            );
        }
    }
}
