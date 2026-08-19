using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class ResoundingEcho : CeremonialBeastCard
{
    // ==========================================
    // ✨ 注册能力变量：基础 8 层（即 8 点伤害）
    // ==========================================
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<ResoundingEchoPower>(8m)
    };

    public ResoundingEcho()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 动态拉取注册的变量值 (8 或 11)
        decimal powerAmount = base.DynamicVars.Values.OfType<PowerVar<ResoundingEchoPower>>().First().BaseValue;

        // 挂载状态
        await PowerCmd.Apply<ResoundingEchoPower>(
            base.Owner.Creature, 
            powerAmount, 
            base.Owner.Creature, 
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级后层数(伤害)提升 3 点 (8 -> 11)
        base.DynamicVars.Values.OfType<PowerVar<ResoundingEchoPower>>().First().UpgradeValueBy(3m);
    }
}