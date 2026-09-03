using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public sealed class ToughRootsPower : CeremonialBeastPower
{
    private decimal _energyNextTurn;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public decimal EnergyNextTurn
    {
        get => _energyNextTurn;
        set
        {
            AssertMutable();
            _energyNextTurn = value;
        }
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.BeforeSideTurnEnd(choiceContext, side, participants);

        if (side != base.Owner.Side || !base.Owner.HasPower<RingingPower>())
        {
            return;
        }

        Flash();
        ToughRootsActivePower? activePower = await PowerCmd.Apply<ToughRootsActivePower>(
            base.Owner,
            base.Amount,
            base.Owner,
            null);

        if (activePower != null)
        {
            activePower.EnergyAmount += EnergyNextTurn;
        }
    }
}
