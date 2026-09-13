using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace CeremonialBeast.CeremonialBeastCode.Patches;

internal static class StaticMerchantCharacterPatch
{
    private static bool HasSpineVisual(NMerchantCharacter character) =>
        character.GetChildCount() > 0 && character.GetChild(0).GetClass() == "SpineSprite";

    [HarmonyPatch(typeof(NMerchantCharacter), nameof(NMerchantCharacter._Ready))]
    private static class ReadyPatch
    {
        private static bool Prefix(NMerchantCharacter __instance) => HasSpineVisual(__instance);
    }

    [HarmonyPatch(typeof(NMerchantCharacter), nameof(NMerchantCharacter.PlayAnimation))]
    private static class PlayAnimationPatch
    {
        private static bool Prefix(NMerchantCharacter __instance) => HasSpineVisual(__instance);
    }
}
