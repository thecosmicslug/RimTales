﻿using RimWorld.Planet;
using Verse;

namespace RimTales
{
    public class Saves : WorldComponent
    {
        public Saves(World world) : base(world)
        {
        }

        public override void ExposeData()
        {
            Scribe_References.Look(ref Resources.lastGrave, "RS_LastGrave", true);
            Scribe_Collections.Look(ref Resources.EventManager, "RS_Events", LookMode.Deep, null);
            Scribe_Collections.Look(ref Resources.pawnsAttended, "RS_PawnsAttended", LookMode.Reference, null, true);
            Scribe_Collections.Look(ref Resources.deadPawnsForMassFuneral, "RS_DeadPawns", LookMode.Reference, null,true);
            Scribe_Collections.Look(ref Resources.deadPawnsForMassFuneralBuried, "RS_DeadPawnsBuried",LookMode.Reference, null, true);
            Scribe_Values.Look(ref Resources.isMemorialDayCreated, "RS_Memorial_Day");
            Scribe_Values.Look(ref Resources.showRaidsInLog, "RS_ShowRaidInLog", true);
            Scribe_Values.Look(ref Resources.showRaidsInLog, "RS_ShowDeadColonistsInLog", true);
            Scribe_Values.Look(ref Resources.showRaidsInLog, "RS_showIncidentsInLog", true);
        }
    }
}
