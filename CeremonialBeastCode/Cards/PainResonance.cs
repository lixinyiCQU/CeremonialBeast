using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class PainResonance : CeremonialBeastCard
{
    // 注册 PowerVar，基础伤害为 11
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<PainResonancePower>(11m)
    };

    public PainResonance()
        : base(2, CardType.Power, CardRarity.Uncommon, TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 播放能力牌的释放动画
        
        
        // 挂载痛苦共鸣状态，层数即为伤害值
        await PowerCmd.Apply<PainResonancePower>(base.Owner.Creature, base.DynamicVars["PainResonancePower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级后伤害提升 4 点（11 -> 15）
        base.DynamicVars["PainResonancePower"].UpgradeValueBy(4m);
    }
}