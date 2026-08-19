using BaseLib.Abstracts;
using BaseLib.Utils;
using CeremonialBeast.CeremonialBeastCode.Character;

namespace CeremonialBeast.CeremonialBeastCode.Potions;

[Pool(typeof(CeremonialBeastPotionPool))]
public abstract class CeremonialBeastPotion : CustomPotionModel;