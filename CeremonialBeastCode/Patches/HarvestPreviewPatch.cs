using CeremonialBeast.CeremonialBeastCode.Cards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace CeremonialBeast.CeremonialBeastCode.Patches;

[HarmonyPatch(typeof(NCardPlay), "ShowMultiCreatureTargetingVisuals")]
internal static class HarvestPreviewPatch
{
    private static void Postfix(NCardPlay __instance)
    {
        if (__instance.Holder.CardNode?.Model is Harvest harvest)
        {
            __instance.Holder.CardNode.SetPreviewTarget(harvest.Owner.Creature);
        }
    }
}
