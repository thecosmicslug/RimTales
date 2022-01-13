using System.Collections.Generic;
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

    [HarmonyPatch(typeof(IncidentWorker_VisitorGroup))]
    [HarmonyPatch("SendLetter")]
    internal class VisitorGroupHook
    {
        private static void Postfix(IncidentParms parms, List<Pawn> pawns, Pawn leader, bool traderExists)
        {
            string StrOutput = "";
            if (pawns.Count == 1)
			{
                //* Single Pawn
                StrOutput = "RT_VisitorSingle".Translate(parms.faction.Name,pawns[0]);
                if (traderExists){
                    StrOutput = StrOutput + "RT_Trader".Translate();
                }
                Core.AddIncident("Incident_VisitorSingle", StrOutput);
			}else{
                //* A Group
                StrOutput = "RT_VisitorGroup".Translate(parms.faction.Name);
                if (traderExists){
                    StrOutput = StrOutput + "RT_Trader".Translate();
                }
                Core.AddIncident("Incident_VisitorGroup", StrOutput);
			}

        }
    }

}