using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class Gigantification() : CeremonialBeastCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var creature = Owner.Creature;
        if (creature == null) return;

        decimal currentHp = creature.CurrentHp;
        decimal maxHp = creature.MaxHp;

        // 计算当前这次翻倍需要增加的量
        decimal bonusMaxHp = maxHp; 

        // 执行生命值翻倍
        await CreatureCmd.SetMaxHp(creature, maxHp + bonusMaxHp);
        await CreatureCmd.SetCurrentHp(creature, currentHp * 2m);

        // ==========================================
        // ✨ 核心修复：兵分两路
        // 1. 数据交接：将真实的血量累加存入秘密账本
        // ==========================================
        decimal previousBonus = GigantificationPower.BonusHpTracker.Get(creature);
        GigantificationPower.BonusHpTracker.Set(creature, previousBonus + bonusMaxHp);

        // ==========================================
        // 2. UI 交接：通知底层施加 1 层巨兽化能力（这样界面就会显示清爽的层数）
        // ==========================================
        await PowerCmd.Apply<GigantificationPower>(creature, 1m, creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
