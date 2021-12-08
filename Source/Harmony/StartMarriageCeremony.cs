using HarmonyLib;
using RimWorld;
using Verse;

namespace RimTales
{
    [HarmonyPatch(typeof(VoluntarilyJoinableLordsStarter))]
    [HarmonyPatch("TryStartMarriageCeremony")]
    internal class StartMarriageCeremony
    {
        private static void Postfix(ref Pawn firstFiance, ref Pawn secondFiance)
        {
            Log.Message("[RimTales]: VoluntarilyJoinableLordsStarter.TryStartMarriageCeremony()");
            Resources.EventManager.Add(new AMarriage(Utils.CurrentDate(), firstFiance, secondFiance));
        }
    }
}