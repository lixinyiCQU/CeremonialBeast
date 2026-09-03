using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps; // 引入枚举命名空间

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public class TenacityPower : CeremonialBeastPower
{
    public override PowerType Type => PowerType.Buff;
    
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != base.Owner.Side)
        {
            return;
        }

        if ((decimal)base.Owner.CurrentHp <= (decimal)base.Owner.MaxHp / 2m)
        {
            Flash(); 
            
            // 修复 CS1503：将层数强转为 decimal，并传入 ValueProp.Unpowered
            await CreatureCmd.GainBlock(base.Owner, (decimal)base.Amount, ValueProp.Unpowered, null);
        }
    }
}
