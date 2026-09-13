using BaseLib.Extensions;
using CeremonialBeast.CeremonialBeastCode.Potions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace CeremonialBeast.CeremonialBeastCode.Patches;

internal static class PotionImagePatch
{
    private static string AssetName(PotionModel potion) =>
        $"{potion.Id.Entry.RemovePrefix().ToLowerInvariant()}.png";

    [HarmonyPatch(typeof(PotionModel), "get_PackedImagePath")]
    private static class PackedImagePathPatch
    {
        private static void Postfix(PotionModel __instance, ref string __result)
        {
            if (__instance is CeremonialBeastPotion)
            {
                __result = $"res://CeremonialBeast/images/potions/{AssetName(__instance)}";
            }
        }
    }

    [HarmonyPatch(typeof(PotionModel), "get_PackedOutlinePath")]
    private static class PackedOutlinePathPatch
    {
        private static void Postfix(PotionModel __instance, ref string __result)
        {
            if (__instance is CeremonialBeastPotion)
            {
                string name = AssetName(__instance).Replace(".png", "_outline.png");
                __result = $"res://CeremonialBeast/images/potions/{name}";
            }
        }
    }

    [HarmonyPatch(typeof(PotionModel), nameof(PotionModel.LargeImagePath), MethodType.Getter)]
    private static class LargeImagePathPatch
    {
        private static void Postfix(PotionModel __instance, ref string __result)
        {
            if (__instance is CeremonialBeastPotion)
            {
                __result = $"res://CeremonialBeast/images/potions/large/{AssetName(__instance)}";
            }
        }
    }
}
