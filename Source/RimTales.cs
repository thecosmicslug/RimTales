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
            // WipeTaleLog();

            if(Resources.TaleManager.Count > 0){
                Logger.Message("Done! Loaded " + Resources.TaleManager.Count + " tales.");
            }

            Resources.TEST_MAP = map;
            base.MapLoaded(map);

            //* One tick per hour for the anniversaries
            HugsLibController.Instance.TickDelayScheduler.ScheduleCallback(() =>{
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
        private static void AddTale(Tale tale, String overrideName){
            
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
                TaleTMP.customLabel = str.ToString();
            }
            else{
                string[] temp = str.ToString().Split(':');
                string final = temp[0] + ":" + temp[1];
                var outstr = final + overrideName;
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

            Tale_SinglePawnAndDef tale2 = tale as Tale_SinglePawnAndDef;
            Tale_SinglePawnAndThing tale3 = tale as Tale_SinglePawnAndThing;
            Tale_DoublePawn tale4 = tale as Tale_DoublePawn;
            Tale_DoublePawnAndDef tale5 = tale as Tale_DoublePawnAndDef;
            Tale_TriplePawn tale6 = tale as Tale_TriplePawn;

            switch (tale.def.defName)
            {
                case "Vomited":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "LandedInPod":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "Drunk":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "WasOnFire":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "WalkedNaked":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "CollapseDodged":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "CaravanAmbushedByHumanlike":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "CaravanMeeting":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "CaravanDemand":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "CaravanFormed":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "CaravanFled":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "CaravanAmbushDefeated":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "CaravanAssaultSuccessful":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "Meditated":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "Prayed":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "LaunchedShip":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "AttendedConcert":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "HeldConcert":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "Exhausted":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "VSIE_IngestedHumanFlesh":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "VSIE_BingedFood":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "VSIE_BingedDrug":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "VSIE_HideInRoom":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "VSIE_ThrewTantrum":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "VSIE_WanderedInSaddness":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "VSIE_WentBerserk":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "VSIE_WentIntoSadisticRage":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "VSIE_WentOnFireStartingSpree":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "VSIE_WentOnMurderousRage":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "VSIE_RanWild":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "ButcheredHumanlikeCorpse":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "AteRawHumanlikeMeat":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "BuiltSnowman":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "EnteredCryptosleep":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "MentalStateBerserk":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "MentalStateGaveUp":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "VSIE_SlaughteredAnimalInRage":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "VSIE_TamedThrumbo":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "VSIE_WasPreviouslyOurEnemy":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "VSIE_WasBadlyInjured":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "VSIE_RemovedPrisonersOrgans":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "VSIE_FailedMedicalOperationAndKilled":
                    //Tale_SinglePawn
                    AddTale(tale,"");
                    break;

                case "CaravanAmbushedByManhunter":
                    //Tale_SinglePawn
                    AddTale(tale,"RT_CaravanAmbushedByManhunter".Translate(tale2.pawnData.name));
                    break;    


                case "CaravanRemoteMining":
                    //Tale_SinglePawnAndDef
                    AddTale(tale,"RT_CaravanRemoteMining".Translate(tale2.pawnData.name,tale2.defData.def.LabelCap));
                    break;


                case "PlayedGame":
                    //Tale_SinglePawnAndDef
                    AddTale(tale,"RT_PlayedGame".Translate(tale2.pawnData.name,tale2.defData.def.LabelCap));
                    break;

                case "HeatstrokeRevealed":
                    //Tale_SinglePawnAndDef
                    AddTale(tale,"RT_HeatstrokeRevealed".Translate(tale2.pawnData.name));
                    break;

                case "HypothermiaRevealed":
                    //Tale_SinglePawnAndDef
                    AddTale(tale,"RT_HypothermiaRevealed".Translate(tale2.pawnData.name));
                    break;

                case "ToxicityRevealed":
                    //Tale_SinglePawnAndDef
                    AddTale(tale,"RT_PlayedGame".Translate(tale2.pawnData.name,tale2.defData.def.LabelCap));
                    break;

                case "FinishedResearchProject":
                    //Tale_SinglePawnAndDef
                    AddTale(tale,"RT_FinishedResearchProject".Translate(tale2.pawnData.name,tale2.defData.def.LabelCap));
                    break;

                case "CompletedLongConstructionProject":
                    //Tale_SinglePawnAndDef
                    AddTale(tale,"RT_CompletedLongConstructionProject".Translate(tale2.pawnData.name,tale2.defData.def.LabelCap));
                    break;

                case "IllnessRevealed":
                    //Tale_SinglePawnAndDef
                    AddTale(tale,"RT_IllnessRevealed".Translate(tale2.pawnData.name,tale2.defData.def.LabelCap));
                    break;

                case "MinedValuable":
                    //Tale_SinglePawnAndDef
                    AddTale(tale,"RT_MinedValuable".Translate(tale2.pawnData.name,tale2.defData.def.LabelCap));
                    break;

                case "GainedMasterSkillWithPassion":
                    //Tale_SinglePawnAndDef
                    AddTale(tale,"RT_GainedMasterSkillWithPassion".Translate(tale2.pawnData.name,tale2.defData.def.LabelCap));
                    break;

                case "GainedMasterSkillWithoutPassion":
                    //Tale_SinglePawnAndDef
                    AddTale(tale,"RT_GainedMasterSkillWithoutPassion".Translate(tale2.pawnData.name,tale2.defData.def.LabelCap));
                    break;

                case "CompletedLongCraftingProject":
                    //Tale_SinglePawnAndDef
                    AddTale(tale,"RT_CompletedLongCraftingProject".Translate(tale2.pawnData.name,tale2.defData.def.LabelCap));
                    break;

                case "CraftedArt":
                    //* Tale_SinglePawnAndThing
                    AddTale(tale,"RT_CraftedArt".Translate(tale3.pawnData.name,tale3.thingData.thingDef.LabelCap));
                    break;

                case "StruckMineable":
                    //Tale_SinglePawnAndThing
                    AddTale(tale,"RT_StruckMineable".Translate(tale3.pawnData.name,tale3.thingData.thingDef.LabelCap));
                    break;

                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
    
    
    
                case "Recruited":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && tale4.firstPawnData != null){
                        AddTale(tale,"RT_Recruited".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                    }
                    break;

                case "DefeatedHostileFactionLeader":
                    // Double-Tale?!
                    if (!tale4.firstPawnData.kind.RaceProps.Humanlike ){
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_DefeatedHostileFactionLeader".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.kind.LabelCap));
                        }else{
                            AddTale(tale,"RT_DefeatedHostileFactionLeader".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.name));
                        }
                    }else{
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                                AddTale(tale,"RT_DefeatedHostileFactionLeader".Translate(tale4.firstPawnData.name,tale4.secondPawnData.kind.LabelCap));
                            }else{
                                AddTale(tale,"RT_DefeatedHostileFactionLeader".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                            }        
                    }
                    break;

                case "Stripped":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && tale4.firstPawnData != null){
                        AddTale(tale,"RT_Stripped".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                    }
                    break;

                case "IncreasedMenagerie":
                    //Tale_DoublePawn
                    if (tale4.secondPawnData != null && tale4.firstPawnData != null){
                        AddTale(tale,"RT_IncreasedMenagerie".Translate(tale4.firstPawnData.name,tale4.secondPawnData.kind.LabelCap));
                    }
                    break;

                case "AttendedParty":
                    // Tale_DoublePawn
                    if (tale4.firstPawnData != null && tale4.secondPawnData != null){
                        AddTale(tale,"RT_AttendedParty".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                    }
                    break;

                case "VSIE_DidNotAttendWedding":
                    //Tale_DoublePawn
                    if (tale4.firstPawnData != null && tale4.secondPawnData != null){
                        AddTale(tale,"RT_DidNotAttendWedding".Translate(tale4.secondPawnData.name,tale4.firstPawnData.name));
                    }
                    break;

                case "VSIE_AttendedMyWedding":
                    //Tale_DoublePawn
                    if (tale4.firstPawnData != null && tale4.secondPawnData != null){
                        AddTale(tale,"RT_AttendedMyWedding".Translate(tale4.secondPawnData.name,tale4.firstPawnData.name));
                    }
                    break;

                case "VSIE_TamedMe":
                    //Tale_DoublePawn
                    if (!tale4.firstPawnData.kind.RaceProps.Humanlike ){
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_TamedMe".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.kind.LabelCap));
                        }else{
                            AddTale(tale,"RT_TamedMe".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.name));
                        }
                    }else{
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                                AddTale(tale,"RT_TamedMe".Translate(tale4.firstPawnData.name,tale4.secondPawnData.kind.LabelCap));
                            }else{
                                AddTale(tale,"RT_TamedMe".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                            }        
                    }
                    break;

                case "VSIE_ArrestedMe":
                    //Tale_DoublePawn
                    if (tale4.firstPawnData != null && tale4.secondPawnData != null){
                        AddTale(tale,"RT_ArrestedMe".Translate(tale4.secondPawnData.name,tale4.firstPawnData.name));
                    }
                    break;

                case "VSIE_ResurrectedMe":
                    //Tale_DoublePawn
                    if (tale4.firstPawnData != null && tale4.secondPawnData != null){
                        AddTale(tale,"RT_ResurrectedMe".Translate(tale4.secondPawnData.name,tale4.firstPawnData.name));
                    }
                    break;

                case "VSIE_BrokeUpWithMe":
                    //Tale_DoublePawn
                    if (tale4.firstPawnData != null && tale4.secondPawnData != null){
                        AddTale(tale,"RT_BrokeUpWithMe".Translate(tale4.secondPawnData.name,tale4.firstPawnData.name));
                    }
                    break;

                case "VSIE_BondedPetButchered":
                    //Tale_DoublePawn
                    if (!tale4.firstPawnData.kind.RaceProps.Humanlike ){
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_BondedPetButchered".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.kind.LabelCap));
                        }else{
                            AddTale(tale,"RT_BondedPetButchered".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.name));
                        }
                    }else{
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                                AddTale(tale,"RT_BondedPetButchered".Translate(tale4.firstPawnData.name,tale4.secondPawnData.kind.LabelCap));
                            }else{
                                AddTale(tale,"RT_BondedPetButchered".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                            }        
                    }
                    break;

                case "VSIE_ExposedCorpseOfMyFriend":
                    // Tale_DoublePawn
                    if (!tale4.firstPawnData.kind.RaceProps.Humanlike ){
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_ExposedCorpseOfMyFriend".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.kind.LabelCap));
                        }else{
                            AddTale(tale,"RT_ExposedCorpseOfMyFriend".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.name));
                        }
                    }else{
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                                AddTale(tale,"RT_ExposedCorpseOfMyFriend".Translate(tale4.firstPawnData.name,tale4.secondPawnData.kind.LabelCap));
                            }else{
                                AddTale(tale,"RT_ExposedCorpseOfMyFriend".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                            }        
                    }
                    break;

                case "Hunted":
                    //Tale_DoublePawn
                    if (tale4.firstPawnData != null && tale4.secondPawnData != null){
                        AddTale(tale,"RT_Hunted".Translate(tale4.firstPawnData.name,tale4.secondPawnData.kind.LabelCap));
                    }
                    break;

                case "KidnappedColonist":
                    //Tale_DoublePawn -- can animals kidnap?
                    if (tale4.firstPawnData != null && tale4.secondPawnData != null){
                        AddTale(tale,"RT_KidnappedColonist".Translate(tale4.secondPawnData.name,tale4.firstPawnData.name));
                    }
                    break;

                case "DidSurgery":
                    //Tale_DoublePawn
                    if (!tale4.secondPawnData.kind.RaceProps.Humanlike){
                        AddTale(tale,"RT_DidSurgery".Translate(tale4.secondPawnData.kind.LabelCap,tale4.firstPawnData.name));
                    }else{
                        AddTale(tale,"RT_DidSurgery".Translate(tale4.secondPawnData.name,tale4.firstPawnData.name));
                    }
                    break;

                case "KilledColonist":
                    //Tale_DoublePawn
                    if (!tale4.firstPawnData.kind.RaceProps.Humanlike ){
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_KilledColonist".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.kind.LabelCap));
                        }else{
                            AddTale(tale,"RT_KilledColonist".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.name));
                        }
                    }else{
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                                AddTale(tale,"RT_KilledColonist".Translate(tale4.firstPawnData.name,tale4.secondPawnData.kind.LabelCap));
                            }else{
                                AddTale(tale,"RT_KilledColonist".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                            }        
                    }
                    break;

                case "TradedWith":
                    //Tale_DoublePawn
                    if (tale4.firstPawnData != null && tale4.secondPawnData != null){
                        AddTale(tale,"RT_TradedWith".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                    }
                    break;

                case "VSIE_WeHadNiceChat":
                    //Tale_DoublePawn
                    if (tale4.firstPawnData != null && tale4.secondPawnData != null){
                        AddTale(tale,"RT_WeHadNiceChat".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                    }
                    break;

                case "BecameLover":
                    //Tale_DoublePawn
                    if (tale4.firstPawnData != null && tale4.secondPawnData != null){
                        AddTale(tale,"RT_BecameLover".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                    }
                    break;

                case "Breakup": 
                    //Tale_DoublePawn
                    if (tale4.firstPawnData != null && tale4.secondPawnData != null){
                        AddTale(tale,"RT_Breakup".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                    }
                    break;

                case "Marriage":
                    //Tale_DoublePawn
                    if (tale4.firstPawnData != null && tale4.secondPawnData != null){
                        AddTale(tale,"RT_Marriage".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                    }
                    break;

                case "Captured":
                    //Tale_DoublePawn
                    if (!tale4.firstPawnData.kind.RaceProps.Humanlike ){
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_Captured".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.kind.LabelCap));
                        }else{
                            AddTale(tale,"RT_Captured".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.name));
                        }
                    }else{
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                                AddTale(tale,"RT_Captured".Translate(tale4.firstPawnData.name,tale4.secondPawnData.kind.LabelCap));
                            }else{
                                AddTale(tale,"RT_Captured".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                            }        
                    }
                    break;

                case "ExecutedPrisoner":
                    //Tale_DoublePawn
                    if (!tale4.firstPawnData.kind.RaceProps.Humanlike ){
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_ExecutedPrisoner".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.kind.LabelCap));
                        }else{
                            AddTale(tale,"RT_ExecutedPrisoner".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.name));
                        }
                    }else{
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                                AddTale(tale,"RT_ExecutedPrisoner".Translate(tale4.firstPawnData.name,tale4.secondPawnData.kind.LabelCap));
                            }else{
                                AddTale(tale,"RT_ExecutedPrisoner".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                            }        
                    }
                    break;

                case "SoldPrisoner":
                    //Tale_DoublePawn
                    if (!tale4.firstPawnData.kind.RaceProps.Humanlike ){
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_SoldPrisoner".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.kind.LabelCap));
                        }else{
                            AddTale(tale,"RT_SoldPrisoner".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.name));
                        }
                    }else{
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                                AddTale(tale,"RT_SoldPrisoner".Translate(tale4.firstPawnData.name,tale4.secondPawnData.kind.LabelCap));
                            }else{
                                AddTale(tale,"RT_SoldPrisoner".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                            }        
                    }
                    break;

                case "TamedAnimal":
                    //Tale_DoublePawn
                    if (!tale4.firstPawnData.kind.RaceProps.Humanlike ){
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_TamedAnimal".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.kind.LabelCap));
                        }else{
                            AddTale(tale,"RT_TamedAnimal".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.name));
                        }
                    }else{
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                                AddTale(tale,"RT_TamedAnimal".Translate(tale4.firstPawnData.name,tale4.secondPawnData.kind.LabelCap));
                            }else{
                                AddTale(tale,"RT_TamedAnimal".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                            }        
                    }
                    break;
                
                case "TrainedAnimal":
                    //Tale_DoublePawnAndDef
                    if (tale5.firstPawnData != null && tale5.secondPawnData != null){
                        AddTale(tale,"RT_TrainedAnimal".Translate(tale5.firstPawnData.name,tale5.secondPawnData.kind.LabelCap,tale5.defData.def.LabelCap));  
                    }
                    break;
                
                case "BondedWithAnimal":
                    //Tale_DoublePawn
                    if (!tale4.firstPawnData.kind.RaceProps.Humanlike ){
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_BondedWithAnimal".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.kind.LabelCap));
                        }else{
                            AddTale(tale,"RT_BondedWithAnimal".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.name));
                        }
                    }else{
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                                AddTale(tale,"RT_BondedWithAnimal".Translate(tale4.firstPawnData.name,tale4.secondPawnData.kind.LabelCap));
                            }else{
                                AddTale(tale,"RT_BondedWithAnimal".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                            }        
                    }
                    break;
                
                case "KilledColonyAnimal":
                    //Tale_DoublePawn
                    if (!tale4.firstPawnData.kind.RaceProps.Humanlike ){
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_KilledColonyAnimal".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.kind.LabelCap));
                        }else{
                            AddTale(tale,"RT_KilledColonyAnimal".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.name));
                        }
                    }else{
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                                AddTale(tale,"RT_KilledColonyAnimal".Translate(tale4.firstPawnData.name,tale4.secondPawnData.kind.LabelCap));
                            }else{
                                AddTale(tale,"RT_KilledColonyAnimal".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                            }        
                    }
                    break;
                
                case "BuriedCorpse":
                    //Tale_DoublePawn
                    if (!tale4.firstPawnData.kind.RaceProps.Humanlike ){
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_BuriedCorpse".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.kind.LabelCap));
                        }else{
                            AddTale(tale,"RT_BuriedCorpse".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.name));
                        }
                    }else{
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                                AddTale(tale,"RT_BuriedCorpse".Translate(tale4.firstPawnData.name,tale4.secondPawnData.kind.LabelCap));
                            }else{
                                AddTale(tale,"RT_BuriedCorpse".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                            }        
                    }
                    break;

                case "VisitedGrave":
                    //Tale_DoublePawn
                    // Do animals visit graves?! might be able to clean this.
                    if (!tale4.firstPawnData.kind.RaceProps.Humanlike ){
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_VisitedGrave".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.kind.LabelCap));
                        }else{
                            AddTale(tale,"RT_VisitedGrave".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.name));
                        }
                    }else{
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                                AddTale(tale,"RT_VisitedGrave".Translate(tale4.firstPawnData.name,tale4.secondPawnData.kind.LabelCap));
                            }else{
                                AddTale(tale,"RT_VisitedGrave".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                            }        
                    }
                    break;

                case "PutIntoCryptosleep":
                    //Tale_DoublePawn
                    if (!tale4.firstPawnData.kind.RaceProps.Humanlike ){
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_PutIntoCryptosleep".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.kind.LabelCap));
                        }else{
                            AddTale(tale,"RT_PutIntoCryptosleep".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.name));
                        }
                    }else{
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                                AddTale(tale,"RT_PutIntoCryptosleep".Translate(tale4.firstPawnData.name,tale4.secondPawnData.kind.LabelCap));
                            }else{
                                AddTale(tale,"RT_PutIntoCryptosleep".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                            }        
                    }
                    break;

                case "SocialFight":
                    //Tale_DoublePawn
                    if (tale4.firstPawnData != null &&  tale4.secondPawnData != null){
                        AddTale(tale,"RT_SocialFight".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                    }
                    break;

                case "KilledBy":
                    //Tale_DoublePawn - Should be Tale_DoublePawnKilledBy ?!
                    if (!tale4.firstPawnData.kind.RaceProps.Humanlike ){
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_KilledBy".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.kind.LabelCap));
                        }else{
                            AddTale(tale,"RT_KilledBy".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.name));
                        }
                    }else{
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                                AddTale(tale,"RT_KilledBy".Translate(tale4.firstPawnData.name,tale4.secondPawnData.kind.LabelCap));
                            }else{
                                AddTale(tale,"RT_KilledBy".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                            }        
                    }
                    break;

                case "GaveBirth":
                    //Tale_DoublePawn
                    if (!tale4.firstPawnData.kind.RaceProps.Humanlike ){
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_GaveBirth".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.kind.LabelCap));
                        }else{
                            AddTale(tale,"RT_GaveBirth".Translate(tale4.firstPawnData.kind.LabelCap,tale4.secondPawnData.name));
                        }
                    }else{
                        if (!tale4.secondPawnData.kind.RaceProps.Humanlike ){
                                AddTale(tale,"RT_GaveBirth".Translate(tale4.firstPawnData.name,tale4.secondPawnData.kind.LabelCap));
                            }else{
                                AddTale(tale,"RT_GaveBirth".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                            }        
                    }
                    break;
                
                case "VSIE_InsultedMe":
                    //Tale_DoublePawn
                    if (tale4.firstPawnData != null &&  tale4.secondPawnData != null){
                        AddTale(tale,"RT_InsultedMe".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                    }
                    break;

                case "VSIE_RebuffedMe":
                    //Tale_DoublePawn
                    if (tale4.firstPawnData != null &&  tale4.secondPawnData != null){
                        AddTale(tale,"RT_RebuffedMe".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                    }
                    break;

                case "VSIE_WeHadSocialFight":
                    //Tale_DoublePawn
                    if (tale4.firstPawnData != null &&  tale4.secondPawnData != null){
                        AddTale(tale,"RT_WeHadSocialFight".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                    }
                    break;

                case "VSIE_SavedMeFromMyWounds":
                    //Tale_DoublePawn
                    if (tale4.firstPawnData != null &&  tale4.secondPawnData != null){
                        AddTale(tale,"RT_SavedMeFromMyWounds".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                    }
                    break;

                case "VSIE_HasBeenMyFriendSinceChildhood":
                    //Tale_DoublePawn
                    if (tale4.firstPawnData != null &&  tale4.secondPawnData != null){
                        AddTale(tale,"RT_HasBeenMyFriendSinceChildhood".Translate(tale4.firstPawnData.name,tale4.secondPawnData.name));
                    }
                    break;

                case "KilledMajorThreat":
                    //Tale_DoublePawnAndDef
                    if (!tale5.firstPawnData.kind.RaceProps.Humanlike ){
                        if (!tale5.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_KilledMajorThreat".Translate(tale5.firstPawnData.kind.LabelCap,tale5.secondPawnData.kind.LabelCap,tale5.defData.def.LabelCap));
                        }else{
                            AddTale(tale,"RT_KilledMajorThreat".Translate(tale5.firstPawnData.kind.LabelCap,tale5.secondPawnData.name,tale5.defData.def.LabelCap));
                        }
                    }else{
                        if (!tale5.secondPawnData.kind.RaceProps.Humanlike ){
                                AddTale(tale,"RT_KilledMajorThreat".Translate(tale5.firstPawnData.name,tale5.secondPawnData.kind.LabelCap,tale5.defData.def.LabelCap));
                            }else{
                                AddTale(tale,"RT_KilledMajorThreat".Translate(tale5.firstPawnData.name,tale5.secondPawnData.name,tale5.defData.def.LabelCap));
                            }        
                    }
                    break;

                case "KilledMortar":
                    //Tale_DoublePawnAndDef
                    if (!tale5.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_KilledMortar".Translate(tale5.firstPawnData.name,tale5.secondPawnData.kind.LabelCap,tale5.defData.def.LabelCap));
                    }else{
                            AddTale(tale,"RT_KilledMortar".Translate(tale5.firstPawnData.name,tale5.secondPawnData.name,tale5.defData.def.LabelCap));
                    }
                    break;

                case "KilledLongRange":
                    //Tale_DoublePawnAndDef
                    if (!tale5.firstPawnData.kind.RaceProps.Humanlike ){
                        if (!tale5.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_KilledLongRange".Translate(tale5.firstPawnData.kind.LabelCap,tale5.secondPawnData.kind.LabelCap,tale5.defData.def.LabelCap));
                        }else{
                            AddTale(tale,"RT_KilledLongRange".Translate(tale5.firstPawnData.kind.LabelCap,tale5.secondPawnData.name,tale5.defData.def.LabelCap));
                        }
                    }else{
                        if (!tale5.secondPawnData.kind.RaceProps.Humanlike ){
                                AddTale(tale,"RT_KilledLongRange".Translate(tale5.firstPawnData.name,tale5.secondPawnData.kind.LabelCap,tale5.defData.def.LabelCap));
                            }else{
                                AddTale(tale,"RT_KilledLongRange".Translate(tale5.firstPawnData.name,tale5.secondPawnData.name,tale5.defData.def.LabelCap));
                            }        
                    }
                    break;
                
                case "KilledMelee":
                    //Tale_DoublePawnAndDef
                    if (!tale5.firstPawnData.kind.RaceProps.Humanlike ){
                        if (!tale5.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_KilledMelee".Translate(tale5.firstPawnData.kind.LabelCap,tale5.secondPawnData.kind.LabelCap,tale5.defData.def.LabelCap));
                        }else{
                            AddTale(tale,"RT_KilledMelee".Translate(tale5.firstPawnData.kind.LabelCap,tale5.secondPawnData.name,tale5.defData.def.LabelCap));
                        }
                    }else{
                        if (!tale5.secondPawnData.kind.RaceProps.Humanlike ){
                                AddTale(tale,"RT_KilledMelee".Translate(tale5.firstPawnData.name,tale5.secondPawnData.kind.LabelCap,tale5.defData.def.LabelCap));
                            }else{
                                AddTale(tale,"RT_KilledMelee".Translate(tale5.firstPawnData.name,tale5.secondPawnData.name,tale5.defData.def.LabelCap));
                            }        
                    }
                    break;

                case "KilledCapacity":
                    //Tale_DoublePawnAndDef
                    if (!tale5.firstPawnData.kind.RaceProps.Humanlike ){
                        if (!tale5.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_KilledCapacity".Translate(tale5.firstPawnData.kind.LabelCap,tale5.secondPawnData.kind.LabelCap,tale5.defData.def.LabelCap));
                        }else{
                            AddTale(tale,"RT_KilledCapacity".Translate(tale5.firstPawnData.kind.LabelCap,tale5.secondPawnData.name,tale5.defData.def.LabelCap));
                        }
                    }else{
                        if (!tale5.secondPawnData.kind.RaceProps.Humanlike ){
                                AddTale(tale,"RT_KilledCapacity".Translate(tale5.firstPawnData.name,tale5.secondPawnData.kind.LabelCap,tale5.defData.def.LabelCap));
                            }else{
                                AddTale(tale,"RT_KilledCapacity".Translate(tale5.firstPawnData.name,tale5.secondPawnData.name,tale5.defData.def.LabelCap));
                            }        
                    }
                    break;

                case "Wounded":
                    //Tale_DoublePawnAndDef
                    if (!tale5.firstPawnData.kind.RaceProps.Humanlike ){
                        if (!tale5.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_Wounded".Translate(tale5.firstPawnData.kind.LabelCap,tale5.secondPawnData.kind.LabelCap,tale5.defData.def.LabelCap));
                        }else{
                            AddTale(tale,"RT_Wounded".Translate(tale5.firstPawnData.kind.LabelCap,tale5.secondPawnData.name,tale5.defData.def.LabelCap));
                        }
                    }else{
                        if (!tale5.secondPawnData.kind.RaceProps.Humanlike ){
                                AddTale(tale,"RT_Wounded".Translate(tale5.firstPawnData.name,tale5.secondPawnData.kind.LabelCap,tale5.defData.def.LabelCap));
                            }else{
                                AddTale(tale,"RT_Wounded".Translate(tale5.firstPawnData.name,tale5.secondPawnData.name,tale5.defData.def.LabelCap));
                            }        
                    }
                    break;

                case "Downed":
                    //Tale_DoublePawnAndDef
                    if (!tale5.firstPawnData.kind.RaceProps.Humanlike ){
                        if (!tale5.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_Downed".Translate(tale5.firstPawnData.kind.LabelCap,tale5.secondPawnData.kind.LabelCap,tale5.defData.def.LabelCap));
                        }else{
                            AddTale(tale,"RT_Downed".Translate(tale5.firstPawnData.kind.LabelCap,tale5.secondPawnData.name,tale5.defData.def.LabelCap));
                        }
                    }else{
                        if (!tale5.secondPawnData.kind.RaceProps.Humanlike ){
                                AddTale(tale,"RT_Downed".Translate(tale5.firstPawnData.name,tale5.secondPawnData.kind.LabelCap,tale5.defData.def.LabelCap));
                            }else{
                                AddTale(tale,"RT_Downed".Translate(tale5.firstPawnData.name,tale5.secondPawnData.name,tale5.defData.def.LabelCap));
                            }        
                    }
                    break;

                case "VSIE_SavedMeFromRaiders":
                    //* Triple-pawn tale
                    if (tale6.firstPawnData != null &&  tale6.secondPawnData != null && tale6.thirdPawnData != null){
                        if (!tale5.secondPawnData.kind.RaceProps.Humanlike ){
                            AddTale(tale,"RT_SavedMeFromRaiders".Translate(tale6.firstPawnData.name,tale6.secondPawnData.kind.LabelCap,tale6.thirdPawnData.name));
                        }else{
                            AddTale(tale,"RT_SavedMeFromRaiders".Translate(tale6.firstPawnData.name,tale6.secondPawnData.name,tale6.thirdPawnData.name));
                        }
                    }
                    break;

                case "VSIE_StoleMyLover":
                    //* Triple-pawn tale
                    if (tale6.firstPawnData != null &&  tale6.secondPawnData != null && tale6.thirdPawnData != null){
                        AddTale(tale,"RT_StoleMyLover".Translate(tale6.firstPawnData.name,tale6.secondPawnData.name,tale6.thirdPawnData.name));
                    }
                    break;

                case "VSIE_CuredMyFriend":
                    //* Triple-pawn tale
                    if (tale6.firstPawnData != null &&  tale6.secondPawnData != null && tale6.thirdPawnData != null){
                        AddTale(tale,"RT_CuredMyFriend".Translate(tale6.firstPawnData.name,tale6.secondPawnData.name,tale6.thirdPawnData.name));
                    }
                    break;

                case "Eclipse":
                    AddTale(tale,"RT_Eclipse".Translate());
                    break;
    
                case "Aurora":
                    AddTale(tale,"RT_Aurora".Translate());
                    break;
    
                case "MeteoriteImpact":
                    AddTale(tale,"RT_MeteoriteImpact".Translate());
                    break;
    
                case "ShipPartCrash":
                    AddTale(tale,"RT_ShipPartCrash".Translate());
                    break;
    
                case "EndGame_ShipEscape":
                    AddTale(tale,"RT_EndGame_ShipEscape".Translate());
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