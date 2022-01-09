using HarmonyLib;
using RimWorld;
using Verse;

namespace RimTales
{
    [HarmonyPatch(typeof(IncidentWorker_PsychicSoothe))]
    [HarmonyPatch("DoConditionAndLetter")]
    internal class PsychicSootheHook
    {
        private static void Postfix(IncidentParms parms, Map map, int duration, Gender gender, float points)
        {
            //* Positive mood effects.
            Core.AddIncident("Incident_PsychicSoothe", "RT_PsychicSoothe".Translate(gender.GetLabel()));
        }
    }

    [HarmonyPatch(typeof(IncidentWorker_PsychicDrone))]
    [HarmonyPatch("DoConditionAndLetter")]
    internal class PsychicDroneHook
    {
        private static void Postfix(IncidentParms parms, Map map, int duration, Gender gender, float points)
        {
            //* Negative mood effects.
            Core.AddIncident("Incident_PsychicDrone", "RT_PsychicDrone".Translate(gender.GetLabel()));
        }
    }
}