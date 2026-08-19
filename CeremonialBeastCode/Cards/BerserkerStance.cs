using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class BerserkerStance() : CeremonialBeastCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    // 定义动态变量：基础 25% 额外伤害
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("DamageBonus", 50m)];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 施加狂暴姿态状态
        await PowerCmd.Apply<BerserkerStancePower>(
            Owner.Creature, 
            DynamicVars["DamageBonus"].BaseValue, 
            Owner.Creature, 
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级后额外伤害倍率提升至 50% (+25)
        DynamicVars["DamageBonus"].UpgradeValueBy(25m);
    }
}