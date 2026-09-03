using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Powers;

// 修复 CS1729：移除构造函数的参数，直接继承 CustomPowerModel
public class FreeRingingPower : CeremonialBeastPower
{
    public override PowerType Type => PowerType.Buff;
    
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;

        if (card.Owner?.Creature != base.Owner)
        {
            return false;
        }

        // 修复 CS0117：使用你定义的 CustomTags.Ringing
        if (!card.Tags.Contains(CustomTags.Ringing))
        {
            return false;
        }

        bool isValidPile = false;
        switch (card.Pile?.Type)
        {
            case PileType.Hand:
            case PileType.Play:
                isValidPile = true;
                break;
        }

        if (!isValidPile)
        {
            return false;
        }

        modifiedCost = 0m; 
        return true;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        // 修复 CS0117：使用你定义的 CustomTags.Ringing
        if (cardPlay.Card.Owner?.Creature == base.Owner && cardPlay.Card.Tags.Contains(CustomTags.Ringing))
        {
            bool isValidPile = false;
            switch (cardPlay.Card.Pile?.Type)
            {
                case PileType.Hand:
                case PileType.Play:
                    isValidPile = true;
                    break;
            }

            if (isValidPile)
            {
                await PowerCmd.Decrement(this);
            }
        }
    }
}
