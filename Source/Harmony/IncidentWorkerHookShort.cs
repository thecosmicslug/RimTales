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
                Log.Message("[RimTales]: IncidentWorker.TryExecute() - " + $"RS_{__instance.def.defName}");
                Core.AddIncident(Core.RimTales_DefOf.Incident_AnimalInsanityMass, "(AnimalInsanityMass)");
            }

            if (__instance is IncidentWorker_ManhunterPack){
                Log.Message("[RimTales]: IncidentWorker.TryExecute() - " + $"RS_{__instance.def.defName}");
                Core.AddIncident(Core.RimTales_DefOf.Incident_ManhunterPack, "(ManhunterPack)");
            }

            if (__instance is IncidentWorker_ColdSnap){
                //* Use a Tale because there is no extra details needed
                Log.Message("[RimTales]: IncidentWorker.TryExecute() - " + $"RS_{__instance.def.defName}");
                Core.AddIncident(Core.RimTales_DefOf.Incident_ColdSnap, "(ColdSnap)");
            }

            if (__instance is IncidentWorker_HeatWave){
                //* Use a Tale because there is no extra details needed
                Log.Message("[RimTales]: IncidentWorker.TryExecute() - " + $"RS_{__instance.def.defName}");
                Core.AddIncident(Core.RimTales_DefOf.Incident_HeatWave, "(HeatWave)");
            }

            if (__instance is IncidentWorker_FarmAnimalsWanderIn){
                Log.Message("[RimTales]: IncidentWorker.TryExecute() - " + $"RS_{__instance.def.defName}");
                Core.AddIncident(Core.RimTales_DefOf.Incident_FarmAnimalsWanderIn, "(FarmAnimalsWanderIn)");
            }

            if (__instance is IncidentWorker_Infestation){
                Log.Message("[RimTales]: IncidentWorker.TryExecute() - " + $"RS_{__instance.def.defName}");
                Core.AddIncident(Core.RimTales_DefOf.Incident_Infestation, "(Infestation)");
            }

            if (__instance is IncidentWorker_WandererJoin){
                Log.Message("[RimTales]: IncidentWorker.TryExecute() - " + $"RS_{__instance.def.defName}");
                Core.AddIncident(Core.RimTales_DefOf.Incident_WandererJoin, "(WandererJoin)");
            }

            if (__instance.def.defName == "VolcanicWinter"){
                //* Use a Tale because there is no extra details needed
                Log.Message("[RimTales]: IncidentWorker.TryExecute() - " + $"RS_{__instance.def.defName}");
                Core.AddIncident(Core.RimTales_DefOf.Incident_VolcanicWinter, "(VolcanicWinter)");
            }

            if (__instance.def.defName == "ToxicFallout"){
                //* Use a Tale because there is no extra details needed
                Log.Message("[RimTales]: IncidentWorker.TryExecute() - " + $"RS_{__instance.def.defName}");
                Core.AddIncident(Core.RimTales_DefOf.Incident_ToxicFallout, "(ToxicFallout)");
            }

            if (__instance.def.defName == "Flashstorm"){
                //* Use a Tale because there is no extra details needed
                Log.Message("[RimTales]: IncidentWorker.TryExecute() - " + $"RS_{__instance.def.defName}");
                Core.AddIncident(Core.RimTales_DefOf.Incident_Flashstorm, "(Flashstorm)");
            }
        }
    }
}