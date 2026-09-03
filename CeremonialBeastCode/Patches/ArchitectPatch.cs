using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using BeastCharacter = CeremonialBeast.CeremonialBeastCode.Character.CeremonialBeast;

namespace CeremonialBeast.CeremonialBeastCode.Patches;

[HarmonyPatch(typeof(TheArchitect), "get_DialogueSet")]
internal static class ArchitectPatch
{
    private static void Postfix(AncientDialogueSet __result)
    {
        string characterId = ModelDb.Character<BeastCharacter>().Id.Entry;
        if (__result.CharacterDialogues.ContainsKey(characterId))
        {
            return;
        }

        AncientDialogue dialogue = new("", "")
        {
            VisitIndex = 0,
            EndAttackers = ArchitectAttackers.Both
        };
        dialogue.PopulateLines("THE_ARCHITECT", characterId, 0);
        __result.CharacterDialogues.Add(characterId, new[] { dialogue });
    }
}
