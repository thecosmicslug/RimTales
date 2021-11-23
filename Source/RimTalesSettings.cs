using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimTales 
{
    class RimTalesSettings : ModSettings
    {
        public bool bShowVommit = true;
        public bool bShowDeaths = true;
        public bool bShowWounded = true;
        public bool bShowAnimalTales= true;
        public bool bShowChitChat= true;
        public bool bShowPlayedGame= true;

        public bool bUseColour = true;
        public bool bIsDebugging = false;

        public override void ExposeData()
        {
            base.ExposeData();
            //* Filter settings
            Scribe_Values.Look(ref this.bShowDeaths, "ShowDeaths", true);
            Scribe_Values.Look(ref this.bShowVommit, "ShowVommit", true);
            Scribe_Values.Look(ref this.bShowWounded, "ShowWounded", true);
            Scribe_Values.Look(ref this.bShowAnimalTales, "ShowAnimalTales", true);
            Scribe_Values.Look(ref this.bShowChitChat, "ShowChitChat", true);
            Scribe_Values.Look(ref this.bShowPlayedGame, "ShowPlayedGame", true);

            //* UI Settings
            Scribe_Values.Look(ref this.bUseColour, "UseColour", true);
            Scribe_Values.Look(ref this.bIsDebugging, "IsDebugging", false);
        }
    }

class RimTalesMod : Mod
    {
        public static RimTalesSettings settings;

        public RimTalesMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<RimTalesSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {

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

            listing_Standard.End();
            settings.Write();
        }

        public override string SettingsCategory() => "RimTales";
        
        public override void WriteSettings()
        {
            base.WriteSettings();
        }
    }

}
