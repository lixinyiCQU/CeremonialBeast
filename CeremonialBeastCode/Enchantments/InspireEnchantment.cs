using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Enchantments;

namespace CeremonialBeast.CeremonialBeastCode.Enchantments;

public sealed class InspireEnchantment : CustomEnchantmentModel
{
    private bool _usedThisCombat;

    protected override string? CustomIconPath =>
        "res://CeremonialBeast/images/enchantments/inspire_enchantment.png";

    public bool TryConsume()
    {
        if (_usedThisCombat)
        {
            return false;
        }

        AssertMutable();
        _usedThisCombat = true;
        Status = EnchantmentStatus.Disabled;
        return true;
    }
}
