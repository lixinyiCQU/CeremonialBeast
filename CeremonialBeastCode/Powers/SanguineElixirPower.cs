using System;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public class SanguineElixirPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    // 使用 None 隐藏右下角的数字，让它看起来像是一个纯粹的状态标记
    public override PowerStackType StackType => PowerStackType.None;

    // ✨ 核心追踪变量：记录当前这张攻击牌造成的总未被格挡伤害
    private decimal _totalUnblockedDamage = 0m;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        await base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);

        // 1. 伤害来源必须是玩家本体
        if (dealer != base.Owner) return;

        // 2. 必须是攻击牌造成的伤害（过滤掉遗物或能力的伤害）
        if (cardSource == null || cardSource.Type != CardType.Attack) return;

        // 3. 将每一击未被格挡的物理伤害默默累加到追踪变量中
        if (result.UnblockedDamage > 0)
        {
            _totalUnblockedDamage += result.UnblockedDamage;
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.AfterCardPlayed(choiceContext, cardPlay);

        // 如果打出的是攻击牌（哪怕打空了或被格挡完了，也算作打出了下一张攻击牌，消耗掉 Buff）
        if (cardPlay.Card.Type == CardType.Attack && cardPlay.Card.Owner.Creature == base.Owner)
        {
            // 如果成功造成了伤害
            if (_totalUnblockedDamage > 0)
            {
                // ✨ 计算总伤害的 50%，并向下取整以符合原版生命值计算的严谨性
                decimal healAmount = Math.Floor(_totalUnblockedDamage / 2m);
                
                if (healAmount > 0)
                {
                    // 状态栏图标闪烁，给予视觉反馈
                    Flash();

                    // 调用 Heal，第三个参数为布尔值（是否为超量治疗等标记）
                    await CreatureCmd.Heal(base.Owner, healAmount, false);
                }
            }

            // 结算完毕，将追踪变量清零并销毁自身状态
            _totalUnblockedDamage = 0m;
            await PowerCmd.Remove(this);
        }
    }
}