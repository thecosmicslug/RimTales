using HarmonyLib;
using Verse;

namespace RimTales
{
    [HarmonyPatch(typeof(Pawn),nameof(Pawn.Kill))]
    static class ColonistDied
    {
        [HarmonyPrefix]
        static void Prefix(Pawn __instance)
        {

            if (!__instance.IsColonist)
            {
                return;
            }

            Resources.deadPawns.Add(__instance);
            Resources.deadPawnsForMassFuneral.Add(__instance);
            Resources.EventManager.Add(new ADead(Utils.CurrentDate(), __instance));
            Log.Message("[RimTales]: Pawn.Kill() - New ADead " + __instance);

            if (Resources.isMemorialDayCreated)
            {
                return;
            }

            Resources.EventManager.Add(new AMemorialDay(Utils.CurrentDate(), __instance));
            Resources.isMemorialDayCreated = true;
            Log.Message("[RimTales]: Pawn.Kill() - New AMemorialDay " + __instance);
        }
    }
}