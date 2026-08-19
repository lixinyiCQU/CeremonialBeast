using BaseLib.Abstracts;
using CeremonialBeast.CeremonialBeastCode.Extensions;
using Godot;

namespace CeremonialBeast.CeremonialBeastCode.Character;

public class CeremonialBeastRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => CeremonialBeast.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}