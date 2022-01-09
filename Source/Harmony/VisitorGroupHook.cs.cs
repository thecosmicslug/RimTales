using HarmonyLib;
using RimWorld;
using Verse;

namespace RimTales
{

    [HarmonyPatch(typeof(IncidentWorker_TravelerGroup))]
    [HarmonyPatch("TryExecuteWorker")]
    internal class TravelerGroupHook
    {
        private static void Postfix(IncidentParms parms, bool __result)
        {
            if (!__result){
                return;
            }
            //* Visitors just passing through.
            Core.AddIncident(Core.RimTales_DefOf.Incident_TravelerGroup, "RT_TravelerGroup".Translate(parms.faction.Name));
        }
    }

    [HarmonyPatch(typeof(IncidentWorker_VisitorGroup))]
    [HarmonyPatch("TryExecuteWorker")]
    internal class VisitorGroupHook
    {
        private static void Postfix(IncidentParms parms, bool __result)
        {
            if (!__result){
                return;
            }
            //* Visitors that stay a while.
            Core.AddIncident(Core.RimTales_DefOf.Incident_VisitorGroup, "RT_VisitorGroup".Translate(parms.faction.Name));
        }
    }
}