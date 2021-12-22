using HarmonyLib;
using RimWorld;
using Verse;

namespace RimTales
{
    [HarmonyPatch(typeof(IncidentWorker))]
    [HarmonyPatch("TryExecute")]
    internal class IncidentWorkerHookShort
    {

        private static void Postfix(IncidentWorker __instance, bool __result)
        {
            if (!__result){
                return;
            }

            if (__instance is IncidentWorker_AnimalInsanityMass){
                // TODO: Add Animal Details for Incident_AnimalInsanityMass.
                Core.AddIncident(Core.RimTales_DefOf.Incident_AnimalInsanityMass, "RT_AnimalInsanityMass".Translate());
            }

            if (__instance is IncidentWorker_ManhunterPack){
                // TODO: Add Animal Details for Incident_ManhunterPack.
                Core.AddIncident(Core.RimTales_DefOf.Incident_ManhunterPack, "RT_ManhunterPack".Translate());
            }

            if (__instance is IncidentWorker_ColdSnap){
                Core.AddIncident(Core.RimTales_DefOf.Incident_ColdSnap, "RT_ColdSnap".Translate());
            }

            if (__instance is IncidentWorker_HeatWave){
                Core.AddIncident(Core.RimTales_DefOf.Incident_HeatWave, "RT_HeatWave".Translate());
            }

            if (__instance is IncidentWorker_FarmAnimalsWanderIn){
                // TODO: Add Animal Details for Incident_FarmAnimalsWanderIn.
                Core.AddIncident(Core.RimTales_DefOf.Incident_FarmAnimalsWanderIn, "RT_FarmAnimalsWanderIn".Translate());
            }

            if (__instance is IncidentWorker_Infestation){
                // TODO: Add Details for Incident_Infestation.
                Core.AddIncident(Core.RimTales_DefOf.Incident_Infestation, "RT_Infestation".Translate());
            }

            if (__instance is IncidentWorker_WandererJoin){
                // TODO: Add Pawn details for Incident_WandererJoin.
                Core.AddIncident(Core.RimTales_DefOf.Incident_WandererJoin, "RT_WandererJoin".Translate());
            }

            if (__instance.def.defName == "VolcanicWinter"){
                Core.AddIncident(Core.RimTales_DefOf.Incident_VolcanicWinter, "RT_VolcanicWinter".Translate());
            }

            if (__instance.def.defName == "ToxicFallout"){
                Core.AddIncident(Core.RimTales_DefOf.Incident_ToxicFallout, "RT_ToxicFallout".Translate());
            }

            if (__instance.def.defName == "Flashstorm"){
                Core.AddIncident(Core.RimTales_DefOf.Incident_Flashstorm, "RT_FlashStorm".Translate());
            }
        }
    }
}