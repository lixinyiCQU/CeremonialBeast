using System;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Audio;
using BeastCharacter = CeremonialBeast.CeremonialBeastCode.Character.CeremonialBeast;

namespace CeremonialBeast.CeremonialBeastCode.Patches;

[HarmonyPatch(typeof(SfxCmd), nameof(SfxCmd.PlayDeath), new Type[] { typeof(Player) })]
internal static class CharacterDeathSfxPatch
{
    private static bool Prefix(Player player)
    {
        if (player.Character is not BeastCharacter)
        {
            return true;
        }

        NAudioManager.Instance?.PlayOneShot(BeastCharacter.DeathSfxPath);
        return false;
    }
}
