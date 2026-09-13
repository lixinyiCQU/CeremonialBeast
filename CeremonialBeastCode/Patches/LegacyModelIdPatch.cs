using CeremonialBeast.CeremonialBeastCode.Cards;
using CeremonialBeast.CeremonialBeastCode.Potions;
using CeremonialBeast.CeremonialBeastCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace CeremonialBeast.CeremonialBeastCode.Patches;

[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.InitIds))]
internal static class LegacyModelIdPatch
{
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    private static void RegisterLegacyIds(Dictionary<ModelId, AbstractModel> ____contentById)
    {
        AddAlias<SoulSiphon>(____contentById, "CARD.CEREMONIALBEAST-DEVOUR");
        AddAlias<GravelShot>(____contentById, "CARD.CEREMONIALBEAST-CLAW_FLURRY");
        AddAlias<Stomp>(____contentById, "CARD.CEREMONIALBEAST-CEREMONIAL_STOMP");
        AddAlias<ShieldTackle>(____contentById, "CARD.CEREMONIALBEAST-SIPHONING_BITE");
        AddAlias<RunicBinding>(____contentById, "CARD.CEREMONIALBEAST-ENCHANTED_DETERRENCE");
        AddAlias<SacrificialStone>(____contentById, "CARD.CEREMONIALBEAST-CEREMONIAL_PLOWING");
        AddAlias<Unburden>(____contentById, "CARD.CEREMONIALBEAST-EARTHEN_SHIELD");
        AddAlias<VigorousSwift>(____contentById, "CARD.CEREMONIALBEAST-ESCALATING_FRENZY");
        AddAlias<SacrilegiousCeremony>(____contentById, "CARD.CEREMONIALBEAST-PROFANE_RITUAL");
        AddAlias<Plow>(____contentById, "CARD.CEREMONIALBEAST-RAMPAGE");
        AddAlias<CeremonialRecall>(____contentById, "CARD.CEREMONIALBEAST-RITUAL_RECALL");
        AddAlias<Dormancy>(____contentById, "CARD.CEREMONIALBEAST-TOUGH_ROOTS");
        AddAlias<Soberize>(____contentById, "CARD.CEREMONIALBEAST-NOPE");

        AddAlias<ArchaicTooth>(____contentById, "RELIC.CEREMONIALBEAST-ANCIENT_TOOTH");
        AddAlias<RustedCopperBell>(____contentById, "RELIC.CEREMONIALBEAST-CEREMONIAL_DAGGER");
        AddAlias<CeremonialSpiritStone>(____contentById, "RELIC.CEREMONIALBEAST-CEREMONIAL_SOIL");
        AddAlias<CrimsonChalice>(____contentById, "RELIC.CEREMONIALBEAST-BEAST_BONE_FRAGMENT");
        AddAlias<SpiritBeastSkull>(____contentById, "RELIC.CEREMONIALBEAST-BLESSED_SOIL");
        AddAlias<SpikedRollingLog>(____contentById, "RELIC.CEREMONIALBEAST-BLOODY_TOTEM");
        AddAlias<CenserOfEnlightenment>(____contentById, "RELIC.CEREMONIALBEAST-HEAVY_SHACKLES");
        AddAlias<AmberOfMemory>(____contentById, "RELIC.CEREMONIALBEAST-OLD_PLOWSHARE");
        AddAlias<PaleAntlers>(____contentById, "RELIC.CEREMONIALBEAST-PALE_ANTLER");
        AddAlias<SacredCeremonialCrystal>(____contentById, "RELIC.CEREMONIALBEAST-FORTIFIED_CEREMONIAL_SOIL");

        AddAlias<SacrificialStonePotion>(____contentById, "POTION.CEREMONIALBEAST-EARTHEN_POTION");
        AddAlias<ClarityElixir>(____contentById, "POTION.CEREMONIALBEAST-CLARITY_POTION");
        AddAlias<SacrificeBlood>(____contentById, "POTION.CEREMONIALBEAST-SANGUINE_ELIXIR");
    }

    private static void AddAlias<T>(Dictionary<ModelId, AbstractModel> models, string legacyId)
        where T : AbstractModel
    {
        models.TryAdd(ModelId.Deserialize(legacyId), ModelDb.GetById<T>(ModelDb.GetId<T>()));
    }
}
