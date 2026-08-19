using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public class Forage() : CeremonialBeastCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    // 声明变量：5 点伤害，判定阈值 3 张，抽 1 张牌
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(5m, ValueProp.Move),
        new DynamicVar("CardCount", 3m),
        new CardsVar(1)
    ];

    // ✨ 视觉反馈：满足抽牌条件时，卡牌发金光
    protected override bool ShouldGlowGoldInternal => ShouldDrawCard;

    // 核心判定逻辑提取
    private bool ShouldDrawCard
    {
        get
        {
            // 1. 获取当前手牌中的“其他牌”数量（使用官方安全统计法）
            int otherCardsInHand = base.Owner.PlayerCombatState?.Hand.Cards.Count(c => c != this) ?? 0;
            
            // 2. 加上这张牌自身，还原玩家打出它之前的手牌总数
            int totalHandCards = otherCardsInHand + 1;
            
            // 3. 判断是否满足条件（<= 3 张，升级后 <= 4 张）
            return totalHandCards <= DynamicVars["CardCount"].IntValue;
        }
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (play.Target != null)
        {
            // 1. 结算基础物理伤害
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
                .Targeting(play.Target)
                // 觅食是一种快速动作，使用利爪/快速斩击的特效更合适
                .Execute(choiceContext);

            // 2. 触发条件抽牌
            if (ShouldDrawCard)
            {
                // ✨ 顺手修复：使用 (int) 强转以契合底层的参数要求
                await CardPileCmd.Draw(choiceContext, (int)DynamicVars.Cards.BaseValue, Owner);
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后伤害提升 2 点 (5 -> 7)
        DynamicVars.Damage.UpgradeValueBy(2m);
        
        // ✨ 核心修改：升级后判定阈值提升 1 张 (3 -> 4)
        DynamicVars["CardCount"].UpgradeValueBy(1m);
    }
}
