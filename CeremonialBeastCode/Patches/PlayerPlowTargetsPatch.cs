using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace CeremonialBeast.CeremonialBeastCode.Patches;

// The monster caches world-space endpoints; a playable character can turn around later.
internal static class PlayerPlowTargetsPatch
{
    private const string StartKey = "player_plow_start";
    private const string EndKey = "player_plow_end";

    [HarmonyPatch(typeof(NCeremonialBeastVfx), "TurnOnDeathParticles")]
    private static class DeathParticlesPatch
    {
        private static void Prefix(NCeremonialBeastVfx __instance, GpuParticles2D ____deathParticles)
        {
            var body = __instance.GetParent<Node2D>();
            if (body.HasMeta("player_death_transform"))
                ____deathParticles.Transform = body.Transform * body.GetMeta("player_death_transform").AsTransform2D();
        }
    }

    [HarmonyPatch(typeof(NCeremonialBeastVfx), nameof(NCeremonialBeastVfx._Ready))]
    private static class ReadyPatch
    {
        private static void Postfix(NCeremonialBeastVfx __instance,
            Vector2 ____globalPlowTarget, Vector2 ____globalPlowEndTarget)
        {
            var body = __instance.GetParent<Node2D>();
            if (!body.HasMeta("ceremonial_beast_player")) return;
            body.SetMeta(StartKey, body.ToLocal(____globalPlowTarget));
            body.SetMeta(EndKey, body.ToLocal(____globalPlowEndTarget));
        }
    }

    [HarmonyPatch(typeof(NCeremonialBeastVfx), "OnPlowStart")]
    private static class StartPatch
    {
        private static bool Prefix(NCeremonialBeastVfx __instance, Node2D ____plowStartTarget)
        {
            return RestoreTarget(__instance, ____plowStartTarget, StartKey);
        }
    }

    [HarmonyPatch(typeof(NCeremonialBeastVfx), "OnPlowEnd")]
    private static class EndPatch
    {
        private static bool Prefix(NCeremonialBeastVfx __instance, Node2D ____plowEndTarget)
        {
            return RestoreTarget(__instance, ____plowEndTarget, EndKey);
        }
    }

    private static bool RestoreTarget(NCeremonialBeastVfx effect, Node2D target, string key)
    {
        var body = effect.GetParent<Node2D>();
        if (!body.HasMeta(key)) return true;
        target.GlobalPosition = body.ToGlobal(body.GetMeta(key).AsVector2());
        return false;
    }
}
