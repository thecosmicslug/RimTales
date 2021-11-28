using HarmonyLib;
using RimWorld;
using Verse;

namespace RimTales
{
    [HarmonyPatch(typeof(InteractionWorker_Breakup))]
    [HarmonyPatch("RandomBreakupReason")]
    internal class RandomBreakup
    {
        private static void Postfix(Thought __result, ref Pawn initiator, ref Pawn recipient)
        {
            Log.Message("RimTales InteractionWorker_Breakup.RandomBreakupReason(): " + __result);
        }
    }
}