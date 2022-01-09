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
        
        public static int randomChanceRaid = 10;
        public static int minHour = 10;
        public static int maxHour = 20;
        public static bool isMemorialDayCreated = false;

        public static List<IEvent> EventManager = new List<IEvent>();
        public static List<TaleStorage> TaleManager = new List<TaleStorage>();
    }
}