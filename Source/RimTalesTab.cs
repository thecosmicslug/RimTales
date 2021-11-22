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
    //private const float FactionColorRectSize = 15f;
    //private const float FactionColorRectGap = 10f;
    //private const float RowMinHeight = 80f;
    //private const float LabelRowHeight = 50f;
    //private const float TypeColumnWidth = 100f;
    //private const float NameColumnWidth = 220f;
    //private const float RelationsColumnWidth = 100f;
    //private const float NameLeftMargin = 15f;

    private Vector2 scrollPosition = Vector2.zero;
    private float scrollViewHeight;

    private String filter = "";
    private float TickCount = 0;
    private float NumTales = 0;
    private bool bCheckRefresh = true; 

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
    }
    //*

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
            if (TickCount >= 200){
                TickCount = 0;
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
        
        NumTales = Find.TaleManager.AllTalesListForReading.Count;
        foreach (Tale tale in Find.TaleManager.AllTalesListForReading){

            String plus = "";
            String overrideName = "";

            if (tale is Tale_SinglePawnAndThing){
                Tale_SinglePawnAndThing tale2 = tale as Tale_SinglePawnAndThing;
                if (tale2.thingData != null){
                    plus = " - " + tale2.thingData.thingDef.LabelCap + ".";
                }
                //AddTale(tale,overrideName,plus);
            }
            else if (tale is Tale_DoublePawnKilledBy){
                Tale_DoublePawnKilledBy tale2 = tale as Tale_DoublePawnKilledBy;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    plus = " - Killer: " + tale2.secondPawnData.name + ".";
                }
                if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    plus = " - Killer: " + tale2.secondPawnData.kind.LabelCap + ".";
                }
                if (RimTalesMod.settings.bShowDeaths == true){
                    if (tale.def != TaleDefOf.KilledColonyAnimal){
                    //    AddTale(tale,overrideName,plus);
                    }
                }
            }

            if (tale.def == TaleDefOf.FinishedResearchProject){
                Tale_SinglePawnAndDef tale2 = tale as Tale_SinglePawnAndDef;
                plus = " - " + tale2.defData.def.LabelCap + ".";
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.Vomited){
                if (RimTalesMod.settings.bShowVommit == true){
                    AddTale(tale,overrideName,plus);
                }
                continue;
            }

            if (tale.def == TaleDefOf.Recruited){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    plus = " - Joiner: " + tale2.secondPawnData.name + ".";
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    overrideName = " - Recruiter: " + tale2.firstPawnData.name;
                }
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.Hunted){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    plus = " - Prey: " + tale2.secondPawnData.name + ".";
                }
                if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    plus = " - Prey: " + tale2.secondPawnData.kind.LabelCap + ".";
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    overrideName = " - Hunter: " + tale2.firstPawnData.name;
                }
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.KidnappedColonist){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    plus = " - Victim: " + tale2.secondPawnData.name + ".";
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    overrideName = " - Kidnapper: " + tale2.firstPawnData.name;
                }
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.DidSurgery){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    plus = " - Patient: " + tale2.secondPawnData.name + ".";
                }
                if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    plus = " - Patient: " + tale2.secondPawnData.kind.LabelCap + ".";
                }
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.KilledColonist){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    plus = " - Colonist: " + tale2.secondPawnData.name + ".";
                }
                if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                    overrideName = " - Killer: " + tale2.firstPawnData.kind.LabelCap;
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    overrideName = " - Killer: " + tale2.firstPawnData.name;
                }
                if (RimTalesMod.settings.bShowDeaths == true){
                    AddTale(tale,overrideName,plus);
                }
                continue;
            }

            if (tale.def == TaleDefOf.KilledMajorThreat){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    plus = " - Enemy: " + tale2.secondPawnData.name + ".";
                }
                if (RimTalesMod.settings.bShowDeaths == true){
                    AddTale(tale,overrideName,plus);
                }
                continue;
            }

            if (tale.def == TaleDefOf.TradedWith){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    plus = " - Trader: " + tale2.secondPawnData.name + ".";
                }
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.BecameLover || tale.def == TaleDefOf.Breakup || tale.def == TaleDefOf.Marriage){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    plus = " and " + tale2.secondPawnData.name + ".";
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    overrideName = ": " + tale2.firstPawnData.name + "";
                }
                AddTale(tale,overrideName,plus);
                continue;
            }                
  
            if (tale.def == TaleDefOf.Captured || tale.def == TaleDefOf.ExecutedPrisoner || tale.def == TaleDefOf.SoldPrisoner){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    plus = " - Animal: " + tale2.secondPawnData.kind.LabelCap + ".";
                }
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    plus = " - Prisoner: " + tale2.secondPawnData.name + ".";
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    overrideName = " - Warden: " + tale2.firstPawnData.name;
                }
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.TamedAnimal || tale.def == TaleDefOf.TrainedAnimal || tale.def == TaleDefOf.BondedWithAnimal){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    plus = " - Animal: " + tale2.secondPawnData.kind.LabelCap + ".";
                }
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    plus = " - Animal: " + tale2.secondPawnData.name + ".";
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    overrideName = " - Handler: " + tale2.firstPawnData.name;
                }
                if (RimTalesMod.settings.bShowAnimalTraining == true){
                    AddTale(tale,overrideName,plus);
                }
                continue;
            }

            if (tale.def == TaleDefOf.KilledColonyAnimal){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    plus = " - Animal: " + tale2.secondPawnData.kind.LabelCap + ".";
                }
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    plus = " - Animal: " + tale2.secondPawnData.name + ".";
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    overrideName = " - Killer: " + tale2.firstPawnData.name;
                }
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.LandedInPod){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.BuriedCorpse){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    plus = " - Body: " + tale2.secondPawnData.kind.LabelCap;
                }
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    plus = " - Body: " + tale2.secondPawnData.name + ".";
                }
                if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                    overrideName = " - Worker: " + tale2.firstPawnData.kind.LabelCap;
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    overrideName = " - Worker: " + tale2.firstPawnData.name;
                }
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.WasOnFire){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.CompletedLongConstructionProject){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.CaravanAmbushedByHumanlike){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.IllnessRevealed){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.IncreasedMenagerie){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.MinedValuable){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.CaravanRemoteMining){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.AttendedConcert){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.HeldConcert){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.AttendedParty){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.StruckMineable){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.Exhausted){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.WalkedNaked){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.SocialFight){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.GainedMasterSkillWithPassion){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.KilledBy){
                if (RimTalesMod.settings.bShowDeaths == true){
                    AddTale(tale,overrideName,plus);
                }
                continue;
            }

            if (tale.def == TaleDefOf.KilledMortar){
                if (RimTalesMod.settings.bShowDeaths == true){
                    AddTale(tale,overrideName,plus);
                }
                continue;
            }

            if (tale.def == TaleDefOf.KilledLongRange){
                if (RimTalesMod.settings.bShowDeaths == true){
                    AddTale(tale,overrideName,plus);
                }
                continue;
            }

            if (tale.def == TaleDefOf.KilledMelee){
                if (RimTalesMod.settings.bShowDeaths == true){
                    AddTale(tale,overrideName,plus);
                }
                continue;
            }

            if (tale.def == TaleDefOf.DefeatedHostileFactionLeader){
                if (RimTalesMod.settings.bShowDeaths == true){
                    AddTale(tale,overrideName,plus);
                }
                continue;
            }

            if (tale.def == TaleDefOf.KilledCapacity){
                if (RimTalesMod.settings.bShowDeaths == true){
                    AddTale(tale,overrideName,plus);
                }
                continue;
            }

            if (tale.def == TaleDefOf.CaravanFormed){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.CaravanFled){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.CaravanAmbushDefeated){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.CaravanAssaultSuccessful){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.ButcheredHumanlikeCorpse){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.CraftedArt){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.GaveBirth){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    plus = " - Child: " + tale2.secondPawnData.kind.LabelCap;
                }
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    plus = " - Child: " + tale2.secondPawnData.name + ".";
                }
                if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                    overrideName = " - Mother: " + tale2.firstPawnData.kind.LabelCap;
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    overrideName = " - Mother: " + tale2.firstPawnData.name;
                }
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.AteRawHumanlikeMeat){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.CompletedLongCraftingProject){
                Tale_SinglePawnAndDef tale2 = tale as Tale_SinglePawnAndDef;
                plus = " - " + tale2.defData.def.LabelCap + ".";
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == TaleDefOf.Wounded){
                if (RimTalesMod.settings.bShowWounded == true){
                    Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                    if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                        plus = " - Attacker: " + tale2.secondPawnData.name + ".";
                    }
                    if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                        plus = " - Attacker: " + tale2.secondPawnData.kind.LabelCap + ".";
                    }
                    if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                        overrideName = " - Victim: " + tale2.firstPawnData.name;
                    }
                    if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                        overrideName = " - Victim: " + tale2.firstPawnData.kind.LabelCap + ".";
                    }
                    AddTale(tale,overrideName,plus);
                    continue;
                } 
            }

            if (tale.def == TaleDefOf.Downed){
                Tale_DoublePawn tale2 = tale as Tale_DoublePawn;
                if (tale2.secondPawnData != null && tale2.secondPawnData.name != null){
                    plus = " - Attacker: " + tale2.secondPawnData.name + ".";
                }
                if (tale2.secondPawnData != null && !tale2.secondPawnData.kind.RaceProps.Humanlike){
                    plus = " - Attacker: " + tale2.secondPawnData.kind.LabelCap + ".";
                }
                if (tale2.firstPawnData != null && tale2.firstPawnData.name != null){
                    overrideName = " - Victim: " + tale2.firstPawnData.name;
                }
                if (tale2.firstPawnData != null && !tale2.firstPawnData.kind.RaceProps.Humanlike){
                    overrideName = " - Victim: " + tale2.firstPawnData.kind.LabelCap + ".";
                }
                AddTale(tale,overrideName,plus);
                continue;
            }

            //* Not sure what this means?!
            if (tale.def == TaleDefOf.CollapseDodged){
                AddTale(tale,overrideName,plus);
                continue;
            }

            //*  VanillaSocialInteractionsExpanded stuff
            if (tale.def == VIE_DefOf.VSIE_BondedPetButchered){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_ExposedCorpseOfMyFriend){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_IngestedHumanFlesh){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_BingedFood){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_BingedDrug){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_HideInRoom){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_ThrewTantrum){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_WanderedInSaddness){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_WentBerserk){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_WentIntoSadisticRage){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_WentOnFireStartingSpree){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_WentOnMurderousRage){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_RanWild){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_InsultedMe){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_SlaughteredAnimalInRage){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_RebuffedMe){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_InducedPrisonerToEscape){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_TamedThrumbo){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_WasPreviouslyOurEnemy){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_WasBadlyInjured){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_DidNotAttendWedding){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_AttendedMyWedding){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_RemovedPrisonersOrgans){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_FailedMedicalOperationAndKilled){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_TamedMe){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_ArrestedMe){
                AddTale(tale,overrideName,plus);
                continue;
            }


            if (tale.def == VIE_DefOf.VSIE_ResurrectedMe){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_BrokeUpWithMe){
                AddTale(tale,overrideName,plus);
                continue;
            }
            
            if (tale.def == VIE_DefOf.VSIE_WeHadNiceChat){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_WeHadSocialFight){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_SavedMeFromMyWounds){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_HasBeenMyFriendSinceChildhood){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_SavedMeFromRaiders){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_StoleMyLover){
                AddTale(tale,overrideName,plus);
                continue;
            }

            if (tale.def == VIE_DefOf.VSIE_CuredMyFriend){
                AddTale(tale,overrideName,plus);
                continue;
            }

        }
        this.tales.Reverse();
        Log.Message("RimTales: Listing Complete!");
        bCheckRefresh = true; 
    }

    public void saveTales(){
        
        String outputFile;
        outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "rimworld_tales.txt");
        using (var output = new StreamWriter(outputFile, false)){
            foreach (String tale in this.tales){
                output.WriteLine(tale);
            }
        }
        Log.Message("RimTales: Tales exported to " + outputFile);

        //* export the full data in another log.
        if (RimTalesMod.settings.bIsDebugging == true){
            outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "rimworld_tales_debug.txt");
            using (var output = new StreamWriter(outputFile, false)){
                foreach (Tale tale in Find.TaleManager.AllTalesListForReading){
                    output.WriteLine(tale.ToString());
                }
            }
            Log.Message("RimTales: Debugging Tales exported to " + outputFile);
        }
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
    }
    

}

}
