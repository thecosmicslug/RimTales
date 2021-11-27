using HarmonyLib;
using RimWorld;
using Verse;

namespace RimTales
{
    [HarmonyPatch(typeof(RelationsUtility),nameof(RelationsUtility.TryDevelopBondRelation))]
    static class BondHook
    {
        [HarmonyPostfix]
        static void Postfix(bool __result, ref Pawn humanlike, ref Pawn animal)
        {
            if (!__result)
            {
                return;
            }
            Resources.eventsLog.Add(new ABonded(Utils.CurrentDate(), humanlike, animal));
        }
    }
}