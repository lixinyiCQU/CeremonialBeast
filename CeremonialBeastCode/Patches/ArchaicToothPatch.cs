using System.Collections.Generic;
using CeremonialBeast.CeremonialBeastCode.Cards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace CeremonialBeast.CeremonialBeastCode.Patches;

[HarmonyPatch(typeof(ArchaicTooth), "get_TranscendenceUpgrades")]
internal static class ArchaicToothPatch
{
    private static void Postfix(ref Dictionary<ModelId, CardModel> __result)
    {
        CardModel stomp = ModelDb.Card<CeremonialStomp>();
        __result.TryAdd(stomp.Id, ModelDb.Card<SavageMajesty>());
    }
}
