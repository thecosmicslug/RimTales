using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimTales
{
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

            listing_Standard.CheckboxLabeled("EnUseColour".Translate(), ref settings.bUseColour);
            listing_Standard.CheckboxLabeled("EnShowDeaths".Translate(), ref settings.bShowDeaths);
            listing_Standard.CheckboxLabeled("EnShowVommit".Translate(), ref settings.bShowVommit);
            listing_Standard.CheckboxLabeled("EnShowWounded".Translate(), ref settings.bShowWounded);
            listing_Standard.CheckboxLabeled("EnShowAnimalTraining".Translate(), ref settings.bShowAnimalTraining);

            if (Prefs.DevMode)
            {
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
