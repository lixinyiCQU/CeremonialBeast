using MegaCrit.Sts2.Core.Entities.Relics;

namespace CeremonialBeast.CeremonialBeastCode.Relics;

public class PaleAntlers : CeremonialBeastRelic
{
    // 设置稀有度为商店专属 (Shop)
    public override RelicRarity Rarity => RelicRarity.Shop;

    // 这个遗物不需要写任何生命周期钩子。
    // 它的存在本身就是一个判定条件，实际的属性反转逻辑将由 PlowPower 接管。
}