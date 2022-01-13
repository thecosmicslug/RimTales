using HarmonyLib;
using RimWorld;
using Verse;

namespace RimTales
{
    [HarmonyPatch(typeof(ShortCircuitUtility))]
    [HarmonyPatch("DoShortCircuit")]
    internal class ShortCircuitHook
    {
        private static void Postfix(Building culprit)
        {
            Core.AddIncident("ShortCircuit", "RT_ShortCircuit".Translate(culprit.Label));
        }
    }
}