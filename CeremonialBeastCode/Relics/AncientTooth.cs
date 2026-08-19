using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CeremonialBeast.CeremonialBeastCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace CeremonialBeast.CeremonialBeastCode.Relics;

public sealed class AncientTooth : CeremonialBeastRelic
{
    private SerializableCard? _starterCard;
    private SerializableCard? _ancientCard;
    private List<IHoverTip> _extraHoverTips = [];

    public override RelicRarity Rarity => RelicRarity.Ancient;

    [SavedProperty]
    public SerializableCard? StarterCard
    {
        get => _starterCard;
        private set
        {
            AssertMutable();
            _starterCard = value;
            UpdateHoverTips();
        }
    }

    [SavedProperty]
    public SerializableCard? AncientCard
    {
        get => _ancientCard;
        private set
        {
            AssertMutable();
            _ancientCard = value;
            UpdateHoverTips();
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StringVar("StarterCard"),
        new StringVar("AncientCard")
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => _extraHoverTips;

    protected override void AfterCloned()
    {
        base.AfterCloned();
        _extraHoverTips = [];
    }

    public bool SetupForPlayer(Player player)
    {
        AssertMutable();
        CardModel? stomp = GetStomp(player);
        if (stomp == null)
        {
            return false;
        }

        StarterCard = stomp.ToSerializable();
        AncientCard = CreateSavageMajestyFrom(stomp).ToSerializable();
        UpdateHoverTips();
        return true;
    }

    private static CardModel? GetStomp(Player player)
    {
        return player.Deck.Cards.FirstOrDefault(card => card is CeremonialStomp);
    }

    private CardModel CreateSavageMajestyFrom(CardModel stomp)
    {
        CardModel savageMajesty = stomp.Owner.RunState.CreateCard<SavageMajesty>(stomp.Owner);

        if (stomp.IsUpgraded)
        {
            CardCmd.Upgrade(savageMajesty);
        }

        if (stomp.Enchantment != null)
        {
            EnchantmentModel enchantment = (EnchantmentModel)stomp.Enchantment.MutableClone();
            CardCmd.Enchant(enchantment, savageMajesty, enchantment.Amount);
        }

        return savageMajesty;
    }

    private void UpdateHoverTips()
    {
        _extraHoverTips.Clear();

        if (StarterCard != null)
        {
            CardModel starterCard = CardModel.FromSerializable(StarterCard);
            _extraHoverTips.AddRange(starterCard.HoverTips);
            _extraHoverTips.Add(HoverTipFactory.FromCard(starterCard));
            ((StringVar)base.DynamicVars["StarterCard"]).StringValue = starterCard.Title;
        }

        if (AncientCard != null)
        {
            CardModel ancientCard = CardModel.FromSerializable(AncientCard);
            _extraHoverTips.AddRange(ancientCard.HoverTips);
            _extraHoverTips.Add(HoverTipFactory.FromCard(ancientCard));
            ((StringVar)base.DynamicVars["AncientCard"]).StringValue = ancientCard.Title;
        }
    }

    public override async Task AfterObtained()
    {
        CardModel? stomp = GetStomp(base.Owner);
        if (stomp == null)
        {
            return;
        }

        CardModel savageMajesty = CreateSavageMajestyFrom(stomp);
        StarterCard = stomp.ToSerializable();
        AncientCard = savageMajesty.ToSerializable();
        await CardCmd.Transform(stomp, savageMajesty);
    }
}
