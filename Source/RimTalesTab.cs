using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using Verse;
using UnityEngine;

namespace RimTales {
    //* Our Tales List GUI 
    public class RimTalesTab : MainTabWindow
{
    public static bool bTabOpen = false; 
    private static List<String> tales = new List<String>();
    private Vector2 scrollPosition = Vector2.zero;
    private float scrollViewHeight;
    private String StrFilter = "";

    
    [DefOf]     //* Taken from Vanilla Interactions Expanded.
    public static class RimTales_DefOf{
        public static TaleDef VSIE_BondedPetButchered;
        public static TaleDef VSIE_ExposedCorpseOfMyFriend;
        public static TaleDef VSIE_IngestedHumanFlesh;
        public static TaleDef VSIE_BingedFood;
        public static TaleDef VSIE_BingedDrug;
        public static TaleDef VSIE_HideInRoom;
        public static TaleDef VSIE_ThrewTantrum;
        public static TaleDef VSIE_WanderedInSaddness;
        public static TaleDef VSIE_WentBerserk;
        public static TaleDef VSIE_WentIntoSadisticRage;
        public static TaleDef VSIE_WentOnFireStartingSpree;
        public static TaleDef VSIE_WentOnMurderousRage;
        public static TaleDef VSIE_RanWild;
        public static TaleDef VSIE_InsultedMe;
        public static TaleDef VSIE_SlaughteredAnimalInRage;
        public static TaleDef VSIE_RebuffedMe;
        public static TaleDef VSIE_InducedPrisonerToEscape;
        public static TaleDef VSIE_TamedThrumbo;
        public static TaleDef VSIE_WasPreviouslyOurEnemy;
        public static TaleDef VSIE_WasBadlyInjured;
        public static TaleDef VSIE_DidNotAttendWedding;
        public static TaleDef VSIE_AttendedMyWedding;
        public static TaleDef VSIE_RemovedPrisonersOrgans;
        public static TaleDef VSIE_FailedMedicalOperationAndKilled;
        public static TaleDef VSIE_TamedMe;
        public static TaleDef VSIE_ArrestedMe;
        public static TaleDef VSIE_ResurrectedMe;
        public static TaleDef VSIE_BrokeUpWithMe;
        public static TaleDef VSIE_WeHadNiceChat;
        public static TaleDef VSIE_WeHadSocialFight;
        public static TaleDef VSIE_SavedMeFromMyWounds;
        public static TaleDef VSIE_HasBeenMyFriendSinceChildhood;
        public static TaleDef VSIE_SavedMeFromRaiders;
        public static TaleDef VSIE_StoleMyLover;
        public static TaleDef VSIE_CuredMyFriend;
        //* Extra tales found so far.
        public static TaleDef PlayedGame;
        public static TaleDef MajorThreat;
        public static TaleDef HeatstrokeRevealed;
        public static TaleDef HypothermiaRevealed;
        public static TaleDef ToxicityRevealed;

        public static TaleDef VisitedGrave;
        public static TaleDef BuiltSnowman;
        public static TaleDef EnteredCryptosleep;
        public static TaleDef PutIntoCryptosleep;
        public static TaleDef MentalStateBerserk;
        public static TaleDef MentalStateGaveUp;
        public static TaleDef Raid;
        public static TaleDef Eclipse;
        public static TaleDef Aurora;
        public static TaleDef MeteoriteImpact;
        public static TaleDef ToxicFallout;
        public static TaleDef VolcanicWinter;
        public static TaleDef Flashstorm;
        public static TaleDef Infestation;
        public static TaleDef ManhunterPack;
        public static TaleDef ShipPartCrash;
        public static TaleDef EndGame_ShipEscape;
        public static TaleDef LaunchedShip;
        public static TaleDef Meditated;
        public static TaleDef Prayed;
        public static TaleDef Stripped;
        public static TaleDef Drunk;
        public static TaleDef CaravanAmbushedByManhunter;
        public static TaleDef CaravanDemand;
        public static TaleDef CaravanMeeting;
        
        //* our events from the hooks.

        public static TaleDef AnniversaryDeath;        
        public static TaleDef AnniversaryMarriage;
        public static TaleDef AnniversaryThreat;

        public static TaleDef Incident_AnimalInsanityMass;
        public static TaleDef Incident_ManhunterPack;
        public static TaleDef Incident_ColdSnap;
        public static TaleDef Incident_HeatWave;
        public static TaleDef Incident_FarmAnimalsWanderIn;
        public static TaleDef Incident_Infestation;
        public static TaleDef Incident_WandererJoin;
        public static TaleDef Incident_VolcanicWinter;
        public static TaleDef Incident_ToxicFallout;
    }

    private static String StrTalePlus;
    private static String StrTaleOverride;

    public override Vector2 RequestedTabSize{
        get
        {
            return new Vector2(1000f, 500f);
        }
    }

    //* Called when opening window, Set it all up.
    public override void PreOpen(){
        //* Set the list up
        Log.Message("[RimTales]: Refreshing Tale List...");
        tales.Clear();
        foreach (Tale tale in Resources.TaleManager)
        {
            if(RimTalesMod.settings.bIsDebugging == true){
                //Log.Message("[RimTales]: " + tale.ToString());
            }
            IncomingTale(tale);
        }
        tales.Reverse();
        base.PreOpen();
    }

    public override void PostOpen(){
        bTabOpen=true;
        Log.Message("[RimTales]: Showing Main Tab GUI.");
        StrFilter = "";
        base.PostOpen();
    }

    //* Called when closing window.
    public override void PostClose(){
        bTabOpen=false;
        Log.Message("[RimTales]: Closing Main Tab GUI.");
        base.PostClose();
    }

    //* Draw our GUI on the Window.
    public override void DoWindowContents(Rect fillRect){

        Text.Font = GameFont.Medium;
        Widgets.Label(fillRect, "RimTales");
        Rect position = new Rect(0f, 0f, fillRect.width, fillRect.height);
        GUI.BeginGroup(position);
        Text.Font = GameFont.Small;
        GUI.color = Color.white;
        Rect outRect = new Rect(0f, 50f, position.width, position.height - 50f);
        Rect position2 = new Rect(100f, 0f, 110F, 30F);
        Rect position3 = new Rect(280f, 5f, 110F, 30F);
        Widgets.Label(position3, "EnFilter".Translate());

        Rect position4 = new Rect(320f, 0f, 110F, 30F);
        StrFilter = Widgets.TextField(position4, StrFilter);

        Rect position6 = new Rect(615f, 0f, 300f, 30f);
        if(Widgets.ButtonText(position6, "EnSaveList".Translate())){
            saveTales();
        }

        Rect rect = new Rect(0f, 0f, position.width - 16f, this.scrollViewHeight);
        Widgets.BeginScrollView(outRect, ref this.scrollPosition, rect);

        float num = 0f;
        foreach (String tale in tales)
        {
            bool bShow = false;
            if(StrFilter == ""){
                bShow = true;
            }
            else if (tale.IndexOf(StrFilter, StringComparison.OrdinalIgnoreCase) >= 0){
                bShow = true;
            }
            if (bShow){

                GUI.color = Color.white;
                if (RimTalesMod.settings.bUseColour == true){
                    if (tale.Contains("Recruit") || tale.Contains("nimal") || tale.Contains("Hunted")) GUI.color = Color.green;
                    if (tale.Contains("Traded") || tale.Contains("Struck")) GUI.color = Color.gray;
                    if (tale.Contains("risoner") || tale.Contains("aked") || tale.Contains("Gave up") || tale.Contains("Wounded")) GUI.color = Color.yellow;
                    if (tale.Contains("Marriage") || tale.Contains("Breakup") || tale.Contains("lover")) GUI.color = Color.magenta;
                    if (tale.Contains("Death") || tale.Contains("Kidnap") || tale.Contains("Berserk") || tale.Contains("Kill") || tale.Contains("kill") || tale.Contains("Raid") || tale.Contains("Human") || tale.Contains("Downed")) GUI.color = Color.red;
                    if (tale.Contains("Research") || tale.Contains("Landed") || tale.Contains("Surgery")) GUI.color = Color.cyan;
                }
                
                Rect rect2 = new Rect(0f, num, rect.width, 30f);
                if (Mouse.IsOver(rect2)){
                    GUI.DrawTexture(rect2, TexUI.HighlightTex);
                }
                Widgets.Label(rect2, tale);
                num += 30f;
            }
        }

            
        GUI.color = Color.white;
        if (Event.current.type == EventType.Layout){
            this.scrollViewHeight = num;
        }
        Widgets.EndScrollView();
        GUI.EndGroup();

    }

    //* Hook detected new tales, refresh GUI
    public static void RefreshTales(){
        RimTalesTab.tales.Clear();
        foreach (Tale tale in Resources.TaleManager)
        {
            RimTalesTab.IncomingTale(tale);
        }
        RimTalesTab.tales.Reverse();
    }

    //* Export Recorded Tales to disk
    private void saveTales(){
        
        String outputFile;
        String outputMsg;
        outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "rimworld_tales.txt");
        using (var output = new StreamWriter(outputFile, false)){
            foreach (String tale in tales){
                output.WriteLine(tale);
            }
        }
        Log.Message("[RimTales]: Filtered Tales exported to " + outputFile);
        outputMsg = "Tales saved to " + outputFile + System.Environment.NewLine + System.Environment.NewLine;

        //* export the full data in another log.
        if (RimTalesMod.settings.bIsDebugging == true){
            outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "rimworld_tales_debug.txt");
            using (var output = new StreamWriter(outputFile, false)){
                foreach (Tale tale in Find.TaleManager.AllTalesListForReading){
                    output.WriteLine(tale.ToString());
                }
            }
            Log.Message("[RimTales]: Debugging Tales exported to " + outputFile);
            outputMsg = outputMsg + "Full-text Tales saved to " + outputFile +System.Environment.NewLine;

        }

        Dialog_MessageBox window = new Dialog_MessageBox(outputMsg, "OK!");
		Find.WindowStack.Add(window);

    }

    //* Add String Tales for GUI
    private static void AddTale(Tale tale, String overrideName, String plus){

            StringBuilder str = new StringBuilder();
            Vector2 longitude = Vector2.zero;
            
            if (tale.surroundings != null && tale.surroundings.tile >= 0){
                longitude = Find.WorldGrid.LongLatOf(tale.surroundings.tile);
            }
            
            str.Append(GenDate.DateFullStringAt(tale.date, longitude) + ": ");
            string taleStr = tale.ToString();
            string taleStr2 = taleStr.Split(new[] {"(age"}, StringSplitOptions.None)[0];
            string taleStr3 = taleStr2.Split(',')[0];
            str.Append(taleStr3.Remove(0, taleStr2.IndexOf(':') + 2));
            if (overrideName == ""){
                str.Append(plus);
                tales.Add(str.ToString());
            }
            else{
                string[] temp = str.ToString().Split(':');
                string final = temp[0] + ":" + temp[1];
                var outstr = final + overrideName + plus;
                tales.Add(outstr);
            }
    }

    //* Process Tales, Build String version
    private static void IncomingTale(Tale tale){

        StrTalePlus = "";
        StrTaleOverride = "";

        //* Tale_SinglePawn First
        //*****************************
        if (tale.def == TaleDefOf.Vomited){
            //Tale_SinglePawn
            if (RimTalesMod.settings.bShowVommit == true){
                AddTale(tale,StrTaleOverride,StrTalePlus);
            }
            return;
        }

        if (tale.def == TaleDefOf.LandedInPod){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.Drunk){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.WasOnFire){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.WalkedNaked){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.CollapseDodged){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.CaravanAmbushedByHumanlike){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.CaravanAmbushedByManhunter){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.CaravanMeeting){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.CaravanDemand){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.Meditated){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.Prayed){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.CaravanFormed){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.CaravanFled){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.CaravanAmbushDefeated){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.CaravanAssaultSuccessful){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.LaunchedShip){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.CaravanAmbushedByManhunter){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.CaravanDemand){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.CaravanMeeting){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.AttendedConcert){
            // Tale_SinglePawn ??
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.HeldConcert){
            // Tale_SinglePawn ??
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.Exhausted){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        } 

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_IngestedHumanFlesh){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_BingedFood){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_BingedDrug){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_HideInRoom){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_ThrewTantrum){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_WanderedInSaddness){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_WentBerserk){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_WentIntoSadisticRage){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_WentOnFireStartingSpree){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_WentOnMurderousRage){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_RanWild){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }       

        if (tale.def == TaleDefOf.ButcheredHumanlikeCorpse){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.AteRawHumanlikeMeat){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.BuiltSnowman){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.EnteredCryptosleep){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.MentalStateBerserk){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.MentalStateGaveUp){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_SlaughteredAnimalInRage){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_InducedPrisonerToEscape){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_TamedThrumbo){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_WasPreviouslyOurEnemy){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_WasBadlyInjured){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_RemovedPrisonersOrgans){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_FailedMedicalOperationAndKilled){
            //Tale_SinglePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.CaravanRemoteMining){
            Tale_SinglePawnAndDef tale2 = tale as Tale_SinglePawnAndDef;
            StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.PlayedGame){
            if (RimTalesMod.settings.bShowPlayedGame == true){
                Tale_SinglePawnAndDef tale2 = tale as Tale_SinglePawnAndDef;
                StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
                AddTale(tale,StrTaleOverride,StrTalePlus);
            }
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.HeatstrokeRevealed){
            Tale_SinglePawnAndDef tale2 = tale as Tale_SinglePawnAndDef;
            StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.HypothermiaRevealed){
            Tale_SinglePawnAndDef tale2 = tale as Tale_SinglePawnAndDef;
            StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.ToxicityRevealed){
            Tale_SinglePawnAndDef tale2 = tale as Tale_SinglePawnAndDef;
            StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.FinishedResearchProject){
            Tale_SinglePawnAndDef tale2 = tale as Tale_SinglePawnAndDef;
            StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.CompletedLongConstructionProject){
            Tale_SinglePawnAndDef tale2 = tale as Tale_SinglePawnAndDef;
            StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.IllnessRevealed){
            Tale_SinglePawnAndDef tale2 = tale as Tale_SinglePawnAndDef;
            StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.MinedValuable){
            Tale_SinglePawnAndDef tale2 = tale as Tale_SinglePawnAndDef;
            StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.GainedMasterSkillWithPassion){
            Tale_SinglePawnAndDef tale2 = tale as Tale_SinglePawnAndDef;
            StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.GainedMasterSkillWithoutPassion){
            Tale_SinglePawnAndDef tale2 = tale as Tale_SinglePawnAndDef;
            StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.DefeatedHostileFactionLeader){
            if (RimTalesMod.settings.bShowDeaths == true){
                Tale_SinglePawnAndDef tale2 = tale as Tale_SinglePawnAndDef;
                StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
                AddTale(tale,StrTaleOverride,StrTalePlus);
            }
            return;
        }

        if (tale.def == TaleDefOf.CraftedArt){
            //Tale_SinglePawnAndThing
            Tale_SinglePawnAndDef tale2 = tale as Tale_SinglePawnAndDef;
            StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.CompletedLongCraftingProject){
            Tale_SinglePawnAndDef tale2 = tale as Tale_SinglePawnAndDef;
            StrTalePlus = " - " + tale2.defData.def.LabelCap + ".";
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.StruckMineable){
            Tale_SinglePawnAndThing tale2 = tale as Tale_SinglePawnAndThing;
            StrTalePlus = " - " + tale2.thingData.thingDef.LabelCap + ".";
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        //* Double Pawn Tales
        //*****************************************
        if (tale.def == TaleDefOf.Recruited){
            Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
            if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                StrTalePlus = " - Joiner: " + tale2.secondPawnData.name + ".";
            }
            if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                StrTaleOverride = ": Recruiter: " + tale2.firstPawnData.name;
            }
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.Stripped){
            //Tale_DoublePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.IncreasedMenagerie){
            //Tale_DoublePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.AttendedParty){
            // Tale_DoublePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_DidNotAttendWedding){
            //Tale_DoublePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_AttendedMyWedding){
            //Tale_DoublePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_TamedMe){
            //Tale_DoublePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_ArrestedMe){
            //Tale_DoublePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_ResurrectedMe){
            //Tale_DoublePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_BrokeUpWithMe){
            //Tale_DoublePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_BondedPetButchered){
            //Tale_DoublePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_ExposedCorpseOfMyFriend){
            // Tale_DoublePawn
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.Hunted){
            Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
            if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                StrTaleOverride = ": " + tale2.secondPawnData.name;
            }
            if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                StrTaleOverride = ": " + tale2.secondPawnData.kind.LabelCap;
            }
            if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                StrTalePlus = " - Hunter: " + tale2.firstPawnData.name;
            }
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.KidnappedColonist){
            Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
            if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                StrTaleOverride = ": Victim: " + tale2.secondPawnData.name;
            }
            if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                StrTalePlus = " - Kidnapper: " + tale2.firstPawnData.name + ".";
            }
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.DidSurgery){
            Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
            if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                StrTalePlus = " - Patient: " + tale2.secondPawnData.name + ".";
            }
            if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                StrTalePlus = " - Patient: " + tale2.secondPawnData.kind.LabelCap + ".";
            }
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.KilledColonist){
            Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
            if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                StrTaleOverride = ": " + tale2.secondPawnData.name;
            }
            if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                StrTalePlus = " - Killer: " + tale2.firstPawnData.kind.LabelCap + ".";
            }
            if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                StrTalePlus = " - Killer: " + tale2.firstPawnData.name + ".";
            }
            if (RimTalesMod.settings.bShowDeaths == true){
                AddTale(tale,StrTaleOverride,StrTalePlus);
            }
            return;
        }

        if (tale.def == TaleDefOf.KilledMajorThreat){
            //Tale_DoublePawnAndDef
            if (RimTalesMod.settings.bShowDeaths == true){
                Tale_DoublePawnAndDef tale2 = tale as Tale_DoublePawnAndDef;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    StrTalePlus = " - Enemy: " + tale2.secondPawnData.name + " Weapon: " + tale2.defData.def.LabelCap + ".";
                }
                AddTale(tale,StrTaleOverride,StrTalePlus);
            }
            return;
        }

        if (tale.def == TaleDefOf.TradedWith){
            Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
            if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                StrTalePlus = " - Trader: " + tale2.secondPawnData.name + ".";
            }
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.BecameLover || tale.def == TaleDefOf.Breakup || tale.def == TaleDefOf.Marriage){
            Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
            if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                StrTalePlus = " and " + tale2.secondPawnData.name + ".";
            }
            if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                StrTaleOverride = ": " + tale2.firstPawnData.name + "";
            }
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }                

        if (tale.def == TaleDefOf.Captured || tale.def == TaleDefOf.ExecutedPrisoner || tale.def == TaleDefOf.SoldPrisoner){
            Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
            if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                StrTalePlus = " - Animal: " + tale2.secondPawnData.kind.LabelCap + ".";
            }
            if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                StrTalePlus = " - Prisoner: " + tale2.secondPawnData.name + ".";
            }
            if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                StrTaleOverride = ": Warden: " + tale2.firstPawnData.name;
            }
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.TamedAnimal || tale.def == TaleDefOf.TrainedAnimal || tale.def == TaleDefOf.BondedWithAnimal){
            Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
            if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                StrTaleOverride = ": " + tale2.secondPawnData.kind.LabelCap;
            }
            if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                StrTaleOverride = ": " + tale2.secondPawnData.name;
            }
            if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                StrTalePlus = " - Handler: " + tale2.firstPawnData.name + ".";
            }
            if (RimTalesMod.settings.bShowAnimalTales == true){
                AddTale(tale,StrTaleOverride,StrTalePlus);
            }
            return;
        }

        if (tale.def == TaleDefOf.KilledColonyAnimal){
            Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
            if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                StrTalePlus = " - Animal: " + tale2.secondPawnData.kind.LabelCap + ".";
            }
            if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                StrTalePlus = " - Animal: " + tale2.secondPawnData.name + ".";
            }
            if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                StrTaleOverride = ": Killer: " + tale2.firstPawnData.kind.LabelCap;
            }
            if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                StrTaleOverride = ": Killer: " + tale2.firstPawnData.name;
            }
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.BuriedCorpse){
            Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
            if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                StrTaleOverride = ": " + tale2.secondPawnData.kind.LabelCap;
            }
            if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                StrTaleOverride = ": " + tale2.secondPawnData.name;
            }
            if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                StrTalePlus = " - Worker: " + tale2.firstPawnData.kind.LabelCap + ".";
            }
            if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                StrTalePlus = " - Worker: " + tale2.firstPawnData.name + ".";
            }
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VisitedGrave){
            Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
            if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                StrTalePlus = " went to visit the grave of  " + tale2.secondPawnData.kind.LabelCap  + ".";
            }
            if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                StrTalePlus = " went to visit the grave of " + tale2.secondPawnData.name  + ".";
            }
            if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                StrTaleOverride = ": " + tale2.firstPawnData.kind.LabelCap;
            }
            if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                StrTaleOverride = ": " + tale2.firstPawnData.name;
            }
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.PutIntoCryptosleep){
            Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
            if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                StrTalePlus = " helped - " + tale2.secondPawnData.kind.LabelCap  + ".";
            }
            if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                StrTalePlus = " helped - " + tale2.secondPawnData.name  + ".";
            }
            if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                StrTaleOverride = ": " + tale2.firstPawnData.kind.LabelCap;
            }
            if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                StrTaleOverride = ": " + tale2.firstPawnData.name;
            }
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.SocialFight){
            Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
            if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                StrTalePlus = " - Attacker: " + tale2.secondPawnData.name + ".";
            }
            if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                StrTalePlus = " - Attacker: " + tale2.secondPawnData.kind.LabelCap + ".";
            }
            if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                StrTaleOverride = ": " + tale2.firstPawnData.name;
            }
            if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                StrTaleOverride = ": " + tale2.firstPawnData.kind.LabelCap;
            }
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.KilledBy){
            if (RimTalesMod.settings.bShowDeaths == true){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    StrTalePlus = " - Attacker: " + tale2.secondPawnData.name + ".";
                }
                if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    StrTalePlus = " - Attacker: " + tale2.secondPawnData.kind.LabelCap + ".";
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    StrTaleOverride = ": " + tale2.firstPawnData.name;
                }
                if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                    StrTaleOverride = ": " + tale2.firstPawnData.kind.LabelCap;
                }
                AddTale(tale,StrTaleOverride,StrTalePlus);
            }   
            return;
        }
        if (tale.def == TaleDefOf.KilledMortar){
            //Tale_DoublePawnAndDef
            if (RimTalesMod.settings.bShowDeaths == true){
                Tale_DoublePawnAndDef tale2 = tale as Tale_DoublePawnAndDef;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    StrTalePlus = " killed - " + tale2.secondPawnData.name + ".";
                }
                if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    StrTalePlus = " killed - " + tale2.secondPawnData.kind.LabelCap + ".";
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    StrTaleOverride = ": " + tale2.firstPawnData.name;
                }
                AddTale(tale,StrTaleOverride,StrTalePlus);
            }   
            return;
        }

        if (tale.def == TaleDefOf.KilledLongRange){
            //Tale_DoublePawnAndDef
            if (RimTalesMod.settings.bShowDeaths == true){
                Tale_DoublePawnAndDef tale2 = tale as Tale_DoublePawnAndDef;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    StrTalePlus = " killed " + tale2.secondPawnData.name + " with " + tale2.defData.def.LabelCap + ".";
                }
                if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    StrTalePlus = " killed " + tale2.secondPawnData.kind.LabelCap + " with " + tale2.defData.def.LabelCap + ".";
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    StrTaleOverride = ": " + tale2.firstPawnData.name;
                }
                if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                    StrTaleOverride = ": " + tale2.firstPawnData.kind.LabelCap;
                }
                AddTale(tale,StrTaleOverride,StrTalePlus);
            }
            return;
        }

        if (tale.def == TaleDefOf.KilledMelee){
            if (RimTalesMod.settings.bShowDeaths == true){
                //Tale_DoublePawnAndDef
                Tale_DoublePawnAndDef tale2 = tale as Tale_DoublePawnAndDef;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    StrTalePlus = " killed " + tale2.secondPawnData.name + " with " + tale2.defData.def.LabelCap + ".";
                }
                if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    StrTalePlus = " killed " + tale2.secondPawnData.kind.LabelCap + " with " + tale2.defData.def.LabelCap + ".";
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    StrTaleOverride = ": " + tale2.firstPawnData.name;
                }
                if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                    StrTaleOverride = ": " + tale2.firstPawnData.kind.LabelCap;
                }
                AddTale(tale,StrTaleOverride,StrTalePlus);
            }
            return;
        }

        if (tale.def == TaleDefOf.KilledCapacity){
            //Tale_DoublePawnAndDef
            if (RimTalesMod.settings.bShowDeaths == true){
                Tale_DoublePawnAndDef tale2 = tale as Tale_DoublePawnAndDef;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    StrTalePlus = " killed " + tale2.secondPawnData.name + " with " + tale2.defData.def.LabelCap + ".";
                }
                if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    StrTalePlus = " killed " + tale2.secondPawnData.kind.LabelCap + " with " + tale2.defData.def.LabelCap + ".";
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    StrTaleOverride = ": " + tale2.firstPawnData.name;
                }
                if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                    StrTaleOverride = ": " + tale2.firstPawnData.kind.LabelCap;
                }
                AddTale(tale,StrTaleOverride,StrTalePlus);
            }
            return;
        }

        if (tale.def == TaleDefOf.GaveBirth){
            Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
            if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                StrTaleOverride = ": " + tale2.secondPawnData.kind.LabelCap;
            }
            if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                StrTaleOverride = ": " + tale2.secondPawnData.name;
            }
            if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                StrTalePlus = " - Mother: " + tale2.firstPawnData.kind.LabelCap  + ".";
            }
            if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                StrTalePlus = " - Mother: " + tale2.firstPawnData.name + ".";
            }
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == TaleDefOf.Wounded){
            //Tale_DoublePawnAndDef
            if (RimTalesMod.settings.bShowWounded == true){
                Tale_DoublePawnAndDef tale2 = tale as Tale_DoublePawnAndDef;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    StrTalePlus = " - Attacker: " + tale2.secondPawnData.name + " with " + tale2.defData.def.LabelCap + ".";
                }
                else if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    StrTalePlus = " - Attacker: " + tale2.secondPawnData.kind.LabelCap + " with " + tale2.defData.def.LabelCap + ".";
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    StrTaleOverride = ": " + tale2.firstPawnData.name;
                }
                else if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                    StrTaleOverride = ": " + tale2.firstPawnData.kind.LabelCap;
                }
                AddTale(tale,StrTaleOverride,StrTalePlus);
            }
            return;
        }

        if (tale.def == TaleDefOf.Downed){
            //Tale_DoublePawnAndDef
            Tale_DoublePawnAndDef tale2 = tale as Tale_DoublePawnAndDef;
            if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                StrTalePlus = " - Attacker: " + tale2.secondPawnData.name + tale2.defData.def.LabelCap + ".";
            }
            if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                StrTalePlus = " - Attacker: " + tale2.secondPawnData.kind.LabelCap + tale2.defData.def.LabelCap + ".";
            }
            if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                StrTaleOverride = ": " + tale2.firstPawnData.name;
            }
            if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                StrTaleOverride = ": " + tale2.firstPawnData.kind.LabelCap;
            }
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_InsultedMe){
            Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
            if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                StrTalePlus = " - Attacker: " + tale2.secondPawnData.name + ".";
            }
            if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                StrTaleOverride = ": " + tale2.firstPawnData.name;
            }
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_RebuffedMe){
            Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
            if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                StrTalePlus = " - Attacker: " + tale2.secondPawnData.name + ".";
            }
            if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                StrTaleOverride = ": " + tale2.firstPawnData.name;
            }
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }
        
        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_WeHadNiceChat){
            if (RimTalesMod.settings.bShowChitChat == true){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    StrTalePlus = " & " + tale2.secondPawnData.name + ".";
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    StrTaleOverride = ": " + tale2.firstPawnData.name;
                }
                AddTale(tale,StrTaleOverride,StrTalePlus);
                }
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_WeHadSocialFight){
            Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
            if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                StrTalePlus = " - Attacker: " + tale2.secondPawnData.name + ".";
            }
            if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                StrTaleOverride = ": " + tale2.firstPawnData.name;
            }
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_SavedMeFromMyWounds){
            Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
            if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                StrTalePlus = " - Friend: " + tale2.secondPawnData.name + ".";
            }
            if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                StrTaleOverride = ": " + tale2.firstPawnData.name;
            }
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_HasBeenMyFriendSinceChildhood){
            Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
            if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                StrTalePlus = " - Friend: " + tale2.secondPawnData.name + ".";
            }
            if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                StrTaleOverride = ": " + tale2.firstPawnData.name;
            }
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_SavedMeFromRaiders){
            //* Triple-pawn  tale
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_StoleMyLover){
            //* Triple-pawn  tale
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VSIE_CuredMyFriend){
            //* Triple-pawn  tale
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        //*************** Events
        if (tale.def == RimTalesTab.RimTales_DefOf.MajorThreat){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.Raid){
            //* Duplicate Tale, dont add.
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.Eclipse){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.Aurora){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.MeteoriteImpact){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.ToxicFallout){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.VolcanicWinter){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.Flashstorm){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.Infestation){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.ManhunterPack){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.ShipPartCrash){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.EndGame_ShipEscape){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        //**************** Our Tales ***************************
        if (tale.def == RimTalesTab.RimTales_DefOf.AnniversaryDeath){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.AnniversaryMarriage){
            Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
            if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                StrTalePlus = " and " + tale2.secondPawnData.name + ".";
            }
            if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                StrTaleOverride = ": " + tale2.firstPawnData.name + "";
            }
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.AnniversaryThreat){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.Incident_AnimalInsanityMass){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.Incident_ManhunterPack){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.Incident_ColdSnap){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.Incident_HeatWave){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.Incident_FarmAnimalsWanderIn){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.Incident_Infestation){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.Incident_WandererJoin){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.Incident_VolcanicWinter){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        if (tale.def == RimTalesTab.RimTales_DefOf.Incident_ToxicFallout){
            AddTale(tale,StrTaleOverride,StrTalePlus);
            return;
        }

        //* We Shouldn't end up here, log the tale if debug-mode
        if (RimTalesMod.settings.bIsDebugging == true){
            Log.Message("[RimTales]:============UNKNOWN-TALE===========");
            Log.Message("[RimTales]:(TOSTRING) " + tale.ToString());
            Log.Message("[RimTales]:(DEF) " +  tale.def);
            Log.Message("[RimTales]:===================================");
        }
    }

}

}
