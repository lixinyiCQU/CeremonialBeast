using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using CeremonialBeast.CeremonialBeastCode.Powers;
// 确保引入了自定义标签所在的命名空间
// using CeremonialBeast.CeremonialBeastCode.Enums; 

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class BestialPresence : CeremonialBeastCard
{
    // 注册 Ringing 标签，底层将自动为你挂载状态
    protected override HashSet<CardTag> CanonicalTags => [CustomTags.Ringing];

    // 注册悬停提示，玩家鼠标悬停时会显示虚弱和易伤的词条解释
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<CeremonialBeast.CeremonialBeastCode.Powers.RingingPower>()
    ];

    // 使用官方的 PowerVar 规范声明初始层数
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>(2m),       // 初始 2 层虚弱
        new PowerVar<VulnerablePower>(2m)  // 初始 2 层易伤
    ];

    public BestialPresence()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 调用底层指令，向战斗状态中所有可受击的敌人 (HittableEnemies) 群体施加状态
        await PowerCmd.Apply<WeakPower>(base.CombatState!.HittableEnemies, base.DynamicVars.Weak.BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<VulnerablePower>(base.CombatState.HittableEnemies, base.DynamicVars.Vulnerable.BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级后虚弱提升 1 层（变成 2 层），易伤保持 2 层不变
        base.DynamicVars.Weak.UpgradeValueBy(1m);
        base.DynamicVars.Vulnerable.UpgradeValueBy(1m);
    }
}
