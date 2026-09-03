using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public sealed class ToughRootsActivePower : CeremonialBeastPower
{
    private decimal _energyAmount;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public decimal EnergyAmount
    {
        get => _energyAmount;
        set
        {
            AssertMutable();
            _energyAmount = value;
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await base.AfterPlayerTurnStart(choiceContext, player);

        if (player.Creature != base.Owner)
        {
            return;
        }

        Flash();
        await PlayerCmd.GainEnergy(EnergyAmount, player);
        await CardPileCmd.Draw(choiceContext, base.Amount, player);
        await PowerCmd.Remove(this);
    }
}
