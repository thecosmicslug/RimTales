using HarmonyLib;
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
            //* 1/10 chance to add an aniversary fpr the raid                            
            if (Resources.rng.Next(101) <= Resources.randomChanceRaid)
            {
                //* Random chance to add anniversary
                Resources.EventManager.Add(new ABigThreat(Utils.CurrentDate(), parms.faction));
                Log.Message("[RimTales]: EventManager.Add(ABigThreat) - " + parms.faction.Name);
            }
            
            Core.AddIncident(Core.RimTales_DefOf.Incident_Raid, "RT_ColonyAttacked".Translate(parms.faction.Name));
        }
    }
}