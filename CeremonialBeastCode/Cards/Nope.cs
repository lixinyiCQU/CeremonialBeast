using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using CeremonialBeast.CeremonialBeastCode.Powers;
using BaseLib.Utils;
using BaseLib.Patches.Content; // 确保 [Pool] 标签能正常工作

namespace CeremonialBeast.CeremonialBeastCode.Cards;

[Pool(typeof(MegaCrit.Sts2.Core.Models.CardPools.TokenCardPool))]
public sealed class Nope : CeremonialBeastCard
{
    public override RingingBehavior RingingInteractBehavior => RingingBehavior.Require;
    
    // ✨ 修复 1：只提供初始（未升级）状态下的基础关键字
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal, CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<RingingPower>()
    ];

    public Nope()
        : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.Owner?.Creature != null)
        {
            await PowerCmd.Remove<RingingPower>(base.Owner.Creature);
        }
    }

    // ✨ 修复 2：在升级钩子中，显式地动态修改卡牌实例上的关键字
    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Ethereal); // 移除“虚无”
        AddKeyword(CardKeyword.Retain);      // 添加“保留”
    }

    public static async Task<CardModel?> CreateOneInHand(Player owner, int amount, ICombatState combatState, bool upgraded = false)
    {
        var cards = await CreateInHand(owner, amount, combatState, upgraded);
        return cards.FirstOrDefault();
    }

    public static async Task<CardModel?> CreateInHand(Player owner, ICombatState combatState, bool upgraded = false)
    {
        var cards = await CreateInHand(owner, 1, combatState, upgraded);
        return cards.FirstOrDefault();
    }

    public static async Task<IEnumerable<CardModel>> CreateInHand(Player owner, int count, ICombatState combatState, bool upgraded = false)
    {
        if (count <= 0 || CombatManager.Instance?.IsOverOrEnding == true)
        {
            return Array.Empty<CardModel>();
        }

        List<CardModel> nopes = new List<CardModel>();
        for (int i = 0; i < count; i++)
        {
            CardModel newCard = combatState.CreateCard<Nope>(owner);
            
            if (upgraded)
            {
                // 底层调用 Upgrade 时，就会精准触发上面写好的 OnUpgrade 钩子
                CardCmd.Upgrade(newCard);
            }
            
            nopes.Add(newCard);
        }
        await CardPileCmd.AddGeneratedCardsToCombat(nopes, PileType.Hand, owner);
        
        return nopes;
    }
}
