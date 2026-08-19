using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

public class CeremonialHoardPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // ==========================================
    // 动作 1：在准备弃牌前，拦截并触发保留机制
    // ==========================================
    public override async Task BeforeFlushLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner.Player || !Hook.ShouldFlush(player.Creature.CombatState!, player))
        {
            return;
        }

        // 调用底层接口弹出选牌 UI
        List<CardModel> list = [.. (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 0, base.Amount), 
            context: choiceContext, 
            player: base.Owner.Player, 
            filter: RetainFilter, 
            source: this))];

        if (list.Count > 0)
        {
            foreach (CardModel item in list)
            {
                item.GiveSingleTurnRetain();
            }
        }
    }

    // ==========================================
    // 动作 2：回合彻底结束时，安全自我销毁
    // ==========================================
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);

        // 只有当前阵营（玩家回合）结束时，才清空此状态，确保它只对本回合生效
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }

    // 过滤掉那些本来就已经带有保留属性的卡牌，防止重复选择
    private bool RetainFilter(CardModel card)
    {
        return !card.ShouldRetainThisTurn;
    }
}
