using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using CeremonialBeast.CeremonialBeastCode.Powers;
using MegaCrit.Sts2.Core.Models;
using BaseLib.Utils;

namespace CeremonialBeast.CeremonialBeastCode.Cards;
[Pool(typeof(MegaCrit.Sts2.Core.Models.CardPools.TokenCardPool))]

// 💡 优化 1：添加 sealed 关键字
public sealed class CeremonialPlowing() : CeremonialBeastCard(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PlowPower>()
    ];

    // 1. 定义变量：获得 2 层 Plow
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<PlowPower>(3)];

    // 2. 注册关键字：保留 (Retain) 和 消耗 (Exhaust)
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Retain, 
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 播放施法动画
        

        // 获得 Plow 状态
        await PowerCmd.Apply<PlowPower>(
            base.Owner.Creature, 
            base.DynamicVars[nameof(PlowPower)].BaseValue, 
            base.Owner.Creature, 
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(PlowPower)].UpgradeValueBy(1m);
    }

    // ==========================================
    // 💡 优化 2：植入官方同款“衍生牌工厂方法”
    // ==========================================

    // 工厂方法 A：生成到手牌 (如《耕耘之击》调用)
    public static async Task<CardModel?> CreateInHand(Player owner, ICombatState combatState, bool upgraded = false)
    {
        return (await CreateInHand(owner, 1, combatState, upgraded)).FirstOrDefault();
    }

    public static async Task<IEnumerable<CardModel>> CreateInHand(Player owner, int count, ICombatState combatState, bool upgraded = false)
    {
        if (count <= 0 || CombatManager.Instance.IsOverOrEnding) return Array.Empty<CardModel>();

        List<CardModel> generatedCards = new List<CardModel>();
        for (int i = 0; i < count; i++)
        {
            CardModel newCard = combatState.CreateCard<CeremonialPlowing>(owner);
            if (upgraded) CardCmd.Upgrade(newCard);
            generatedCards.Add(newCard);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(generatedCards, PileType.Hand, owner);
        return generatedCards;
    }

    // 工厂方法 B：生成到抽牌堆 (专为《丰收祭典》定制)
    public static async Task<IEnumerable<CardModel>> CreateInDrawPile(Player owner, int count, ICombatState combatState, bool upgraded = false)
    {
        if (count <= 0 || CombatManager.Instance.IsOverOrEnding) return Array.Empty<CardModel>();

        List<CardModel> generatedCards = new List<CardModel>();
        for (int i = 0; i < count; i++)
        {
            CardModel newCard = combatState.CreateCard<CeremonialPlowing>(owner);
            if (upgraded) CardCmd.Upgrade(newCard);
            generatedCards.Add(newCard);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(generatedCards, PileType.Draw, owner);
        return generatedCards;
    }
}
