﻿using HarmonyLib;
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
           Resources.EventManager.Add(new AMarriage(Utils.CurrentDate(), firstFiance, secondFiance));
           Log.Message("[RimTales]: EventManager.Add(AMarriage)");

        }
    }
}