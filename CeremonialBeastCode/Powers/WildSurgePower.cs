using MegaCrit.Sts2.Core.Models.Powers;
using CeremonialBeast.CeremonialBeastCode.Cards;
using BaseLib.Abstracts; // 确保引用了这个命名空间

namespace CeremonialBeast.CeremonialBeastCode.Powers;

// 保持继承 TemporaryStrengthPower，并添加 ICustomModel 接口
public class WildSurgePower : CustomTemporaryPowerModelWrapper<WildSurge, StrengthPower>
{
    public override string CustomPackedIconPath => "res://CeremonialBeast/images/powers/wild_surge_power.png";
    public override string CustomBigIconPath => "res://CeremonialBeast/images/powers/big/wild_surge_power.png";

    // 指向卡牌源，用于图标显示和名称匹配
}
