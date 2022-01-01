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

            // IncidentWorker_AnimalInsanitySingle
            // IncidentWorker_AnimaTreeSpawn
            // IncidentWorker_CaravanArrivalTributeCollector
            // IncidentWorker_TraderCaravanArrival
            // IncidentWorker_CaravanDemand
            // IncidentWorker_CaravanMeeting
            // IncidentWorker_CropBlight
            // IncidentWorker_DeepDrillInfestation
            // IncidentWorker_DiseaseAnimal
            // IncidentWorker_DiseaseHuman
            // IncidentWorker_GauranlenPodSpawn
            // IncidentWorker_HerdMigration
            // IncidentWorker_InsectJelly
            // IncidentWorker_MechCluster
            // IncidentWorker_OrbitalTraderArrival
            // IncidentWorker_PsychicDrone
            // IncidentWorker_PsychicSoothe
            // IncidentWorker_RaidFriendly
            // IncidentWorker_RansomDemand
            // IncidentWorker_SelfTame
            // IncidentWorker_ThrumboPasses
            // IncidentWorker_TravelerGroup
            // IncidentWorker_VisitorGroup
            // IncidentWorker_WanderersSkylanterns
            // IncidentWorker_WildManWandersIn

            Log.Message("[RimTales]: IncidentWorker.TryExecute(): defName=" + __instance.def.defName);
            Log.Message("[RimTales]: IncidentWorker.TryExecute(): durationDays=" + __instance.def.durationDays);
            Log.Message("[RimTales]: IncidentWorker.TryExecute(): category=" + __instance.def.category);
            Log.Message("[RimTales]: IncidentWorker.TryExecute(): letterLabel=" + __instance.def.letterLabel);
            Log.Message("[RimTales]: IncidentWorker.TryExecute(): letterText=" +  __instance.def.letterText);
            Log.Message("[RimTales]: IncidentWorker.TryExecute(): ToString=" + __instance.ToString());

            if (__instance is IncidentWorker_AnimalInsanityMass){
                // TODO: Add Animal Details for Incident_AnimalInsanityMass.
                Core.AddIncident(Core.RimTales_DefOf.Incident_AnimalInsanityMass, "RT_AnimalInsanityMass".Translate());
            }

            if (__instance is IncidentWorker_ManhunterPack){
                // TODO: Add Animal Details for Incident_ManhunterPack.
                Core.AddIncident(Core.RimTales_DefOf.Incident_ManhunterPack, "RT_ManhunterPack".Translate());
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

            if (__instance is IncidentWorker_AmbrosiaSprout){
                Core.AddIncident(Core.RimTales_DefOf.Incident_AmbrosiaSprout, "RT_AmbrosiaSprout".Translate());
            }

            if (__instance is IncidentWorker_ShortCircuit){
                Core.AddIncident(Core.RimTales_DefOf.Incident_ShortCircuit, "RT_ShortCircuit".Translate());
            }
            
            if (__instance is IncidentWorker_ResourcePodCrash){
                Core.AddIncident(Core.RimTales_DefOf.Incident_ResourcePodCrash, "RT_ResourcePodCrash".Translate());
            }

            if (__instance is IncidentWorker_ColdSnap){
                Core.AddIncident(Core.RimTales_DefOf.Incident_ColdSnap, "RT_ColdSnap".Translate());
            }

            if (__instance is IncidentWorker_HeatWave){
                Core.AddIncident(Core.RimTales_DefOf.Incident_HeatWave, "RT_HeatWave".Translate());
            }

            if (__instance is IncidentWorker_Flashstorm){
                Core.AddIncident(Core.RimTales_DefOf.Incident_Flashstorm, "RT_FlashStorm".Translate());
            }

            //* Extras
            if (__instance.def.defName == "Alphabeavers"){
                Core.AddIncident(Core.RimTales_DefOf.Incident_Alphabeavers, "RT_Alphabeavers".Translate());
            }

            if (__instance.def.defName == "VolcanicWinter"){
                Core.AddIncident(Core.RimTales_DefOf.Incident_VolcanicWinter, "RT_VolcanicWinter".Translate());
            }

            if (__instance.def.defName == "ToxicFallout"){
                Core.AddIncident(Core.RimTales_DefOf.Incident_ToxicFallout, "RT_ToxicFallout".Translate());
            }

        }
    }
}