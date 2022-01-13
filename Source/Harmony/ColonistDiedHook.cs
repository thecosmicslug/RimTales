using HarmonyLib;
using Verse;

namespace RimTales
{
    [HarmonyPatch(typeof(Pawn),nameof(Pawn.Kill))]
    static class ColonistDiedHook
    {
        [HarmonyPrefix]
        static void Prefix(Pawn __instance)
        {

            if (!__instance.IsColonist)
            {
                return;
            }
            
            Resources.EventManager.Add(new ADead(Utils.CurrentDate(), __instance));
            Core.AddIncident("Incident_ColonistDeath", "RT_ColonistDiedIncident".Translate(__instance.Name));

            if (RimTalesMod.settings.bVerboseLogging){
                Log.Message("[RimTales]: ColonistDied() - EventManager.Add(ADead)");
            }

            if (Resources.isMemorialDayCreated)
            {            
                return;
            }

            Resources.EventManager.Add(new AMemorialDay(Utils.CurrentDate(), __instance));
            Resources.isMemorialDayCreated = true;
            
        }
    }
}