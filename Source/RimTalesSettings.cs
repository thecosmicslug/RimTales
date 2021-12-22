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

        public override void ExposeData()
        {
            base.ExposeData();
            //* UI Settings
            Scribe_Values.Look(ref this.bUseColour, "UseColour", true);
            //* Filter settings
            Scribe_Values.Look(ref this.bShowDeaths, "ShowDeaths", true);
            Scribe_Values.Look(ref this.bShowVommit, "ShowVommit", true);
            Scribe_Values.Look(ref this.bShowWounded, "ShowWounded", true);
            Scribe_Values.Look(ref this.bShowAnimalTales, "ShowAnimalTales", true);
            Scribe_Values.Look(ref this.bShowChitChat, "ShowChitChat", false);
            Scribe_Values.Look(ref this.bShowPlayedGame, "ShowPlayedGame", false);

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

            Text.Font = GameFont.Medium;
            listing_Standard.Label("RT_FilterSettings".Translate());
            Text.Font = GameFont.Small;

            listing_Standard.Gap(10f);
            listing_Standard.Gap(10f);

            //* List Colouring
            listing_Standard.CheckboxLabeled("RT_UseColour".Translate(), ref settings.bUseColour);
            listing_Standard.Gap(10f);

            //* Filter settings
            listing_Standard.CheckboxLabeled("RT_ShowDeaths".Translate(), ref settings.bShowDeaths);
            listing_Standard.Gap(10f);
            listing_Standard.CheckboxLabeled("RT_ShowVommit".Translate(), ref settings.bShowVommit);
            listing_Standard.Gap(10f);
            listing_Standard.CheckboxLabeled("RT_ShowWounded".Translate(), ref settings.bShowWounded);
            listing_Standard.Gap(10f);
            listing_Standard.CheckboxLabeled("RT_ShowAnimalTales".Translate(), ref settings.bShowAnimalTales);
            listing_Standard.Gap(10f);
            listing_Standard.CheckboxLabeled("RT_ShowChitChat".Translate(), ref settings.bShowChitChat);
            listing_Standard.Gap(10f);
            listing_Standard.CheckboxLabeled("RT_ShowPlayedGame".Translate(), ref settings.bShowPlayedGame);
            listing_Standard.Gap(10f);
            listing_Standard.Gap(10f);
            
            Text.Font = GameFont.Medium;
            listing_Standard.Label("RT_AnniversarySettings".Translate());
            
            Text.Font = GameFont.Small;

            listing_Standard.Gap(10f);
            listing_Standard.Gap(10f);

            //* Aniversary Settings
            listing_Standard.CheckboxLabeled("RT_EnableMarriageAnniversary".Translate(), ref settings.enableMarriageAnniversary);
            listing_Standard.Gap(10f);
            listing_Standard.CheckboxLabeled("RT_EnableMemoryDay".Translate(), ref settings.enableMemoryDay);
            listing_Standard.Gap(10f);
            listing_Standard.CheckboxLabeled("RT_EnableDaysOfVictory".Translate(), ref settings.enableDaysOfVictory);
            listing_Standard.Gap(10f);
            listing_Standard.CheckboxLabeled("RT_EnableIndividualThoughts".Translate(), ref settings.enableIndividualThoughts);
            listing_Standard.Gap(10f);
            listing_Standard.CheckboxLabeled("RT_EnableFunerals".Translate(), ref settings.enableFunerals);

            listing_Standard.End();
            settings.Write();
            
            base.DoSettingsWindowContents(inRect);
        }

    }

}
