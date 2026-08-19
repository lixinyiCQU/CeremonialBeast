using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using CeremonialBeast.CeremonialBeastCode.Powers;
using CeremonialBeast.CeremonialBeastCode.Enchantments;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class CeremonialHarvest : CeremonialBeastCard
{
    public override bool CanBeGeneratedInCombat => false;

    // 无视 Ringing 封印，可自由打出
    public override RingingBehavior RingingInteractBehavior => RingingBehavior.Dynamic;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    // 基础 12 点伤害
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(12m, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.Static(StaticHoverTip.Fatal),
        ..HoverTipFactory.FromEnchantment<AccumulateEnchantment>() 
    ];

    public CeremonialHarvest()
        : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        
        bool shouldTriggerFatal = cardPlay.Target.Powers.All((PowerModel p) => p.ShouldOwnerDeathTriggerFatal());

        AttackCommand attackCommand = await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        if (shouldTriggerFatal && attackCommand.Results.SelectMany(r => r).Any(r => r.WasTargetKilled))
        {
            // 安全获取玩家实体
            var player = base.Owner;
            if (player == null) return;

            var deckCards = PileType.Deck.GetPile(player).Cards;

            // ==========================================
            // ✨ 核心修复 1：严格排他性过滤
            // 只要身上有任何附魔 (Enchantment != null)，就绝对不允许再次被附魔。
            // 同时彻底删除了“如果没有未附魔的牌，就退而求其次”的备用逻辑。
            // ==========================================
            var validCards = deckCards
                .Where(c => c.Enchantment == null && (c.DeckVersion == null || c.DeckVersion.Enchantment == null))
                .ToList();

            if (validCards.Any())
            {
                // ==========================================
                // ✨ 核心修复 2：固定随机种子，杜绝 S/L 改变结果
                // 抛弃 new System.Random()，使用官方战斗卡牌抽取专属的 RNG
                // ==========================================
                var chosenCard = player.RunState.Rng.CombatCardSelection.NextItem(validCards);

                // 战斗内临时附魔生效
                CardCmd.Enchant<AccumulateEnchantment>(chosenCard!, 1m);
                
                // 永久写入玩家卡组（DeckVersion）
                if (chosenCard!.DeckVersion != null)
                {
                    var accumulateEnchantment = (AccumulateEnchantment)ModelDb.Enchantment<AccumulateEnchantment>().ToMutable();
                    chosenCard.DeckVersion.EnchantInternal(accumulateEnchantment, 1m);
                }

                // ==========================================
                // ✨ 核心修复 3：视觉展示
                // 强制唤起牌面预览 UI，让玩家清晰地看到哪张牌被附魔了
                // ==========================================
                CardCmd.Preview(chosenCard);
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后伤害提升
        base.DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
