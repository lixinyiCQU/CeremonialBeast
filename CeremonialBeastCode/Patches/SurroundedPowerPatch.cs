using System;
using System.Threading.Tasks;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Powers;
using BeastCharacter = CeremonialBeast.CeremonialBeastCode.Character.CeremonialBeast;

namespace CeremonialBeast.CeremonialBeastCode.Patches;

[HarmonyPatch(typeof(SurroundedPower), "FlipScale", new Type[] { typeof(Node2D) })]
internal static class SurroundedPowerNodePatch
{
    private static bool Prefix(SurroundedPower __instance, Node2D? body, ref Task __result)
    {
        if (__instance.Owner.Player?.Character is not BeastCharacter)
        {
            return true;
        }

        FlipForMonsterVisuals(__instance, body);
        __result = Task.CompletedTask;
        return false;
    }

    internal static void FlipForMonsterVisuals(SurroundedPower power, Node2D? body)
    {
        if (body == null)
        {
            return;
        }

        float x = body.Scale.X;
        if (ShouldFlip(power, x))
        {
            body.Scale *= new Vector2(-1f, 1f);
        }
    }

    internal static bool ShouldFlip(SurroundedPower power, float scaleX)
    {
        return (power.Facing == SurroundedPower.Direction.Right && scaleX > 0f) ||
               (power.Facing == SurroundedPower.Direction.Left && scaleX < 0f);
    }
}

[HarmonyPatch(typeof(SurroundedPower), "FlipScale", new Type[] { typeof(Control) })]
internal static class SurroundedPowerControlPatch
{
    private static bool Prefix(SurroundedPower __instance, Control? body, ref Task __result)
    {
        if (__instance.Owner.Player?.Character is not BeastCharacter)
        {
            return true;
        }

        if (body != null && SurroundedPowerNodePatch.ShouldFlip(__instance, body.Scale.X))
        {
            body.Scale *= new Vector2(-1f, 1f);
        }
        __result = Task.CompletedTask;
        return false;
    }
}
