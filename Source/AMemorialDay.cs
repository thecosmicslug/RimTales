﻿using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RimTales
{
    internal class AMemorialDay : IEvent
    {
        private bool anniversary = true;
        private Date date;
        private Pawn deadPawn;
        private List<int> yearsWhenEventStarted = new List<int>();

        public AMemorialDay()
        {
        }

        public AMemorialDay(Date date, Pawn deadPawn)
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
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref anniversary, "RT_Anniversary", true);
            Scribe_References.Look(ref deadPawn, "RT_DeadPawn", true);
            Scribe_Collections.Look(ref yearsWhenEventStarted, "RT_yearsWhenEventStarted", LookMode.Value);
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
                return (date.day + " " + date.quadrum + " " + date.year + " - " + "RT_AMemorialDayLetter".Translate());
            }
            return (date.day + " " + date.quadrum + " " + date.year + "RT_AMemorialDayLetter".Translate());
        }

        public bool TryStartEvent()
        {
            throw new NotImplementedException();
        }

        public bool TryStartEvent(Map map)
        {
            var isThisYear = true;
            foreach (var y in yearsWhenEventStarted)
            {
                if (y == Utils.CurrentYear())
                {
                    isThisYear = false;
                }
            }

            if (Utils.CurrentDay() != date.day || Utils.CurrentQuadrum() != date.quadrum ||
                Utils.CurrentHour() < Resources.minHour || Utils.CurrentHour() > Resources.maxHour ||
                Utils.CurrentYear() == date.year || !isThisYear)
            {
                return false;
            }

            var pawn = GatheringsUtility.FindRandomGatheringOrganizer(Faction.OfPlayer, map, GatheringDefOf.Party);
            if (pawn == null)
            {
                return false;
            }

            if (!RCellFinder.TryFindGatheringSpot(pawn, GatheringDefOf.Party, true, out var intVec))
            {
                return false;
            }

            yearsWhenEventStarted.Add(Utils.CurrentYear());

            string StrOutput ="";
            if (Resources.EventManager.Count > 0){
                foreach (var e in Resources.EventManager){
                    if (e is ADead){
                        StrOutput = StrOutput + e.ShowInLog() + "\n";
                    }
                }
                StrOutput = StrOutput + "\n";
            }

            var unused = LordMaker.MakeNewLord(pawn.Faction, new LordJob_Joinable_Party(intVec, pawn, GatheringDefOf.Party), map);
            Find.LetterStack.ReceiveLetter("RT_AMemorialDayLetter".Translate(), "RT_AMemorialDayDesc".Translate() + StrOutput ,LetterDefOf.PositiveEvent);

            if (deadPawn != null){
                Core.AddIncident("AnniversaryMemorialDay", "RT_AMemorialDayLetter".Translate() + "RT_AMemorialDayDesc".Translate());
            }
       
            return true;
        }

        private void AddAttendedMemorialDay(Pawn pawn)
        {
            pawn.needs.mood.thoughts.memories.TryGainMemory(Thoughts.RT_AttendedMemorialDay);
        }
    }
}