using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class EscalatingFrenzy : CeremonialBeastCard
{
    // 注册抽牌变量，初始值为 2
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(2)
    };

    public EscalatingFrenzy()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 获取当前的抽牌数量
        int drawCount = (int)base.DynamicVars["Cards"].BaseValue;

        // 2. 执行抽牌指令
        if (drawCount > 0)
        {
            await CardPileCmd.Draw(choiceContext, drawCount, base.Owner);
        }

        // 3. 核心成长机制：将本张卡牌的抽牌基础数值增加 1
        // 在单场战斗中，这个修改会一直保留在该卡牌实例上
        base.DynamicVars["Cards"].BaseValue += 1m;
        
        // 提示：如果希望 UI 上的数字立即产生视觉上的颜色跳变反馈，可以调用以下方法（可选）
        // base.DynamicVars["Cards"].Flash(); 
    }

    protected override void OnUpgrade()
    {
        // 升级后初始抽牌数从 2 提升至 3
        base.DynamicVars["Cards"].UpgradeValueBy(1m);
    }
}