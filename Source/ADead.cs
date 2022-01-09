using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RimTales
{
    internal class ADead : IEvent
    {
        private bool anniversary = true;
        private Date date;
        private Pawn deadPawn;
        private List<int> yearsWhenEventStarted = new List<int>();

        public ADead()
        {
        }

        public ADead(Date date, Pawn deadPawn)
        {
            this.date = date;
            this.deadPawn = deadPawn;
        }

        public Date Date()
        {
            return date;
        }

        public void EndEvent()
        {
            throw new NotImplementedException();
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref anniversary, "RT_Anniversary", true);
            Scribe_References.Look(ref deadPawn, "RT_DeadPawn", true);
            Scribe_Collections.Look(ref yearsWhenEventStarted, "RT_YearsWhenEventStarted", LookMode.Value);
            Scribe_Deep.Look(ref date, "RT_DateAttacked");
        }

        public bool GetIsAnniversary()
        {
            return anniversary;
        }

        public bool IsStillEvent()
        {
            throw new NotImplementedException();
        }

        public string ShowInLog()
        {
            if (deadPawn != null) {
                return (deadPawn.Name + " - Died on " + date.day + " " + date.quadrum + " " + date.year);
            }
            return (date.day + " " + date.quadrum + " " + date.year + " colonist died");
        }

        public bool TryStartEvent()
        {
            throw new NotImplementedException();
        }

        public bool TryStartEvent(Map map)
        {
            var unused = deadPawn.relations.RelatedPawns;


            if (Utils.CurrentDay() != date.day || Utils.CurrentQuadrum() != date.quadrum || Utils.CurrentYear() == date.year){
                return true;
            }

            foreach (var p in deadPawn.relations.PotentiallyRelatedPawns)
            {
                if (p.Dead)
                {
                    continue;
                }

                foreach (var item in p.GetRelations(deadPawn))
                {
                    if (item == PawnRelationDefOf.Fiance)
                    {
                        p.needs.mood.thoughts.memories.TryGainMemory(Thoughts.RT_DayDiedFiance);
                    }

                    if (item == PawnRelationDefOf.Lover)
                    {
                        p.needs.mood.thoughts.memories.TryGainMemory(Thoughts.RT_DayDiedLover);
                    }

                    if (item == PawnRelationDefOf.Spouse)
                    {
                        p.needs.mood.thoughts.memories.TryGainMemory(Thoughts.RT_DayDiedSpouse);
                    }

                    if (p.relations.DirectRelationExists(PawnRelationDefOf.Parent, deadPawn))
                    {
                        p.needs.mood.thoughts.memories.TryGainMemory(Thoughts.RT_DayDiedFather);
                    }

                    if (p.relations.DirectRelationExists(PawnRelationDefOf.Parent, deadPawn))
                    {
                        p.needs.mood.thoughts.memories.TryGainMemory(Thoughts.RT_DayDiedFiance);
                    }

                    if (item == PawnRelationDefOf.Child)
                    {
                        p.needs.mood.thoughts.memories.TryGainMemory(Thoughts.RT_DayDiedChild);
                    }

                    if (item == PawnRelationDefOf.Sibling)
                    {
                        p.needs.mood.thoughts.memories.TryGainMemory(Thoughts.RT_DayDiedSibling);
                    }
                }
            }

            yearsWhenEventStarted.Add(Utils.CurrentYear());
            //* Added a tale for wedding anniversary
            Core.AddIncident(Core.RimTales_DefOf.AnniversaryDeath, "RT_AFuneral".Translate(deadPawn.LabelShort, yearsWhenEventStarted.Count, date.day + " " + date.quadrum + " " + date.year));
            return true;
        }

        private void AddDeadOfRelativeThoughts(Pawn pawn)
        {
            pawn.needs.mood.thoughts.memories.TryGainMemory(Thoughts.RT_AttendedMemorialDay);
        }
    }
}