using HarmonyLib;
using RimWorld;
using Verse;

namespace RimTales
{
    [HarmonyPatch(typeof(GameCondition))]
    [HarmonyPatch("Init")]
    internal class IncidentWorkerHookLong
    {
        private static void Postfix(IncidentWorker __instance)
        {
            Log.Message("RimTales: GameCondition.Init() - " + $"it works {__instance.def.defName}");
        }
    }
}