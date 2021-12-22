﻿using HarmonyLib;
using RimWorld;
using Verse;

namespace RimTales
{
    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy))]
    [HarmonyPatch("TryExecuteWorker")]
    internal class RaidHook
    {
        private static void Postfix(IncidentParms parms)
        {
            if (Resources.rng.Next(101) <= Resources.randomChanceRaid)
            {
                Resources.EventManager.Add(new ABigThreat(Utils.CurrentDate(), parms.faction));
            }
            
            Core.AddIncident(Core.RimTales_DefOf.Incident_Raid, "RT_ColonyAttacked".Translate(parms.faction.Name));
        }
    }
}