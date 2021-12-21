using System;
using System.Collections.Generic;
using System.Text;
using HugsLib;
using UnityEngine.SceneManagement;
using RimWorld;
using Verse;
using UnityEngine;
using RimWorld.Planet;

namespace RimTales
{

    public class Core : ModBase{

        public override string ModIdentifier {
            get { return "RimTales"; }
        }

        [DefOf]     //* Taken from Vanilla Interactions Expanded.
        public static class RimTales_DefOf{
            //* our events from the hooks.
            public static TaleDef AnniversaryDeath;        
            public static TaleDef AnniversaryMarriage;
            public static TaleDef AnniversaryThreat;
            //* Incident hooks
            public static TaleDef Incident_Raid;
            public static TaleDef Incident_AnimalInsanityMass;
            public static TaleDef Incident_ManhunterPack;
            public static TaleDef Incident_ColdSnap;
            public static TaleDef Incident_Flashstorm;
            public static TaleDef Incident_HeatWave;
            public static TaleDef Incident_FarmAnimalsWanderIn;
            public static TaleDef Incident_Infestation;
            public static TaleDef Incident_WandererJoin;
            public static TaleDef Incident_VolcanicWinter;
            public static TaleDef Incident_ToxicFallout;
        }

        public override void MapLoaded(Map map){

            Logger.Message("Initialisng Tales...");
            //* UNCOMMENT THIS TO WIPE OUR TALES DATA & RE-IMPORT ON LOAD
            //WipeTaleLog();
            Logger.Message("Done! Loaded " + Resources.TaleManager.Count + " tales.");
            Resources.TEST_MAP = map;
            base.MapLoaded(map);

            //* One tick per hour
            HugsLibController.Instance.TickDelayScheduler.ScheduleCallback(() =>{
                //* No point checking if anniversarys are disabled.
                if(RimTalesMod.settings.enableMarriageAnniversary || RimTalesMod.settings.enableMemoryDay || RimTalesMod.settings.enableDaysOfVictory || RimTalesMod.settings.enableIndividualThoughts ){
                    if (Resources.EventManager.Count > 0){
                        foreach (var e in Resources.EventManager){
                            if (e is AMarriage && RimTalesMod.settings.enableMarriageAnniversary){
                                e.TryStartEvent(map);
                            }
                            if (e is AMemorialDay && RimTalesMod.settings.enableMemoryDay){
                                e.TryStartEvent(map);
                            }
                            if (e is ABigThreat && RimTalesMod.settings.enableDaysOfVictory){
                                e.TryStartEvent(map);
                            }
                            if (e is ADead && RimTalesMod.settings.enableIndividualThoughts){
                                e.TryStartEvent(map);
                            }
                        }
                    }
            }

            }, 2500, null, true);

            //* Mass funeral code
            HugsLibController.Instance.TickDelayScheduler.ScheduleCallback(() =>{
                if (Resources.deadPawnsForMassFuneralBuried.Count <= 0 || !RimTalesMod.settings.enableFunerals){
                    return;
                }

                if (Resources.dateLastFuneral == null ||
                    Utils.CurrentDay() != Resources.dateLastFuneral.GetDate().day &&
                    Utils.CurrentQuadrum() != Resources.dateLastFuneral.GetDate().quadrum &&
                    Utils.CurrentYear() != Resources.dateLastFuneral.GetDate().year){
                }

                if (!MassFuneral.TryStartMassFuneral(map)){
                    return;
                }

                Resources.deadPawnsForMassFuneralBuried.Clear();
                Resources.dateLastFuneral = Utils.CurrentDate();
            }, 2500, null, true);
        }

        public override void SceneLoaded(Scene scene){
            //* Dirty hacks for deleting static lists. Don't look. 
            base.SceneLoaded(scene);
            if (!GenScene.InEntryScene){
                return;
            }

            Resources.EventManager.Clear();
            Resources.TaleManager.Clear();
        }

        public override void WorldLoaded(){
            base.WorldLoaded();
            var unused = Find.World.GetComponent<Saves>();
        }

        //* Add String Tales for GUI
        private static void AddTale(Tale tale, String overrideName, String plus){

            StringBuilder str = new StringBuilder();
            Vector2 longitude = Vector2.zero;
            
            if (tale.surroundings != null && tale.surroundings.tile >= 0){
                longitude = Find.WorldGrid.LongLatOf(tale.surroundings.tile);
            }
            
            TaleStorage TaleTMP = new TaleStorage();
            TaleTMP.def = tale.def;

            str.Append(GenDate.DateFullStringAt(tale.date, longitude) + ": ");
            string taleStr = tale.ToString();
            string taleStr2 = taleStr.Split(new[] {"(age"}, StringSplitOptions.None)[0];
            string taleStr3 = taleStr2.Split(',')[0];
            str.Append(taleStr3.Remove(0, taleStr2.IndexOf(':') + 2));
            if (overrideName == ""){
                str.Append(plus);
                TaleTMP.customLabel = str.ToString();
            }
            else{
                string[] temp = str.ToString().Split(':');
                string final = temp[0] + ":" + temp[1];
                var outstr = final + overrideName + plus;
                TaleTMP.customLabel = outstr;

            }

            Resources.TaleManager.Add(TaleTMP);
            if(RimTalesTab.bTabOpen == true){
                RimTalesTab.UpdateList(TaleTMP);
            }

        }

        //* Add Incidents from our hook, that don't have a tale
        public static void AddIncident(TaleDef def, String Text){
            
            //* Work out the date 
            Vector2 val;
            if (WorldRendererUtility.WorldRenderedNow && Find.WorldSelector.selectedTile >= 0){
                val = Find.WorldGrid.LongLatOf(Find.WorldSelector.selectedTile);
            }
            else if (WorldRendererUtility.WorldRenderedNow && Find.WorldSelector.NumSelectedObjects > 0){
                val = Find.WorldGrid.LongLatOf(Find.WorldSelector.FirstSelectedObject.Tile);
            }else{
                if (Find.CurrentMap == null){
                    return;
                }
                val = Find.WorldGrid.LongLatOf(Find.CurrentMap.Tile);
            }
            string StrDate = GenDate.DateFullStringAt(Find.TickManager.TicksAbs, val);

            //* Make our tale object
            TaleStorage TaleTMP = new TaleStorage();
            TaleTMP.def = def;
            TaleTMP.customLabel = StrDate + ": " + Text;

            //* Add it to the collection
            Resources.TaleManager.Add(TaleTMP);

        }

        //* Wipe our log (DEBUGGING)
        public static void WipeTaleLog(){
            Resources.TaleManager.Clear();
            foreach (Tale tale in Find.TaleManager.AllTalesListForReading){
                Core.IncomingTale(tale);
            }
        }

        //* Process Tales, Build String version
        public static void IncomingTale(Tale tale){

            if (Prefs.DevMode){
                Log.Message("[RimTales]: TaleManager.Add() - " + tale.ToString());
            }

            String StrTalePlus = "";
            String StrTaleOverride = "";
            Tale_SinglePawnAndDef tale2 = tale as Tale_SinglePawnAndDef;
            Tale_SinglePawnAndThing tale3 = tale as Tale_SinglePawnAndThing;
            Tale_DoublePawn tale4 = tale as Tale_DoublePawn;
            Tale_DoublePawnAndDef tale5 = tale as Tale_DoublePawnAndDef;

            switch (tale.def.defName)
            {
                case "Vomited":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "LandedInPod":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "Drunk":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "WasOnFire":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "WalkedNaked":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "CollapseDodged":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "CaravanAmbushedByHumanlike":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "CaravanMeeting":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "CaravanDemand":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "CaravanFormed":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "CaravanFled":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "CaravanAmbushDefeated":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "CaravanAssaultSuccessful":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "Meditated":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "Prayed":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "LaunchedShip":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "AttendedConcert":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "HeldConcert":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "Exhausted":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_IngestedHumanFlesh":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_BingedFood":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_BingedDrug":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_HideInRoom":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_ThrewTantrum":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_WanderedInSaddness":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_WentBerserk":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_WentIntoSadisticRage":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_WentOnFireStartingSpree":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_WentOnMurderousRage":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_RanWild":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "ButcheredHumanlikeCorpse":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "AteRawHumanlikeMeat":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "BuiltSnowman":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "EnteredCryptosleep":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "MentalStateBerserk":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "MentalStateGaveUp":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_SlaughteredAnimalInRage":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_TamedThrumbo":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_WasPreviouslyOurEnemy":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_WasBadlyInjured":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_RemovedPrisonersOrgans":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_FailedMedicalOperationAndKilled":
                    //Tale_SinglePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "CaravanRemoteMining":
                    //Tale_SinglePawnAndDef
                    StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "CaravanAmbushedByManhunter":
                    //Tale_SinglePawnAndDef
                    StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;    

                case "PlayedGame":
                    //Tale_SinglePawnAndDef
                    StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "HeatstrokeRevealed":
                    //Tale_SinglePawnAndDef
                    StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "HypothermiaRevealed":
                    //Tale_SinglePawnAndDef
                    StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "ToxicityRevealed":
                    //Tale_SinglePawnAndDef
                    StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "FinishedResearchProject":
                    //Tale_SinglePawnAndDef
                    StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "CompletedLongConstructionProject":
                    //Tale_SinglePawnAndDef
                    StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "IllnessRevealed":
                    //Tale_SinglePawnAndDef
                    StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "MinedValuable":
                    //Tale_SinglePawnAndDef
                    StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "GainedMasterSkillWithPassion":
                    //Tale_SinglePawnAndDef
                    StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "GainedMasterSkillWithoutPassion":
                    //Tale_SinglePawnAndDef
                    StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "DefeatedHostileFactionLeader":
                    //Tale_SinglePawnAndDef
                    StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "CompletedLongCraftingProject":
                    //Tale_SinglePawnAndDef
                    StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "CraftedArt":
                    //* Tale_SinglePawnAndThing ?!?
                    //StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
                    StrTalePlus = " - " + tale3.thingData.thingDef.LabelCap + ".";
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "StruckMineable":
                    //Tale_SinglePawnAndThing
                    StrTalePlus = " - " + tale3.thingData.thingDef.LabelCap + ".";
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "Recruited":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTalePlus = " - Joiner: " + tale4.secondPawnData.name + ".";
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTaleOverride = ": Recruiter: " + tale4.firstPawnData.name;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "Stripped":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTaleOverride = " removed clothing from - " + tale4.secondPawnData.name;
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTalePlus = ": " + tale4.firstPawnData.name;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "IncreasedMenagerie":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && !tale4.secondPawnData.kind.RaceProps.Humanlike){
                        StrTaleOverride = ": " + tale4.secondPawnData.kind.LabelCap;
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTalePlus = " has been tamed by - " + tale4.firstPawnData.name;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "AttendedParty":
                    // Tale_DoublePawn
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTalePlus = " atttended the party of " + tale4.secondPawnData.name + ".";
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTaleOverride = ": " + tale4.firstPawnData.name;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_DidNotAttendWedding":
                    //Tale_DoublePawn
                    //TODO: Finish double-pawn tales.
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_AttendedMyWedding":
                    //Tale_DoublePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_TamedMe":
                    //Tale_DoublePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_ArrestedMe":
                    //Tale_DoublePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_ResurrectedMe":
                    //Tale_DoublePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_BrokeUpWithMe":
                    //Tale_DoublePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_BondedPetButchered":
                    //Tale_DoublePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_ExposedCorpseOfMyFriend":
                    // Tale_DoublePawn
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "Hunted":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTaleOverride = ": " + tale4.secondPawnData.name;
                    }
                    if (tale4.secondPawnData != null && !tale4.secondPawnData.kind.RaceProps.Humanlike){
                        StrTaleOverride = ": " + tale4.secondPawnData.kind.LabelCap;
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTalePlus = " - Hunter: " + tale4.firstPawnData.name;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "KidnappedColonist":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTaleOverride = ": Victim: " + tale4.secondPawnData.name;
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTalePlus = " - Kidnapper: " + tale4.firstPawnData.name + ".";
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "DidSurgery":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTalePlus = " - Patient: " + tale4.secondPawnData.name + ".";
                    }
                    if (tale4.secondPawnData != null && !tale4.secondPawnData.kind.RaceProps.Humanlike){
                        StrTalePlus = " - Patient: " + tale4.secondPawnData.kind.LabelCap + ".";
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "KilledColonist":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTaleOverride = ": " + tale4.secondPawnData.name;
                    }
                    if (tale4.firstPawnData != null && !tale4.firstPawnData.kind.RaceProps.Humanlike){
                        StrTalePlus = " - Killer: " + tale4.firstPawnData.kind.LabelCap + ".";
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTalePlus = " - Killer: " + tale4.firstPawnData.name + ".";
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "KilledMajorThreat":
                    //Tale_DoublePawnAndDef
                    if (tale5.secondPawnData != null && tale5.secondPawnData.name != null){
                        StrTalePlus = " - Enemy: " + tale5.secondPawnData.name + " Weapon: " + tale5.defData.def.LabelCap + ".";
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "TradedWith":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTalePlus = " - Trader: " + tale4.secondPawnData.name + ".";
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_WeHadNiceChat":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTalePlus = " & " + tale4.secondPawnData.name + ".";
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTaleOverride = ": " + tale4.firstPawnData.name;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "BecameLover":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTalePlus = " and " + tale4.secondPawnData.name + ".";
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTaleOverride = ": " + tale4.firstPawnData.name + "";
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "Breakup": 
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTalePlus = " and " + tale4.secondPawnData.name + ".";
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTaleOverride = ": " + tale4.firstPawnData.name + "";
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "Marriage":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTalePlus = " and " + tale4.secondPawnData.name + ".";
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTaleOverride = ": " + tale4.firstPawnData.name + "";
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "Captured":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && !tale4.secondPawnData.kind.RaceProps.Humanlike){
                        StrTalePlus = " - Animal: " + tale4.secondPawnData.kind.LabelCap + ".";
                    }
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTalePlus = " - Prisoner: " + tale4.secondPawnData.name + ".";
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTaleOverride = ": Warden: " + tale4.firstPawnData.name;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "ExecutedPrisoner":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && !tale4.secondPawnData.kind.RaceProps.Humanlike){
                        StrTalePlus = " - Animal: " + tale4.secondPawnData.kind.LabelCap + ".";
                    }
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTalePlus = " - Prisoner: " + tale4.secondPawnData.name + ".";
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTaleOverride = ": Warden: " + tale4.firstPawnData.name;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "SoldPrisoner":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && !tale4.secondPawnData.kind.RaceProps.Humanlike){
                        StrTalePlus = " - Animal: " + tale4.secondPawnData.kind.LabelCap + ".";
                    }
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTalePlus = " - Prisoner: " + tale4.secondPawnData.name + ".";
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTaleOverride = ": Warden: " + tale4.firstPawnData.name;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "TamedAnimal":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && !tale4.secondPawnData.kind.RaceProps.Humanlike){
                        StrTaleOverride = ": " + tale4.secondPawnData.kind.LabelCap;
                    }
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTaleOverride = ": " + tale4.secondPawnData.name;
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTalePlus = " - Handler: " + tale4.firstPawnData.name + ".";
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;
                
                case "TrainedAnimal":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && !tale4.secondPawnData.kind.RaceProps.Humanlike){
                        StrTaleOverride = ": " + tale4.secondPawnData.kind.LabelCap;
                    }
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTaleOverride = ": " + tale4.secondPawnData.name;
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTalePlus = " - Handler: " + tale4.firstPawnData.name + ".";
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;
                
                case "BondedWithAnimal":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && !tale4.secondPawnData.kind.RaceProps.Humanlike){
                        StrTaleOverride = ": " + tale4.secondPawnData.kind.LabelCap;
                    }
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTaleOverride = ": " + tale4.secondPawnData.name;
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTalePlus = " - Handler: " + tale4.firstPawnData.name + ".";
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;
                
                case "KilledColonyAnimal":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && !tale4.secondPawnData.kind.RaceProps.Humanlike){
                        StrTalePlus = " - Animal: " + tale4.secondPawnData.kind.LabelCap + ".";
                    }
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTalePlus = " - Animal: " + tale4.secondPawnData.name + ".";
                    }
                    if (tale4.firstPawnData != null && !tale4.firstPawnData.kind.RaceProps.Humanlike){
                        StrTaleOverride = ": Killer: " + tale4.firstPawnData.kind.LabelCap;
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTaleOverride = ": Killer: " + tale4.firstPawnData.name;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;
                
                case "BuriedCorpse":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && !tale4.secondPawnData.kind.RaceProps.Humanlike){
                        StrTaleOverride = ": " + tale4.secondPawnData.kind.LabelCap;
                    }
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTaleOverride = ": " + tale4.secondPawnData.name;
                    }
                    if (tale4.firstPawnData != null && !tale4.firstPawnData.kind.RaceProps.Humanlike){
                        StrTalePlus = " - Worker: " + tale4.firstPawnData.kind.LabelCap + ".";
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTalePlus = " - Worker: " + tale4.firstPawnData.name + ".";
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VisitedGrave":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && !tale4.secondPawnData.kind.RaceProps.Humanlike){
                        StrTalePlus = " went to visit the grave of  " + tale4.secondPawnData.kind.LabelCap  + ".";
                    }
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTalePlus = " went to visit the grave of " + tale4.secondPawnData.name  + ".";
                    }
                    if (tale4.firstPawnData != null && !tale4.firstPawnData.kind.RaceProps.Humanlike){
                        StrTaleOverride = ": " + tale4.firstPawnData.kind.LabelCap;
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTaleOverride = ": " + tale4.firstPawnData.name;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "PutIntoCryptosleep":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && !tale4.secondPawnData.kind.RaceProps.Humanlike){
                        StrTalePlus = " helped - " + tale4.secondPawnData.kind.LabelCap  + ".";
                    }
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTalePlus = " helped - " + tale4.secondPawnData.name  + ".";
                    }
                    if (tale4.firstPawnData != null && !tale4.firstPawnData.kind.RaceProps.Humanlike){
                        StrTaleOverride = ": " + tale4.firstPawnData.kind.LabelCap;
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTaleOverride = ": " + tale4.firstPawnData.name;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "SocialFight":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTalePlus = " - Attacker: " + tale4.secondPawnData.name + ".";
                    }
                    if (tale4.secondPawnData != null && !tale4.secondPawnData.kind.RaceProps.Humanlike){
                        StrTalePlus = " - Attacker: " + tale4.secondPawnData.kind.LabelCap + ".";
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTaleOverride = ": " + tale4.firstPawnData.name;
                    }
                    if (tale4.firstPawnData != null && !tale4.firstPawnData.kind.RaceProps.Humanlike){
                        StrTaleOverride = ": " + tale4.firstPawnData.kind.LabelCap;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "KilledBy":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTalePlus = " - Attacker: " + tale4.secondPawnData.name + ".";
                    }
                    if (tale4.secondPawnData != null && !tale4.secondPawnData.kind.RaceProps.Humanlike){
                        StrTalePlus = " - Attacker: " + tale4.secondPawnData.kind.LabelCap + ".";
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTaleOverride = ": " + tale4.firstPawnData.name;
                    }
                    if (tale4.firstPawnData != null && !tale4.firstPawnData.kind.RaceProps.Humanlike){
                        StrTaleOverride = ": " + tale4.firstPawnData.kind.LabelCap;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "KilledMortar":
                    //Tale_DoublePawnAndDef
                    if (tale5.secondPawnData != null && tale5.secondPawnData.name != null){
                        StrTalePlus = " killed - " + tale5.secondPawnData.name + ".";
                    }
                    if (tale5.secondPawnData != null && !tale5.secondPawnData.kind.RaceProps.Humanlike){
                        StrTalePlus = " killed - " + tale5.secondPawnData.kind.LabelCap + ".";
                    }
                    if (tale5.firstPawnData != null && tale5.firstPawnData.name != null){
                        StrTaleOverride = ": " + tale5.firstPawnData.name;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "KilledLongRange":
                    //Tale_DoublePawnAndDef
                    if (tale5.secondPawnData != null && tale5.secondPawnData.name != null){
                        StrTalePlus = " killed " + tale5.secondPawnData.name + " with " + tale5.defData.def.LabelCap + ".";
                    }
                    if (tale5.secondPawnData != null && !tale5.secondPawnData.kind.RaceProps.Humanlike){
                        StrTalePlus = " killed " + tale5.secondPawnData.kind.LabelCap + " with " + tale5.defData.def.LabelCap + ".";
                    }
                    if (tale5.firstPawnData != null && tale5.firstPawnData.name != null){
                        StrTaleOverride = ": " + tale5.firstPawnData.name;
                    }
                    if (tale5.firstPawnData != null && !tale5.firstPawnData.kind.RaceProps.Humanlike){
                        StrTaleOverride = ": " + tale5.firstPawnData.kind.LabelCap;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;
                
                case "KilledMelee":
                    //Tale_DoublePawnAndDef
                    if (tale5.secondPawnData != null && tale5.secondPawnData.name != null){
                        StrTalePlus = " killed " + tale5.secondPawnData.name + " with " + tale5.defData.def.LabelCap + ".";
                    }
                    if (tale5.secondPawnData != null && !tale5.secondPawnData.kind.RaceProps.Humanlike){
                        StrTalePlus = " killed " + tale5.secondPawnData.kind.LabelCap + " with " + tale5.defData.def.LabelCap + ".";
                    }
                    if (tale5.firstPawnData != null && tale5.firstPawnData.name != null){
                        StrTaleOverride = ": " + tale5.firstPawnData.name;
                    }
                    if (tale5.firstPawnData != null && !tale5.firstPawnData.kind.RaceProps.Humanlike){
                        StrTaleOverride = ": " + tale5.firstPawnData.kind.LabelCap;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "KilledCapacity":
                    //Tale_DoublePawnAndDef
                    if (tale5.secondPawnData != null && tale5.secondPawnData.name != null){
                        StrTalePlus = " killed " + tale5.secondPawnData.name + " with " + tale5.defData.def.LabelCap + ".";
                    }
                    if (tale5.secondPawnData != null && !tale5.secondPawnData.kind.RaceProps.Humanlike){
                        StrTalePlus = " killed " + tale5.secondPawnData.kind.LabelCap + " with " + tale5.defData.def.LabelCap + ".";
                    }
                    if (tale5.firstPawnData != null && tale5.firstPawnData.name != null){
                        StrTaleOverride = ": " + tale5.firstPawnData.name;
                    }
                    if (tale5.firstPawnData != null && !tale5.firstPawnData.kind.RaceProps.Humanlike){
                        StrTaleOverride = ": " + tale5.firstPawnData.kind.LabelCap;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "GaveBirth":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && !tale4.secondPawnData.kind.RaceProps.Humanlike){
                        StrTaleOverride = ": " + tale4.secondPawnData.kind.LabelCap;
                    }
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTaleOverride = ": " + tale4.secondPawnData.name;
                    }
                    if (tale4.firstPawnData != null && !tale4.firstPawnData.kind.RaceProps.Humanlike){
                        StrTalePlus = " - Mother: " + tale4.firstPawnData.kind.LabelCap  + ".";
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTalePlus = " - Mother: " + tale4.firstPawnData.name + ".";
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;
                
                case "Wounded":
                    //Tale_DoublePawnAndDef
                    if (tale5.secondPawnData != null && tale5.secondPawnData.name != null){
                        StrTalePlus = " - Attacker: " + tale5.secondPawnData.name + " with " + tale5.defData.def.LabelCap + ".";
                    }
                    else if (tale5.secondPawnData != null && !tale5.secondPawnData.kind.RaceProps.Humanlike){
                        StrTalePlus = " - Attacker: " + tale5.secondPawnData.kind.LabelCap + " with " + tale5.defData.def.LabelCap + ".";
                    }
                    if (tale5.firstPawnData != null && tale5.firstPawnData.name != null){
                        StrTaleOverride = ": " + tale5.firstPawnData.name;
                    }
                    else if (tale5.firstPawnData != null && !tale5.firstPawnData.kind.RaceProps.Humanlike){
                        StrTaleOverride = ": " + tale5.firstPawnData.kind.LabelCap;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "Downed":
                    //Tale_DoublePawnAndDef
                    if (tale5.secondPawnData != null && tale5.secondPawnData.name != null){
                        StrTalePlus = " - Attacker: " + tale5.secondPawnData.name + tale5.defData.def.LabelCap + ".";
                    }
                    if (tale5.secondPawnData != null && !tale5.secondPawnData.kind.RaceProps.Humanlike){
                        StrTalePlus = " - Attacker: " + tale5.secondPawnData.kind.LabelCap + tale5.defData.def.LabelCap + ".";
                    }
                    if (tale5.firstPawnData != null && tale5.firstPawnData.name != null){
                        StrTaleOverride = ": " + tale5.firstPawnData.name;
                    }
                    if (tale5.firstPawnData != null && !tale5.firstPawnData.kind.RaceProps.Humanlike){
                        StrTaleOverride = ": " + tale5.firstPawnData.kind.LabelCap;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;
                
                case "VSIE_InsultedMe":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTalePlus = " - Attacker: " + tale4.secondPawnData.name + ".";
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTaleOverride = ": " + tale4.firstPawnData.name;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_RebuffedMe":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTalePlus = " - Attacker: " + tale4.secondPawnData.name + ".";
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTaleOverride = ": " + tale4.firstPawnData.name;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_WeHadSocialFight":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTalePlus = " - Attacker: " + tale4.secondPawnData.name + ".";
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTaleOverride = ": " + tale4.firstPawnData.name;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_SavedMeFromMyWounds":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTalePlus = " - Friend: " + tale4.secondPawnData.name + ".";
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTaleOverride = ": " + tale4.firstPawnData.name;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_HasBeenMyFriendSinceChildhood":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && tale4.secondPawnData.name != null){
                        StrTalePlus = " - Friend: " + tale4.secondPawnData.name + ".";
                    }
                    if (tale4.firstPawnData != null && tale4.firstPawnData.name != null){
                        StrTaleOverride = ": " + tale4.firstPawnData.name;
                    }
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_SavedMeFromRaiders":
                    // TODO: Add triple-pawn tales.
                    //* Triple-pawn  tale
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_StoleMyLover":
                    //* Triple-pawn  tale
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "VSIE_CuredMyFriend":
                    //* Triple-pawn  tale
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;

                case "MajorThreat":
                    //* Duplicate ??
                    break;
                    
                case "Raid":
                    //* Duplicate ??
                    break;

                case "ToxicFallout":
                    //* Duplicate ??
                    break;

                case "VolcanicWinter":
                    //* Duplicate ??
                    break;

                case "Flashstorm":
                    //* Duplicate ??
                    break;

                case "Infestation":
                    //* Duplicate ??
                    break;

                case "ManhunterPack":
                    //* Duplicate ??
                    break;

                case "Eclipse":
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;
    
                case "Aurora":
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;
    
                case "MeteoriteImpact":
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;
    
                case "ShipPartCrash":
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;
    
                case "EndGame_ShipEscape":
                    AddTale(tale,StrTaleOverride,StrTalePlus);
                    break;
    
                default:
                    //* We Shouldn't end up here, log the tale if debug-mode
                    if (Prefs.DevMode){
                        Log.Message("[RimTales]:============UNKNOWN-TALE===========");
                        Log.Message("[RimTales]:(TOSTRING) " + tale.ToString());
                        Log.Message("[RimTales]:(DEF) " +  tale.def);
                        Log.Message("[RimTales]:===================================");
                    }
                    break;

                    

            }

        }

    }
}