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
            Log.Message("RimTales: RecordTale() - " + tale.ToString());
            Resources.tales.Add(tale);
            
            if (RimTalesTab.bTabOpen == true){
                RimTalesTab.RefreshTales();
            }
        }


    }
}