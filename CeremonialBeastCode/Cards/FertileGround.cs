using System.Collections.Generic;
using System.Threading.Tasks;
using CeremonialBeast.CeremonialBeastCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class FertileGround : CeremonialBeastCard
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => (IEnumerable<IHoverTip>)(object)new IHoverTip[1] { HoverTipFactory.FromPower<PlowPower>() };

	protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[1] { (DynamicVar)new PowerVar<FertileGroundPower>(1m) };

	public FertileGround()
		: base(2, (CardType)3, (CardRarity)3, (TargetType)1)
	{
	}

	protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await PowerCmd.Apply<FertileGroundPower>(((CardModel)this).Owner.Creature, ((CardModel)this).DynamicVars["FertileGroundPower"].BaseValue, ((CardModel)this).Owner.Creature, (CardModel)(object)this, false);
	}

	protected override void OnUpgrade()
	{
		((CardModel)this).EnergyCost.UpgradeBy(-1);
	}
}
