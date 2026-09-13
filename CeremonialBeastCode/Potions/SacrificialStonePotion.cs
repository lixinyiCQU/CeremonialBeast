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
public class SacrificialStonePotion : CeremonialBeastPotion 
{
    public override PotionRarity Rarity => PotionRarity.Common;
    public override TargetType TargetType => TargetType.None; // 不需要选中敌人
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await PowerCmd.Apply<PlowPower>(
            Owner.Creature, // 施加目标（玩家自己）
            3,              // 施加层数
            Owner.Creature, // 施加目标（玩家自己）
            null            // 来源卡牌（因为是药水，所以直接填 null）
        );
    }
}