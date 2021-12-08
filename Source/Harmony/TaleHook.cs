using System;
using System.Text;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;

namespace RimTales
{
    [HarmonyPatch(typeof(TaleManager))]
    [HarmonyPatch("Add")]
    internal class TaleHook
    {

        private static void Postfix(Tale tale)
        {
            Core.IncomingTale(tale);
        }


    }
}