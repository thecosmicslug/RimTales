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
                Log.Message("[RimTales]: IncidentWorker_RaidEnemy.TryExecuteWorker() - RAID: " + parms.faction.Name );
            }

            //* what is the IF branch doing?!
            Log.Message("[RimTales]: IncidentWorker_RaidEnemy.TryExecuteWorker() - RAID(2): " + parms.faction.Name);
            Core.AddIncident(Core.RimTales_DefOf.Incident_Raid, "Enemy Raid: " + parms.faction.Name + " are launching an attack on your colony.");
        }
    }
}