using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Powers;
using CeremonialBeast.CeremonialBeastCode.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using BaseLib.Abstracts;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public sealed class CeremonialFormPower : CeremonialBeastPower
{
    public int UpgradedStacks;
    
    public override PowerType Type => PowerType.Buff;
    
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player == base.Owner.Player)
        {
            Flash();

            await PowerCmd.Apply<RingingPower>(base.Owner, 1m, base.Owner, null);

            int upgradedCount = Math.Min(UpgradedStacks, (int)base.Amount);
            int normalCount = (int)base.Amount - upgradedCount;

            await Plow.CreateInHand(player, normalCount, combatState, false);
            await Plow.CreateInHand(player, upgradedCount, combatState, true);
        }
    }
}
