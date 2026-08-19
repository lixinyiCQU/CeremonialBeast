using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class Stampede() : CeremonialBeastCard(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new CardsVar(3),
        new EnergyVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [EnergyHoverTip];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 挂载重命名后的临时状态
        await PowerCmd.Apply<CeremonialStampedePower>(
            Owner.Creature, 
            DynamicVars.Energy.BaseValue, 
            Owner.Creature, 
            this
        );

        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);

        // 卸载重命名后的临时状态
        var tempPower = Owner.Creature.GetPower<CeremonialStampedePower>();
        if (tempPower != null)
        {
            await PowerCmd.Remove(tempPower);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(2m);
    }
}