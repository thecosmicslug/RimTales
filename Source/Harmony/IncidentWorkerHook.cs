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
                // TODO: Turn into a Harmony hook for Incident_AnimalInsanityMass.
                // Incident_AnimalInsanitySingle is handled by IncidentWorker_AnimalInsanityMass now!
                Core.AddIncident("Incident_AnimalInsanityMass", "RT_AnimalInsanityMass".Translate());
            }

            if (__instance is IncidentWorker_ManhunterPack){
                // TODO: Turn into a Harmony hook for Incident_ManhunterPack.
                Core.AddIncident("Incident_ManhunterPack", "RT_ManhunterPack".Translate());
            }

            if (__instance is IncidentWorker_FarmAnimalsWanderIn){
                // TODO: Turn into a Harmony hook for Incident_FarmAnimalsWanderIn.
                Core.AddIncident("Incident_FarmAnimalsWanderIn", "RT_FarmAnimalsWanderIn".Translate());
            }

            if (__instance is IncidentWorker_Infestation){
                // TODO: Turn into a Harmony hook for Incident_Infestation.
                Core.AddIncident("Incident_Infestation", "RT_Infestation".Translate());
            }

            if (__instance is IncidentWorker_WandererJoin){
                // TODO: Turn into a Harmony hook for Incident_WandererJoin.
                Core.AddIncident("Incident_WandererJoin", "RT_WandererJoin".Translate());
            }
            
            if (__instance is IncidentWorker_SelfTame){
                // TODO: #3 Turn into a Harmony hook for IncidentWorker_SelfTame.
                Core.AddIncident("Incident_SelfTame", "RT_SelfTame".Translate());
            }

            if (__instance is IncidentWorker_WildManWandersIn){
                // TODO: Turn into a Harmony hook for IncidentWorker_WildManWandersIn.
                Core.AddIncident("Incident_WildManWandersIn", "RT_WildManWandersIn".Translate());
            }

            if (__instance is IncidentWorker_HerdMigration){
                // TODO: Turn into a Harmony hook for IncidentWorker_HerdMigration.
                Core.AddIncident("Incident_HerdMigration", "RT_HerdMigration".Translate());
            }

            if (__instance is IncidentWorker_CropBlight){
                // TODO: Turn into a Harmony hook for IncidentWorker_CropBlight.
                Core.AddIncident("Incident_CropBlight", "RT_CropBlight".Translate());
            }
            
            //* These ones don't have any extra detail included?
            if (__instance is IncidentWorker_ResourcePodCrash){
                Core.AddIncident("Incident_ResourcePodCrash", "RT_ResourcePodCrash".Translate());
            }

            if (__instance is IncidentWorker_ColdSnap){
                Core.AddIncident("Incident_ColdSnap", "RT_ColdSnap".Translate());
            }

            if (__instance is IncidentWorker_HeatWave){
                Core.AddIncident("Incident_HeatWave", "RT_HeatWave".Translate());
            }

            if (__instance is IncidentWorker_Flashstorm){
                Core.AddIncident("Incident_Flashstorm", "RT_FlashStorm".Translate());
            }

            if (__instance is IncidentWorker_ThrumboPasses){
                Core.AddIncident("Incident_ThrumboPasses", "RT_ThrumboPasses".Translate());
            }

            if (__instance is IncidentWorker_AmbrosiaSprout){
                Core.AddIncident("Incident_AmbrosiaSprout", "RT_AmbrosiaSprout".Translate());
            }

            if (__instance is IncidentWorker_ShortCircuit){
                Core.AddIncident("ShortCircuit", "RT_ShortCircuit".Translate());
            }

            //* Extras
            if (__instance.def.defName == "SolarFlare"){
                Core.AddIncident("Incident_SolarFlare", "RT_SolarFlare".Translate());
            }

            if (__instance.def.defName == "Alphabeavers"){
                Core.AddIncident("Incident_Alphabeavers", "RT_Alphabeavers".Translate());
            }

            if (__instance.def.defName == "VolcanicWinter"){
                Core.AddIncident("Incident_VolcanicWinter", "RT_VolcanicWinter".Translate());
            }

            if (__instance.def.defName == "ToxicFallout"){
                Core.AddIncident("Incident_ToxicFallout", "RT_ToxicFallout".Translate());
            }

        }
    }
}