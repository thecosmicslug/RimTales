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
            Core.AddIncident("Incident_TravelerGroup", "RT_TravelerGroup".Translate(parms.faction.Name));
        }
    }

    //IncidentWorker_VisitorGroup.SendLetter(IncidentParms parms, List<Pawn> pawns, Pawn leader, bool traderExists)
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
            Core.AddIncident("Incident_VisitorGroup", "RT_VisitorGroup".Translate(parms.faction.Name));
        }
    }
}