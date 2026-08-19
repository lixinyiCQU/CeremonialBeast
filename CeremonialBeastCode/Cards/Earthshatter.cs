using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using CeremonialBeast.CeremonialBeastCode.Powers; 

namespace CeremonialBeast.CeremonialBeastCode.Cards;

// 💡 优化点 1：添加 sealed 关键字护体
public sealed class Earthshatter() : CeremonialBeastCard(2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    // 💡 优化点 2：补充缺失的悬停提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromPower<RingingPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(7m, ValueProp.Move),
        new RepeatVar(2)
    ];

    // ==========================================
    // 💡 优化点 3：使用行为架构！彻底删除旧的拦截逻辑
    // ==========================================
    // 我们只需用这一行代码，就完美替代了你之前的 IsPlayable 和 ShouldGlowGoldInternal 重写！
    public override RingingBehavior RingingInteractBehavior => RingingBehavior.Require;

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 执行多段 AOE 攻击指令
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .WithHitCount(base.DynamicVars.Repeat.IntValue)
            .FromCard(this, play)
            .TargetingAllOpponents(base.CombatState!)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // 升级后伤害提升 2 点 (5 -> 7)
        base.DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
