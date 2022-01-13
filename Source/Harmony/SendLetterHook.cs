using System;
using HarmonyLib;
using RimWorld;
using Verse;
using RimWorld.Planet;

namespace RimTales
{
    [HarmonyPatch(typeof(IncidentWorker),"SendStandardLetter", new Type[] {typeof(TaggedString), typeof(TaggedString), typeof(LetterDef), typeof(IncidentParms), typeof(LookTargets), typeof(NamedArgument[])})]
    internal class SendStandardLetterHook
    {
        private static void Postfix(IncidentWorker __instance, TaggedString baseLetterLabel, TaggedString baseLetterText, LetterDef baseLetterDef, IncidentParms parms, LookTargets lookTargets, params NamedArgument[] textArgs)
        {
            Pawn targetPawn = null ;
   
            switch (__instance.def.defName)
            {
                case "AnimalInsanityMass":
                    targetPawn = lookTargets.TryGetPrimaryTarget().Thing as Pawn;
                    if (targetPawn != null){
                        Core.AddIncident("Incident_AnimalInsanityMass", "RT_AnimalInsanityMass".Translate(targetPawn.LabelCap));
                    }
                    break;

                case "AnimalInsanitySingle":
                    targetPawn = lookTargets.TryGetPrimaryTarget().Thing as Pawn;
                    if (targetPawn != null){
                        Core.AddIncident("Incident_AnimalInsanitySingle", "RT_AnimalInsanitySingle".Translate(targetPawn.LabelCap));
                    }
                    break;
                case "WildManWandersIn":
                    targetPawn = lookTargets.TryGetPrimaryTarget().Thing as Pawn;
                    if (targetPawn != null){
                        if (targetPawn.gender == Gender.Male){
                            Core.AddIncident("Incident_WildManWandersIn", "RT_WildManWandersIn".Translate(targetPawn.Name.ToString()));
                        }else{
                            Core.AddIncident("Incident_WildWomanWandersIn", "RT_WildWomanWandersIn".Translate(targetPawn.Name.ToString()));
                        }
                    }
 
                    break; 

                case "WandererJoin":
                    //* FIXME: Not hooking here, need to find another way to trigger this.
                    //targetPawn = lookTargets.TryGetPrimaryTarget().Thing as Pawn;
                    //if (targetPawn != null){
                        //strOutput = targetPawn.Name.ToString();
                    //}
                    //if (strOutput != ""){
                    //    Core.AddIncident("Incident_WandererJoin", "RT_WandererJoin".Translate(strOutput));
                    //}else{
                    //    Core.AddIncident("Incident_WandererJoin", "RT_WandererJoin2".Translate());
                    //}
                    break;
                
                case "StrangerInBlackJoin":
                    targetPawn = lookTargets.TryGetPrimaryTarget().Thing as Pawn;
                    if (targetPawn != null){
                        if (targetPawn.gender == Gender.Male){
                            Core.AddIncident("Incident_StrangerInBlackJoin", "RT_ManInBlackJoin".Translate(targetPawn.Name.ToString()));
                        }else{
                            Core.AddIncident("Incident_StrangerInBlackJoin", "RT_WomanInBlackJoin".Translate(targetPawn.Name.ToString()));
                        }
                    }
                    break;

                case "FarmAnimalsWanderIn":
                    targetPawn = lookTargets.TryGetPrimaryTarget().Thing as Pawn;
                    if (targetPawn != null){
                        Core.AddIncident("Incident_FarmAnimalsWanderIn", "RT_FarmAnimalsWanderIn".Translate(targetPawn.LabelCap));
                    }
                    break;

                case "ManhunterPack":
                    targetPawn = lookTargets.TryGetPrimaryTarget().Thing as Pawn;
                    if (targetPawn != null){
                        Core.AddIncident("Incident_ManhunterPack", "RT_ManhunterPack".Translate(targetPawn.LabelCap));
                    }
                    break;

                case "SelfTame":
                    targetPawn = lookTargets.TryGetPrimaryTarget().Thing as Pawn;
                    if (targetPawn != null){
                        Core.AddIncident("Incident_SelfTame", "RT_SelfTame".Translate(targetPawn.LabelCap));
                    }
                    break;

                case "HerdMigration":
                    targetPawn = lookTargets.TryGetPrimaryTarget().Thing as Pawn;
                    if (targetPawn != null){
                        Core.AddIncident("Incident_HerdMigration", "RT_HerdMigration".Translate(targetPawn.LabelCap));
                    }
                    break;

                default:
                    break;
            }

        }
    }

}