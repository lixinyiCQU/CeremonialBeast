using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

// 【修复】：类名改为 Strike, 1费, 攻击牌
public class Strike() : CeremonialBeastCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    // 1. 定义基础伤害变量：基础 6 点伤害
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];

    // 2. 添加 Strike 标签，确保它能被相关遗物/卡牌识别
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    // 3. 卡牌打出时的核心逻辑
    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 组装并执行攻击指令
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target!)
            .Execute(choiceContext);
            
    }

    // 4. 卡牌升级逻辑
    protected override void OnUpgrade()
    {
        // 伤害提升 3 点 (从 6 提升至 9)
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
    
}
