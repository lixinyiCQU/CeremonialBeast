using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using CeremonialBeast.CeremonialBeastCode.Character;
using CeremonialBeast.CeremonialBeastCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace CeremonialBeast.CeremonialBeastCode.Potions;

[Pool(typeof(CeremonialBeastPotionPool))]
public class SanguineElixir : CeremonialBeastPotion 
{
    // ✨ 专属稀有药水
    public override PotionRarity Rarity => PotionRarity.Rare;
    public override TargetType TargetType => TargetType.None; 
    public override PotionUsage Usage => PotionUsage.CombatOnly;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        // 饮用后，给玩家挂载“吸血”状态预备拦截
        await PowerCmd.Apply<SanguineElixirPower>(
            Owner.Creature,
            1m, // 只需要触发一次，1 层即可
            Owner.Creature
        );
    }
}