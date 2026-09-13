using CeremonialBeast.CeremonialBeastCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace CeremonialBeast.CeremonialBeastCode.Patches;

[HarmonyPatch(typeof(TouchOfOrobas), nameof(TouchOfOrobas.GetUpgradedStarterRelic))]
internal static class StarterRelicUpgradePatch
{
    private static void Postfix(RelicModel starterRelic, ref RelicModel __result)
    {
        if (starterRelic.Id == ModelDb.Relic<CeremonialSpiritStone>().Id)
        {
            __result = ModelDb.Relic<SacredCeremonialCrystal>();
        }
    }
}
