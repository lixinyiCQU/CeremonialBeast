using BaseLib.Abstracts;
using BaseLib.Extensions;
using CeremonialBeast.CeremonialBeastCode.Extensions;
using Godot;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public abstract class CeremonialBeastPower : CustomPowerModel
{
    //Loads from CeremonialBeast/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}