using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace RimTales
{
    internal class Breakup : IEvent
    {
        private bool anniversary = true;
        private Date date;
        private Pawn pawn1, pawn2;
        private Thought thought;
        private List<int> yearsWhenEventStarted = new List<int>();

        public Breakup()
        {
        }

        public Breakup(Date date, Pawn pawn1, Pawn pawn2, Thought thought)
        {
            this.date = date;
            this.pawn1 = pawn1;
            this.pawn2 = pawn2;
            this.thought = thought;
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
            Scribe_References.Look(ref pawn1, "RT_DeadPawn1");
            Scribe_References.Look(ref pawn2, "RT_DeadPawn2");
            Scribe_Collections.Look(ref yearsWhenEventStarted, "RT_yearsWhenEventStarted", LookMode.Value);
            Scribe_Deep.Look(ref date, "RT_DateAttacked");
        }

        public bool GetIsAnniversary()
        {
            throw new NotImplementedException();
        }

        public bool IsStillEvent()
        {
            throw new NotImplementedException();
        }

        public bool TryStartEvent()
        {
            return false;
        }

        public bool TryStartEvent(Map map)
        {
            throw new NotImplementedException();
        }
    }
}