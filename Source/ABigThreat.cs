﻿using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RimTales
{
    internal class ABigThreat : IEvent
    {
        private bool anniversary = true;
        private Date date;
        private Faction faction;
        private List<int> yearsWhenEventStarted = new List<int>();

        public ABigThreat()
        {
        }

        public ABigThreat(Date date, Faction faction)
        {
            this.date = date;
            this.faction = faction;
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
            Scribe_Values.Look(ref anniversary, "ff", true);
            Scribe_Collections.Look(ref yearsWhenEventStarted, "RT_yearsWhenEventStarted", LookMode.Value);
            Scribe_References.Look(ref faction, "RT_FactionAttacked");
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
            if (faction != null && date != null)
            {
                return (date.day + " " + date.quadrum + " " + date.year + " " + "RT_ColonyAttacked".Translate(faction.Name));
            }
            else if (faction == null && date != null){
                return (date.day + " " + date.quadrum + " " + date.year + " " + "RT_ColonyRaided".Translate());
            }
            else 
            {
                return ("RT_ColonyRaided".Translate());
            }
        }

        public bool TryStartEvent()
        {
            return false;
        }

        public bool TryStartEvent(Map map)
        {
            if (faction == null)
            {
                return false;
            }

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
            var unused = LordMaker.MakeNewLord(pawn.Faction, new LordJob_Joinable_Party(intVec, pawn, GatheringDefOf.Party), map);

            string label = "RT_DayOfVictory".Translate(faction.Name);
            string text = "RT_DayOfVictoryDesc".Translate(faction.Name, date.ToString());
            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.PositiveEvent);
            Core.AddIncident("AnniversaryThreat", text);
            return true;
        }

        private void AddAttendedMemorialDay()
        {
        }
    }
}