using CeremonialBeast.CeremonialBeastCode.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using BeastCharacter = CeremonialBeast.CeremonialBeastCode.Character.CeremonialBeast;

namespace CeremonialBeast.CeremonialBeastCode.Patches;

internal static class YummyCookieIconPatch
{
    private const string IconFile = "yummy_cookie_ceremonial_beast.png";
    private const string OutlineFile = "yummy_cookie_ceremonial_beast_outline.png";

    private static bool IsCeremonialBeast(RelicModel relic) =>
        relic is YummyCookie && !relic.IsCanonical && relic.Owner?.Character is BeastCharacter;

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.PackedIconPath), MethodType.Getter)]
    private static class PackedIconPatch
    {
        private static void Postfix(RelicModel __instance, ref string __result)
        {
            if (IsCeremonialBeast(__instance))
            {
                __result = IconFile.RelicImagePath();
            }
        }
    }

    [HarmonyPatch(typeof(RelicModel), "PackedIconOutlinePath", MethodType.Getter)]
    private static class PackedOutlinePatch
    {
        private static void Postfix(RelicModel __instance, ref string __result)
        {
            if (IsCeremonialBeast(__instance))
            {
                __result = OutlineFile.RelicImagePath();
            }
        }
    }

    [HarmonyPatch(typeof(RelicModel), "BigIconPath", MethodType.Getter)]
    private static class BigIconPatch
    {
        private static void Postfix(RelicModel __instance, ref string __result)
        {
            if (IsCeremonialBeast(__instance))
            {
                __result = IconFile.BigRelicImagePath();
            }
        }
    }
}
