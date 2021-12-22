using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Random = System.Random;

namespace RimTales
{
    public static class Resources
    {
        public static Map TEST_MAP;
        public static Random rng = new Random();
        public static int randomChanceRaid = 6;

        public static int minHour = 10;
        public static int maxHour = 20;
        public static Building_Grave lastGrave;

        public static List<Pawn> deadPawnsForMassFuneral = new List<Pawn>();
        public static List<Pawn> deadPawnsForMassFuneralBuried = new List<Pawn>();
        public static List<Building_Grave> graves = new List<Building_Grave>();
        public static List<IEvent> EventManager = new List<IEvent>();
        public static List<Pawn> pawnsAttended = new List<Pawn>();
        public static List<TaleStorage> TaleManager = new List<TaleStorage>();
        public static List<Pawn> deadPawns = new List<Pawn>();

        public static bool isMemorialDayCreated = false;
        public static Date dateLastFuneral = null;

    }
}