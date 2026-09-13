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
public class ClarityElixir : CeremonialBeastPotion 
{
    // ✨ 专属罕见药水
    public override PotionRarity Rarity => PotionRarity.Uncommon;
    public override TargetType TargetType => TargetType.None; 
    public override PotionUsage Usage => PotionUsage.CombatOnly;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        // 检查玩家身上是否有 Ringing 状态，如果有则直接移除
        if (Owner.Creature.HasPower<RingingPower>())
        {
            await PowerCmd.Remove<RingingPower>(Owner.Creature);
        }
    }
}