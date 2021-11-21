using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimTales 
{
    class RimTalesSettings : ModSettings
    {
        public bool bUseColour = true;

        public bool bShowDeaths = true;
        public bool bShowVommit = true;
        public bool bShowWounded = true;
        public bool bShowAnimalTraining = true;
        public bool bIsDebugging = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.bShowDeaths, "ShowDeaths", true);
            Scribe_Values.Look(ref this.bShowVommit, "ShowVommit", true);
            Scribe_Values.Look(ref this.bShowWounded, "ShowWounded", true);
            Scribe_Values.Look(ref this.bShowAnimalTraining, "ShowAnimalTraining", true);
            Scribe_Values.Look(ref this.bUseColour, "UseColour", true);
            Scribe_Values.Look(ref this.bIsDebugging, "IsDebugging", false);
        }
    }
}
