using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using Verse;
using UnityEngine;

namespace RimTales 
{
    public class RimTalesTab : MainTabWindow
{
    private Vector2 scrollPosition = Vector2.zero;
    private float scrollViewHeight;

    private String filter = "";
    private int TickCount = 0;
    private float  NumTales = 0;
    private float ProcessedTales = 0; 
    private bool bCheckRefresh = true;
    private String StrPlus = "";
    private String StrOverride = "";

    private List<String> tales = new List<String>();

    //* Taken from Vanilla Interactions Expanded so we can export even more Tales.
    [DefOf]
    public static class VIE_DefOf
    {
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
        public static TaleDef Raid;
        public static TaleDef Eclipse;
        public static TaleDef Aurora;
        public static TaleDef Meditated;
        public static TaleDef Prayed;
        public static TaleDef Stripped;

    }

    public override Vector2 RequestedTabSize{
        get
        {
            return new Vector2(1000f, 500f);
        }
    }

    //* Called when opening window, Set it all up.
    public override void PostOpen(){
        base.PostOpen();
        ProcessTales();
        filter = "";
    }

    //* Called when the Window is visible.
	public override void WindowUpdate(){
        base.WindowUpdate();
        if(bCheckRefresh == true){
            TickCount = TickCount + 1;
            if (TickCount >= 100){
                TickCount = 0;
                Log.Message("RimTales: WindowUpdate() TickCount=100");
                if(NumTales < Find.TaleManager.AllTalesListForReading.Count){
                    bCheckRefresh = false;
                    Log.Message("RimTales: New Tales Detected! Refreshing list.");
                    ProcessTales();
                }
            }
        }
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

        if(Widgets.ButtonText(position2, "EnInvert".Translate())){
            this.tales.Reverse();
        }
            
        Rect position3 = new Rect(280f, 5f, 110F, 30F);
        Widgets.Label(position3, "EnFilter".Translate());

        Rect position4 = new Rect(320f, 0f, 110F, 30F);
        filter = Widgets.TextField(position4, filter);

        Rect position6 = new Rect(615f, 0f, 300f, 30f);
        if(Widgets.ButtonText(position6, "EnSaveList".Translate())){
            saveTales();
        }

        Rect rect = new Rect(0f, 0f, position.width - 16f, this.scrollViewHeight);
        Widgets.BeginScrollView(outRect, ref this.scrollPosition, rect);

        float num = 0f;
        foreach (String tale in this.tales)
        {
            bool show = false;
            if(filter == ""){
                show = true;
            }
            else if (tale.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0){
                show = true;
            }
            if (show){

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

    public void ProcessTales(){

        //* Stop refreshing while we process the tales.
        bCheckRefresh = false; 

        this.tales.Clear();
        Log.Message("RimTales: Retrieving Tale Log...");
        
        ProcessedTales = 0;
        NumTales = Find.TaleManager.AllTalesListForReading.Count;
        foreach (Tale tale in Find.TaleManager.AllTalesListForReading){

            StrPlus = "";
            StrOverride = "";

            if (tale.def == TaleDefOf.FinishedResearchProject){
                Tale_SinglePawnAndDef tale2 = tale as Tale_SinglePawnAndDef;
                StrPlus = " - " + tale2.defData.def.LabelCap + ".";
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.Vomited){
                if (RimTalesMod.settings.bShowVommit == true){
                    AddTale(tale,StrOverride,StrPlus);
                }else{
                    ProcessedTales =  ProcessedTales + 1;
                }
                continue;
            }

            if (tale.def == TaleDefOf.Recruited){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    StrPlus = " - Joiner: " + tale2.secondPawnData.name + ".";
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    StrOverride = ": Recruiter: " + tale2.firstPawnData.name;
                }
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.Hunted){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    StrOverride = ": " + tale2.secondPawnData.name;
                }
                if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    StrOverride = ": " + tale2.secondPawnData.kind.LabelCap;
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    StrPlus = " - Hunter: " + tale2.firstPawnData.name;
                }
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.KidnappedColonist){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    StrOverride = ": Victim: " + tale2.secondPawnData.name;
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    StrPlus = " - Kidnapper: " + tale2.firstPawnData.name + ".";
                }
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.DidSurgery){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    StrPlus = " - Patient: " + tale2.secondPawnData.name + ".";
                }
                if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    StrPlus = " - Patient: " + tale2.secondPawnData.kind.LabelCap + ".";
                }
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.KilledColonist){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    StrOverride = ": " + tale2.secondPawnData.name;
                }
                if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                    StrPlus = " - Killer: " + tale2.firstPawnData.kind.LabelCap + ".";
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    StrPlus = " - Killer: " + tale2.firstPawnData.name + ".";
                }
                if (RimTalesMod.settings.bShowDeaths == true){
                    AddTale(tale,StrOverride,StrPlus);
                }else{
                    ProcessedTales =  ProcessedTales + 1;
                }
                continue;
            }

            if (tale.def == TaleDefOf.KilledMajorThreat){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    StrPlus = " - Enemy: " + tale2.secondPawnData.name + ".";
                }
                if (RimTalesMod.settings.bShowDeaths == true){
                    AddTale(tale,StrOverride,StrPlus);
                }else{
                    ProcessedTales =  ProcessedTales + 1;
                }
                continue;
            }

            if (tale.def == TaleDefOf.TradedWith){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    StrPlus = " - Trader: " + tale2.secondPawnData.name + ".";
                }
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.BecameLover || tale.def == TaleDefOf.Breakup || tale.def == TaleDefOf.Marriage){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    StrPlus = " and " + tale2.secondPawnData.name + ".";
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    StrOverride = ": " + tale2.firstPawnData.name + "";
                }
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }                
  
            if (tale.def == TaleDefOf.Captured || tale.def == TaleDefOf.ExecutedPrisoner || tale.def == TaleDefOf.SoldPrisoner){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    StrPlus = " - Animal: " + tale2.secondPawnData.kind.LabelCap + ".";
                }
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    StrPlus = " - Prisoner: " + tale2.secondPawnData.name + ".";
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    StrOverride = ": Warden: " + tale2.firstPawnData.name;
                }
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.TamedAnimal || tale.def == TaleDefOf.TrainedAnimal || tale.def == TaleDefOf.BondedWithAnimal){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    StrOverride = ": " + tale2.secondPawnData.kind.LabelCap;
                }
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    StrOverride = ": " + tale2.secondPawnData.name;
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    StrPlus = " - Handler: " + tale2.firstPawnData.name + ".";
                }
                if (RimTalesMod.settings.bShowAnimalTales == true){
                    AddTale(tale,StrOverride,StrPlus);
                }else{
                    ProcessedTales =  ProcessedTales + 1;
                }
                continue;
            }

            if (tale.def == TaleDefOf.KilledColonyAnimal){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    StrPlus = " - Animal: " + tale2.secondPawnData.kind.LabelCap + ".";
                }
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    StrPlus = " - Animal: " + tale2.secondPawnData.name + ".";
                }
                if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                    StrOverride = ": Killer: " + tale2.firstPawnData.kind.LabelCap;
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    StrOverride = ": Killer: " + tale2.firstPawnData.name;
                }
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.LandedInPod){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.BuriedCorpse){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    StrOverride = ": " + tale2.secondPawnData.kind.LabelCap;
                }
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    StrOverride = ": " + tale2.secondPawnData.name;
                }
                if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                    StrPlus = " - Worker: " + tale2.firstPawnData.kind.LabelCap + ".";
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    StrPlus = " - Worker: " + tale2.firstPawnData.name + ".";
                }
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.WasOnFire){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.CompletedLongConstructionProject){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.CaravanAmbushedByHumanlike){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.IllnessRevealed){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.IncreasedMenagerie){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.MinedValuable){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.CaravanRemoteMining){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.AttendedConcert){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.HeldConcert){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.AttendedParty){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.StruckMineable){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.Exhausted){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.WalkedNaked){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.SocialFight){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.GainedMasterSkillWithPassion){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.KilledBy){
                if (RimTalesMod.settings.bShowDeaths == true){
                    AddTale(tale,StrOverride,StrPlus);
                }else{
                    ProcessedTales =  ProcessedTales + 1;
                }
                continue;
            }

            if (tale.def == TaleDefOf.KilledMortar){
                if (RimTalesMod.settings.bShowDeaths == true){
                    AddTale(tale,StrOverride,StrPlus);
                }else{
                    ProcessedTales =  ProcessedTales + 1;
                }
                continue;
            }

            if (tale.def == TaleDefOf.KilledLongRange){
                if (RimTalesMod.settings.bShowDeaths == true){
                    AddTale(tale,StrOverride,StrPlus);
                }else{
                    ProcessedTales =  ProcessedTales + 1;
                }
                continue;
            }

            if (tale.def == TaleDefOf.KilledMelee){
                if (RimTalesMod.settings.bShowDeaths == true){
                    AddTale(tale,StrOverride,StrPlus);
                }else{
                    ProcessedTales =  ProcessedTales + 1;
                }
                continue;
            }

            if (tale.def == TaleDefOf.DefeatedHostileFactionLeader){
                if (RimTalesMod.settings.bShowDeaths == true){
                    AddTale(tale,StrOverride,StrPlus);
                }else{
                    ProcessedTales =  ProcessedTales + 1;
                }
                continue;
            }

            if (tale.def == TaleDefOf.KilledCapacity){
                if (RimTalesMod.settings.bShowDeaths == true){
                    AddTale(tale,StrOverride,StrPlus);
                }else{
                    ProcessedTales =  ProcessedTales + 1;
                }
                continue;
            }

            if (tale.def == TaleDefOf.CaravanFormed){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.CaravanFled){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.CaravanAmbushDefeated){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.CaravanAssaultSuccessful){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.ButcheredHumanlikeCorpse){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.CraftedArt){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.GaveBirth){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    StrOverride = ": " + tale2.secondPawnData.kind.LabelCap;
                }
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    StrOverride = ": " + tale2.secondPawnData.name;
                }
                if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                    StrPlus = " - Mother: " + tale2.firstPawnData.kind.LabelCap  + ".";
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    StrPlus = " - Mother: " + tale2.firstPawnData.name + ".";
                }
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.AteRawHumanlikeMeat){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.CompletedLongCraftingProject){
                Tale_SinglePawnAndDef tale2 = tale as Tale_SinglePawnAndDef;
                StrPlus = " - " + tale2.defData.def.LabelCap + ".";
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == TaleDefOf.Wounded){
                if (RimTalesMod.settings.bShowWounded == true){
                    Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                    if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                        StrPlus = " - Attacker: " + tale2.secondPawnData.name + ".";
                    }
                    if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                        StrPlus = " - Attacker: " + tale2.secondPawnData.kind.LabelCap + ".";
                    }
                    if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                        StrOverride = ": " + tale2.firstPawnData.name;
                    }
                    if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                        StrOverride = ": " + tale2.firstPawnData.kind.LabelCap;
                    }
                    AddTale(tale,StrOverride,StrPlus);
                }else{
                    ProcessedTales =  ProcessedTales + 1;
                }
                continue;
            }

            if (tale.def == TaleDefOf.Downed){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    StrPlus = " - Attacker: " + tale2.secondPawnData.name + ".";
                }
                if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    StrPlus = " - Attacker: " + tale2.secondPawnData.kind.LabelCap + ".";
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    StrOverride = ": " + tale2.firstPawnData.name;
                }
                if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                    StrOverride = ": " + tale2.firstPawnData.kind.LabelCap;
                }
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            //* Not sure what this means?!
            if (tale.def == TaleDefOf.CollapseDodged){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            //*  VanillaSocialInteractionsExpanded stuff
            if (tale.def == VIE_DefOf.VSIE_BondedPetButchered){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_ExposedCorpseOfMyFriend){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_IngestedHumanFlesh){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_BingedFood){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_BingedDrug){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_HideInRoom){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_ThrewTantrum){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_WanderedInSaddness){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_WentBerserk){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_WentIntoSadisticRage){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_WentOnFireStartingSpree){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_WentOnMurderousRage){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_RanWild){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_InsultedMe){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_SlaughteredAnimalInRage){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_RebuffedMe){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_InducedPrisonerToEscape){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_TamedThrumbo){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_WasPreviouslyOurEnemy){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_WasBadlyInjured){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_DidNotAttendWedding){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_AttendedMyWedding){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_RemovedPrisonersOrgans){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_FailedMedicalOperationAndKilled){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_TamedMe){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_ArrestedMe){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }


            if (tale.def == VIE_DefOf.VSIE_ResurrectedMe){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_BrokeUpWithMe){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }
            
            if (tale.def == VIE_DefOf.VSIE_WeHadNiceChat){
                if (RimTalesMod.settings.bShowChitChat == true){
                    AddTale(tale,StrOverride,StrPlus);
                }else{
                    ProcessedTales =  ProcessedTales + 1;
                }
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_WeHadSocialFight){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_SavedMeFromMyWounds){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_HasBeenMyFriendSinceChildhood){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_SavedMeFromRaiders){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_StoleMyLover){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_CuredMyFriend){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            //* Spammy, not sure where these come from yet.
            if (tale.def == VIE_DefOf.PlayedGame){
                if (RimTalesMod.settings.bShowPlayedGame == true){
                    AddTale(tale,StrOverride,StrPlus);
                }else{
                    ProcessedTales =  ProcessedTales + 1;
                }
                continue;
            }

            if (tale.def == VIE_DefOf.MajorThreat){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.HeatstrokeRevealed){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.Raid){
                //* Duplicate Tale, count it but dont add.
                ProcessedTales = ProcessedTales + 1;
                continue;
            }

            if (tale.def == VIE_DefOf.Eclipse){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }
            if (tale.def == VIE_DefOf.Aurora){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }
            if (tale.def == VIE_DefOf.Meditated){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.Prayed){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            if (tale.def == VIE_DefOf.Stripped){
                AddTale(tale,StrOverride,StrPlus);
                continue;
            }

            //* We Shouldn't end up here, log the tale if debug-mode
            if (RimTalesMod.settings.bIsDebugging == true){
                Log.Message("RimTales-DEBUG:============UNKNOWN-TALE===========");
                Log.Message("RimTales-DEBUG:(TOSTRING) " + tale.ToString());
                Log.Message("RimTales-DEBUG:(DEF) " +  tale.def);
                Log.Message("RimTales-DEBUG:===================================");

            }
        }

        this.tales.Reverse();        ;
        Log.Message("RimTales: Listing Complete!");
        Log.Message("RimTales: " + this.tales.Count + "/" + NumTales + " Processed. (" + ((float)this.tales.Count / NumTales).ToString("0.00%") + ")");
        
        bCheckRefresh = true; 
    }

    public void saveTales(){
        
        String outputFile;
        String outputMsg;
        outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "rimworld_tales.txt");
        using (var output = new StreamWriter(outputFile, false)){
            foreach (String tale in this.tales){
                output.WriteLine(tale);
            }
        }
        Log.Message("RimTales: Tales exported to " + outputFile);
        outputMsg = System.Environment.NewLine +  System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + "Tales saved to " + outputFile + System.Environment.NewLine;

        //* export the full data in another log.
        if (RimTalesMod.settings.bIsDebugging == true){
            outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "rimworld_tales_debug.txt");
            using (var output = new StreamWriter(outputFile, false)){
                foreach (Tale tale in Find.TaleManager.AllTalesListForReading){
                    output.WriteLine(tale.ToString());
                }
            }
            Log.Message("RimTales: Debugging Tales exported to " + outputFile);
            outputMsg = outputMsg + "Full-text Tales saved to " + outputFile +System.Environment.NewLine;
        }

        Dialog_MessageBox window = new Dialog_MessageBox(outputMsg, "OK!");
		Find.WindowStack.Add(window);

    }

    public void AddTale(Tale tale, String overrideName, String plus){

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
            this.tales.Add(str.ToString());
        }
        else{
            string[] temp = str.ToString().Split(':');
            string final = temp[0] + ":" + temp[1];
            var outstr = final + overrideName + plus;
            this.tales.Add(outstr);
        }
        ProcessedTales = ProcessedTales + 1;
    }
    
}

}
