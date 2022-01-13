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

            //	public class IncidentWorker_MeteoriteImpact : IncidentWorker
            // 	SendStandardLetter(def.letterLabel + ": " + list[0].def.LabelCap, text, baseLetterDef, parms, new TargetInfo(cell, map));


            //* These incidents don't have any extra details included that interest us.
            if (__instance is IncidentWorker_Infestation){
                Core.AddIncident("Incident_Infestation", "RT_Infestation".Translate());
            }

            if (__instance is IncidentWorker_CropBlight){
                Core.AddIncident("Incident_CropBlight", "RT_CropBlight".Translate());
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

            if (__instance is IncidentWorker_ResourcePodCrash){
                Core.AddIncident("Incident_ResourcePodCrash", "RT_ResourcePodCrash".Translate());
            }

            if (__instance.def.defName == "Alphabeavers"){
                Core.AddIncident("Incident_Alphabeavers", "RT_Alphabeavers".Translate());
            }

            if (__instance.def.defName == "SolarFlare"){
                Core.AddIncident("Incident_SolarFlare", "RT_SolarFlare".Translate());
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