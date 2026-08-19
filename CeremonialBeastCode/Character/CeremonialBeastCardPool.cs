using BaseLib.Abstracts;
using CeremonialBeast.CeremonialBeastCode.Extensions;
using Godot;

namespace CeremonialBeast.CeremonialBeastCode.Character;

public class CeremonialBeastCardPool : CustomCardPoolModel
{
    public override string Title => CeremonialBeast.CharacterId; 
    
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    // BaseLib uses this color to generate the card frame shader while CardFrameMaterialPath remains card_frame_red.
    public override Color ShaderColor => new Color("A3C9D7");
    public override float H => 0.54f;
    public override float S => 0.45f;
    public override float V => 1.7f;
    public override Color DeckEntryCardColor => new Color("A3C9D7");
    
    public override bool IsColorless => false;
}
