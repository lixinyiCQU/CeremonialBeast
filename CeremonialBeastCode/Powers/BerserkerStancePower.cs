using System.Collections.Generic;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public class BerserkerStancePower : CeremonialBeastPower
{
    public override PowerType Type => PowerType.Buff;
    
    public override PowerStackType StackType => PowerStackType.Counter;

    // 注意参数类型里的 ? 号，要和官方源码严格对齐
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        // 1. 如果不是物理攻击，直接返回默认倍率 1m (无加成)
        if (!props.IsPoweredAttack()) 
        {
            return 1m;
        }

        // 2. 双重身份校验：兼顾 UI 悬停预测与实战伤害判定
        bool isPlayerAttack = false;
        if (dealer == base.Owner) 
        {
            isPlayerAttack = true; // 实战打出时
        }
        else if (dealer == null && cardSource?.Owner?.Creature == base.Owner) 
        {
            isPlayerAttack = true; // 手牌悬停预览时
        }

        // 拦截敌人的攻击
        if (!isPlayerAttack) 
        {
            return 1m;
        }

        // 3. 核心血量阈值判定 (半血及以下)
        if ((decimal)base.Owner.CurrentHp <= (decimal)base.Owner.MaxHp / 2m)
        {
            // ✨ 核心修复：返回的是系数！
            // 如果 base.Amount 是 25，这里就是 1m + (25m / 100m) = 1.25m
            // 引擎在底层会自动拿原伤害去乘以这个 1.25m
            return 1m + ((decimal)base.Amount / 100m);
        }

        // 4. 默认放行：条件不满足时，返回系数 1m
        return 1m;
    }
}
