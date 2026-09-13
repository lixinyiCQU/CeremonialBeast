using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Relics;

public class SpiritBeastSkull : CeremonialBeastRelic
{
    private bool _isActive;

    public override RelicRarity Rarity => RelicRarity.Rare;

    private bool IsHpHalfOrLess()
    {
        return base.Owner?.Creature != null
            && base.Owner.Creature.CurrentHp <= base.Owner.Creature.MaxHp / 2m;
    }

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target == null || target != base.Owner?.Creature || dealer == null || dealer.Side == target.Side)
        {
            return 1m;
        }

        return IsHpHalfOrLess() ? 0.8m : 1m;
    }

    public override Task AfterCurrentHpChanged(Creature creature, decimal amount)
    {
        if (creature == base.Owner?.Creature && CombatManager.Instance.IsInProgress)
        {
            UpdateGlowState(flashOnChange: true);
        }

        return Task.CompletedTask;
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        UpdateGlowState(flashOnChange: room is CombatRoom);
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        _isActive = false;
        base.Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    private void UpdateGlowState(bool flashOnChange)
    {
        bool shouldBeActive = IsHpHalfOrLess();
        base.Status = shouldBeActive ? RelicStatus.Active : RelicStatus.Normal;

        if (flashOnChange && shouldBeActive != _isActive)
        {
            Flash();
        }

        _isActive = shouldBeActive;
    }
}
