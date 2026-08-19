using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class BeastInstinct() : CeremonialBeastCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    // 声明变量：基础 22 点伤害
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(22m, ValueProp.Move)
    ];

    // 挂载鸣响标签
    protected override HashSet<CardTag> CanonicalTags => [CustomTags.Ringing];

    // ==========================================
    // 核心机制：使用你找到的底层抽牌拦截钩子
    // ==========================================
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        // 必须保留基类的逻辑
        await base.AfterCardDrawn(choiceContext, card, fromHandDraw);

        // 防御性编程：确保当前引擎刚刚抽到手里的这张牌，就是“野兽本能”它自己
        if (card == this)
        {
            // 自动打出指令
            await CardCmd.AutoPlay(choiceContext, this, null);
        }
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 无论是玩家手动打出，还是 AutoPlay 自动打出，最终都会走到这里执行伤害
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .TargetingAllOpponents(base.CombatState!) // 群体攻击
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // 升级后伤害提升 9 点 (22 -> 31)
        DynamicVars.Damage.UpgradeValueBy(9m);
    }
}
