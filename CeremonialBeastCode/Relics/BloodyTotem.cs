using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.ValueProps;
// 必须引入你的 Powers 命名空间，以便读取 PlowPower
using CeremonialBeast.CeremonialBeastCode.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace CeremonialBeast.CeremonialBeastCode.Relics;

public class BloodyTotem : CeremonialBeastRelic
{
    // 设置稀有度为罕见 (Uncommon)
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    // ==========================================
    // ✨ 核心机制：在玩家的回合开始时触发
    // ==========================================
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        // 确保是当前遗物拥有者的回合
        if (player == base.Owner)
        {
            // 1. 获取玩家身上当前的【犁地】(Plow) 层数
            decimal plowAmount = player.Creature.GetPowerAmount<PlowPower>();

            // 2. 只有当犁地层数大于 0 时才触发遗物效果
            if (plowAmount > 0m)
            {
                // 3. 触发遗物闪烁，给予玩家视觉反馈
                Flash();

                // 4. 获取当前所有存活且可被选中的敌人
                var enemies = player.Creature.CombatState?.HittableEnemies;
                if (enemies?.Any() == true)
                {
                    // 5. 执行群体伤害指令
                    // 注意：这里使用的是 ValueProp.Unpowered，因为遗物造成的直接伤害通常不受玩家“力量”属性的加成
                    await CreatureCmd.Damage(
                        choiceContext, 
                        enemies, 
                        plowAmount, 
                        ValueProp.Unpowered, 
                        base.Owner.Creature, 
                        null,
                        null
                    );
                }
            }
        }
    }
}
