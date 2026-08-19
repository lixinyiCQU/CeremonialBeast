using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Entities.Powers;
using BaseLib.Abstracts;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public sealed class AdaptiveBodyPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    
    // 使用 Single 隐藏右下角的层数数字，因为这是一个布尔型的规则状态
    public override PowerStackType StackType => PowerStackType.None;

    // 不需要重写任何钩子，我们将在 PlowPower 中主动寻找它的存在
}