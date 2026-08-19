using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class SoulOfWhiteStag() : CeremonialBeastCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    // 定义动态变量：提升比例 50
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("HealBonus", 100m)];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 施加“白鹿之魂”能力状态
        await PowerCmd.Apply<SoulOfWhiteStagPower>(
            Owner.Creature, 
            DynamicVars["HealBonus"].BaseValue, 
            Owner.Creature, 
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级后费用降低 1 点 (变为 0 费)
        EnergyCost.UpgradeBy(-1);
    }
}