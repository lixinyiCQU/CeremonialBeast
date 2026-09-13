using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public sealed class SacrilegiousCeremonyPower : CeremonialBeastPower
{
    public override PowerType Type => PowerType.Buff;
    
    // 核心规则修改类状态，使用 Single 隐藏数字
    public override PowerStackType StackType => PowerStackType.None;
}
