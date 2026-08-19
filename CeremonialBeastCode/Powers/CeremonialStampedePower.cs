using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

// 修复 1：重命名为 CeremonialStampedePower，避开与原版游戏重名
public class CeremonialStampedePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner?.Creature == base.Owner && card.Type == CardType.Attack)
        {
            Flash();
            
            // 修复 2：加上 ! 消除可能为 null 的警告，或者先进行安全校验
            if (base.Owner.Player != null)
            {
                await PlayerCmd.GainEnergy(base.Amount, base.Owner.Player);
            }
        }
    }
}