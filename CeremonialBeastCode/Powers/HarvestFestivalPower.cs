using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public sealed class HarvestFestivalPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    
    // 使用 Counter 类型，层数即代表下回合多抽几张牌
    public override PowerStackType StackType => PowerStackType.Counter;

    // 💡 官方标准钩子 1：修改回合开始时的抽牌数量
    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        // 确保是自己抽牌时才触发加成
        if (player == base.Owner.Player)
        {
            // 将基础抽牌数 (count) 加上本状态的层数 (base.Amount)
            return count + base.Amount;
        }
        return count;
    }

    // 💡 官方标准钩子 2：抽牌数量修改完毕后的回调
    public override async Task AfterModifyingHandDraw()
    {
        // 状态图标闪烁，给予玩家视觉反馈
        Flash();

        // 效果已经生效，将这个一次性状态从玩家身上移除
        await PowerCmd.Remove(this);
    }
}