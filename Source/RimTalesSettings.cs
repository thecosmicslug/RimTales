using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimTales {
    //* Holds the actual settings here
    class RimTalesSettings : ModSettings
    {
        //* Filter Settings
        public bool bShowVommit = true;
        public bool bShowDeaths = true;
        public bool bShowWounded = true;
        public bool bShowAnimalTales= true;
        public bool bShowChitChat= false;
        public bool bShowPlayedGame= false;
        public bool bUseColour = true;

        //* Aniverary Settings taken from RimStory
        public bool enableMarriageAnniversary = true;
        public bool enableMemoryDay = true;
        public bool enableDaysOfVictory = true;
        public bool enableIndividualThoughts = true;
        public bool enableFunerals = true;

        //* Extra debug logs
        public bool bIsDebugging = false;


        public override void ExposeData()
        {
            base.ExposeData();
            //* Filter settings
            Scribe_Values.Look(ref this.bShowDeaths, "ShowDeaths", true);
            Scribe_Values.Look(ref this.bShowVommit, "ShowVommit", true);
            Scribe_Values.Look(ref this.bShowWounded, "ShowWounded", true);
            Scribe_Values.Look(ref this.bShowAnimalTales, "ShowAnimalTales", true);
            //* SPAM!
            Scribe_Values.Look(ref this.bShowChitChat, "ShowChitChat", false);
            Scribe_Values.Look(ref this.bShowPlayedGame, "ShowPlayedGame", false);

            //* UI Settings
            Scribe_Values.Look(ref this.bUseColour, "UseColour", true);
            Scribe_Values.Look(ref this.bIsDebugging, "IsDebugging", false);
            //* Aniversary Settings
            Scribe_Values.Look(ref this.enableMarriageAnniversary, "enableMarriageAnniversary", true);
            Scribe_Values.Look(ref this.enableMemoryDay, "enableMemoryDay", true);
            Scribe_Values.Look(ref this.enableDaysOfVictory, "enableDaysOfVictory", true);
            Scribe_Values.Look(ref this.enableIndividualThoughts, "enableIndividualThoughts", true);
            Scribe_Values.Look(ref this.enableFunerals, "enableFunerals", true);
        }
    }

    //* Draw ModSettings GUI here
    class RimTalesMod : Mod
    {
        public static RimTalesSettings settings;
        public override string SettingsCategory() => "RimTales";
        
        public override void WriteSettings(){
            base.WriteSettings();
        }

        public RimTalesMod(ModContentPack content) : base(content){
            settings = GetSettings<RimTalesSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect){

            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);

            //* List Colouring
            listing_Standard.CheckboxLabeled("EnUseColour".Translate(), ref settings.bUseColour);

            //* Filter settings
            listing_Standard.CheckboxLabeled("EnShowDeaths".Translate(), ref settings.bShowDeaths);
            listing_Standard.CheckboxLabeled("EnShowVommit".Translate(), ref settings.bShowVommit);
            listing_Standard.CheckboxLabeled("EnShowWounded".Translate(), ref settings.bShowWounded);
            listing_Standard.CheckboxLabeled("EnShowAnimalTales".Translate(), ref settings.bShowAnimalTales);
            listing_Standard.CheckboxLabeled("EnShowChitChat".Translate(), ref settings.bShowChitChat);
            listing_Standard.CheckboxLabeled("EnShowPlayedGame".Translate(), ref settings.bShowPlayedGame);

            if (Prefs.DevMode)
            {
                //* Print a second text-file with the raw data.
                listing_Standard.CheckboxLabeled("EnDebugLog".Translate(), ref settings.bIsDebugging);
            }

            //* Aniversary Settings
            listing_Standard.CheckboxLabeled("enableMarriageAnniversary".Translate(), ref settings.enableMarriageAnniversary);
            listing_Standard.CheckboxLabeled("enableMemoryDay".Translate(), ref settings.enableMemoryDay);
            listing_Standard.CheckboxLabeled("enableDaysOfVictory".Translate(), ref settings.enableDaysOfVictory);
            listing_Standard.CheckboxLabeled("enableIndividualThoughts".Translate(), ref settings.enableIndividualThoughts);
            listing_Standard.CheckboxLabeled("enableFunerals".Translate(), ref settings.enableFunerals);


            listing_Standard.End();
            settings.Write();
            
            base.DoSettingsWindowContents(inRect);
        }

    }

}
