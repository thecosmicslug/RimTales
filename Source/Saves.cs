using RimWorld.Planet;
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
            Scribe_References.Look(ref Resources.lastGrave, "RT_LastGrave", true);
            Scribe_Collections.Look(ref Resources.EventManager, "RT_Events", LookMode.Deep, null);
            Scribe_Collections.Look(ref Resources.TaleManager, "RT_Tales", LookMode.Deep, null);
            Scribe_Collections.Look(ref Resources.pawnsAttended, "RT_PawnsAttended", LookMode.Reference, null, true);
            Scribe_Collections.Look(ref Resources.deadPawnsForMassFuneral, "RT_DeadPawns", LookMode.Reference, null,true);
            Scribe_Collections.Look(ref Resources.deadPawnsForMassFuneralBuried, "RT_DeadPawnsBuried",LookMode.Reference, null, true);
            Scribe_Values.Look(ref Resources.isMemorialDayCreated, "RT_Memorial_Day");
        }
    }
}
