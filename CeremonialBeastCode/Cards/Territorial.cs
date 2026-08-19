using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class Territorial() : CeremonialBeastCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    // 定义动态变量：抽 1 张牌。因为是能力牌，这个值将作为状态的层数传递。
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 施加专属的“领地意识”状态
        await PowerCmd.Apply<CeremonialTerritorialPower>(
            Owner.Creature, 
            DynamicVars.Cards.BaseValue, 
            Owner.Creature, 
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级后获得固有 (Innate) 属性
        AddKeyword(CardKeyword.Innate);
    }
}