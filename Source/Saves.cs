using RimWorld.Planet;
using Verse;

namespace RimTales
{
    public class Saves : WorldComponent
    {
        public Saves(World world) : base(world)
        {
        }

        //* Saving our Collections & objects in the save-game.
        public override void ExposeData()
        {
            Scribe_Values.Look(ref Resources.isMemorialDayCreated, "RT_Memorial_Day",false);
            Scribe_Collections.Look(ref Resources.EventManager, "RT_Events", LookMode.Deep, null);
            Scribe_Collections.Look(ref Resources.TaleManager, "RT_Tales", LookMode.Deep, null);
        }
    }
}
