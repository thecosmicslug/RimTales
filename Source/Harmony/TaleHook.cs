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
            Resources.tales.Add(tale);
            if(RimTalesMod.settings.bIsDebugging == true){
                Log.Message("[RimTales]: TaleManager.Add() - " + tale.ToString());
            }
            
            //* Tell the GUI to refresh
            if (RimTalesTab.bTabOpen == true){
                RimTalesTab.RefreshTales();
            }
        }


    }
}