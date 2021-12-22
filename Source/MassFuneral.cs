using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RimTales
{
    public static class MassFuneral
    {
        private static string deadPawnsNames = "";

        static MassFuneral()
        {
        }

        public static bool TryStartMassFuneral(Map map)
        {
            if (Utils.CurrentDay() != 7 && Utils.CurrentDay() != 14)
            {
                return false;
            }

            var pawn = GatheringsUtility.FindRandomGatheringOrganizer(Faction.OfPlayer, map, GatheringDefOf.Party);
            if (pawn == null)
            {
                return false;
            }

            if (!RCellFinder.TryFindGatheringSpot(pawn, GatheringDefOf.Party, false, out _))
            {
                return false;
            }

            foreach (var deadPawn in Resources.deadPawnsForMassFuneralBuried)
            {
                if (deadPawn != null)
                {
                    deadPawnsNames = $"{deadPawnsNames}{deadPawn.Label}\n";
                }
            }

            var unused = LordMaker.MakeNewLord(pawn.Faction, new LordJob_RimTales(Resources.lastGrave.Position, pawn),map);
            Find.LetterStack.ReceiveLetter("RT_FuneralLetter".Translate(), "RT_FuneralDesc".Translate() + deadPawnsNames,LetterDefOf.NeutralEvent, Resources.lastGrave);
            deadPawnsNames = "";
            return true;
        }
    }
}