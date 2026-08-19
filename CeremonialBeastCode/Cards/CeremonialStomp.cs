using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using CeremonialBeast.CeremonialBeastCode.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BaseLib.Patches.Content;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class CeremonialStomp() : CeremonialBeastCard(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(12m, ValueProp.Move),
        new PowerVar<WeakPower>(1),
        new PowerVar<VulnerablePower>(2)
    ];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 1. 造成伤害 (流式指令，需要 Execute)
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target!)
            .Execute(choiceContext);

        // 2. 施加虚弱 (直接指令，不需要 Execute)
        await PowerCmd.Apply<WeakPower>(play.Target!, DynamicVars[nameof(WeakPower)].BaseValue, Owner.Creature, this);

        // 3. 施加易伤 (直接指令，不需要 Execute)
        await PowerCmd.Apply<VulnerablePower>(play.Target!, DynamicVars[nameof(VulnerablePower)].BaseValue, Owner.Creature, this);

    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
    protected override HashSet<CardTag> CanonicalTags => [CustomTags.Ringing];
}
