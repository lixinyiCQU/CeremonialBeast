using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using CeremonialBeast.CeremonialBeastCode.Cards;
using CeremonialBeast.CeremonialBeastCode.Character;
using MegaCrit.Sts2.Core.Entities.Relics;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;

namespace CeremonialBeast.CeremonialBeastCode.Relics;

public sealed class FortifiedCeremonialSoil : CeremonialBeastRelic
{
    // 作为初始遗物的替换升级，通常将其稀有度设为 Boss
    public override RelicRarity Rarity => RelicRarity.Starter;

    // 注册动态变量：加入 1 张牌，持续 3 个回合
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DynamicVar("CardsCount", 1m),
        new DynamicVar("Turns", 3m)
    ];

    // 完全复用你完美测试通过的悬停提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromCardWithCardHoverTips<CeremonialPlowing>();

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        // ✨ 核心修改：将回合判断条件从 <= 1 改为 <= 动态变量 Turns (即 3)
        if (player == base.Owner && combatState.RoundNumber <= base.DynamicVars["Turns"].IntValue)
        {
            Flash();

            // 1. 生成一张纯净的、没有主人的卡牌实例
            CardModel cardToAdd = ModelDb.Card<CeremonialPlowing>().ToMutable();

            // 2. 交给引擎的 AddCard 方法进行注册
            combatState.AddCard(cardToAdd, player);

            // 3. 安全加入手牌
            await CardPileCmd.Add(cardToAdd, PileType.Hand, CardPilePosition.Top, this);
        }
    }
}
