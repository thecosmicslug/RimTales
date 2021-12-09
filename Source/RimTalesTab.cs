using System;
using System.Collections.Generic;
using System.IO;
using RimWorld;
using Verse;
using UnityEngine;

namespace RimTales {
    //* Our Tales List GUI 
    public class RimTalesTab : MainTabWindow
{
    public static bool bTabOpen = false; 
    private static List<String> tales = new List<String>();
    private Vector2 scrollPosition = Vector2.zero;
    private float scrollViewHeight;
    private String StrFilter = "";

    public override Vector2 RequestedTabSize{
        get
        {
            return new Vector2(1000f, 500f);
        }
    }

    //* Called when opening window, Set it all up.
    public override void PreOpen(){

        //* Set the list up
        Log.Message("[RimTales]: Refreshing Tale List...");
        tales.Clear();
        foreach (TaleStorage taleTMP in Resources.TaleManager)
        {
            if(CheckTaleFilters(taleTMP)==true){
                tales.Add(taleTMP.ShortSummary);
            }
        }
        tales.Reverse();
        base.PreOpen();
    }

    public override void PostOpen(){

        bTabOpen=true;
        Log.Message("[RimTales]: Showing Main Tab GUI.");
        StrFilter = "";
        base.PostOpen();
    }

    //* Called when closing window.
    public override void PostClose(){

        bTabOpen=false;
        Log.Message("[RimTales]: Closing Main Tab GUI.");
        base.PostClose();
    }

    //* Draw our GUI on the Window.
    public override void DoWindowContents(Rect fillRect){

        Text.Font = GameFont.Medium;
        Widgets.Label(fillRect, "RimTales");
        Rect position = new Rect(0f, 0f, fillRect.width, fillRect.height);
        GUI.BeginGroup(position);
        Text.Font = GameFont.Small;
        GUI.color = Color.white;
        Rect outRect = new Rect(0f, 50f, position.width, position.height - 50f);
        Rect position2 = new Rect(100f, 0f, 110F, 30F);
        Rect position3 = new Rect(280f, 5f, 110F, 30F);
        Widgets.Label(position3, "EnFilter".Translate());

        Rect position4 = new Rect(320f, 0f, 110F, 30F);
        StrFilter = Widgets.TextField(position4, StrFilter);

        Rect position6 = new Rect(615f, 0f, 300f, 30f);
        if(Widgets.ButtonText(position6, "EnSaveList".Translate())){
            saveTales();
        }

        Rect rect = new Rect(0f, 0f, position.width - 16f, this.scrollViewHeight);
        Widgets.BeginScrollView(outRect, ref this.scrollPosition, rect);

        float num = 0f;
        foreach (String tale in tales)
        {
            bool bShow = false;
            if(StrFilter == ""){
                bShow = true;
            }
            else if (tale.IndexOf(StrFilter, StringComparison.OrdinalIgnoreCase) >= 0){
                bShow = true;
            }
            if (bShow){

                GUI.color = Color.white;
                if (RimTalesMod.settings.bUseColour == true){
                    if (tale.Contains("Recruit") || tale.Contains("nimal") || tale.Contains("Hunted")) GUI.color = Color.green;
                    if (tale.Contains("Traded") || tale.Contains("Struck")) GUI.color = Color.gray;
                    if (tale.Contains("risoner") || tale.Contains("aked") || tale.Contains("Gave up") || tale.Contains("Wounded")) GUI.color = Color.yellow;
                    if (tale.Contains("Marriage") || tale.Contains("Breakup") || tale.Contains("lover")) GUI.color = Color.magenta;
                    if (tale.Contains("Death") || tale.Contains("Kidnap") || tale.Contains("Berserk") || tale.Contains("Kill") || tale.Contains("kill") || tale.Contains("Raid") || tale.Contains("Human") || tale.Contains("Downed")) GUI.color = Color.red;
                    if (tale.Contains("Research") || tale.Contains("Landed") || tale.Contains("Surgery")) GUI.color = Color.cyan;
                }
                
                Rect rect2 = new Rect(0f, num, rect.width, 30f);
                if (Mouse.IsOver(rect2)){
                    GUI.DrawTexture(rect2, TexUI.HighlightTex);
                }
                Widgets.Label(rect2, tale);
                num += 30f;
            }
        }

            
        GUI.color = Color.white;
        if (Event.current.type == EventType.Layout){
            this.scrollViewHeight = num;
        }
        Widgets.EndScrollView();
        GUI.EndGroup();

    }

    //* Hook detected new tale, add it to the GUI
    public static void UpdateList(TaleStorage taleTMP){

        if(CheckTaleFilters(taleTMP)==true){
            RimTalesTab.tales.Reverse();
            tales.Add(taleTMP.ShortSummary);
            RimTalesTab.tales.Reverse();
        }

    }

    //* Export Recorded Tales to disk
    private void saveTales(){
        
        String outputFile;
        String outputMsg;
        outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "rimworld_tales.txt");
        using (var output = new StreamWriter(outputFile, false)){
            foreach (String tale in tales){
                output.WriteLine(tale);
            }
        }
        Log.Message("[RimTales]: Filtered Tales exported to " + outputFile);
        outputMsg = "Tales saved to " + outputFile + System.Environment.NewLine + System.Environment.NewLine;

        //* export the full data in another log.
        if (RimTalesMod.settings.bIsDebugging == true){
            outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "rimworld_tales_debug.txt");
            using (var output = new StreamWriter(outputFile, false)){
                foreach (Tale tale in Find.TaleManager.AllTalesListForReading){
                    output.WriteLine(tale.ToString());
                }
            }
            Log.Message("[RimTales]: Debugging Tales exported to " + outputFile);
            outputMsg = outputMsg + "Full-text Tales saved to " + outputFile +System.Environment.NewLine;

        }

        Dialog_MessageBox window = new Dialog_MessageBox(outputMsg, "OK!");
		Find.WindowStack.Add(window);

    }

    public static bool CheckTaleFilters(TaleStorage TaleTMP){

        switch (TaleTMP.def.defName)
        {
            case "Vomited":
                return RimTalesMod.settings.bShowVommit;
            case "PlayedGame":
                return RimTalesMod.settings.bShowPlayedGame;
            case "VSIE_WeHadNiceChat":
                return RimTalesMod.settings.bShowChitChat;;
            case "Wounded":
                return RimTalesMod.settings.bShowWounded;
            case "TamedAnimal":
                return RimTalesMod.settings.bShowAnimalTales;
            case "TrainedAnimal":
                return RimTalesMod.settings.bShowAnimalTales;
            case "BondedWithAnimal":
                return RimTalesMod.settings.bShowAnimalTales;
            case "KilledColonist":
                return RimTalesMod.settings.bShowDeaths;
            case "KilledMajorThreat":
                return RimTalesMod.settings.bShowDeaths;
            case "KilledBy":
                return RimTalesMod.settings.bShowDeaths;
            case "KilledMortar":
                return RimTalesMod.settings.bShowDeaths;
            case "KilledLongRange":
                return RimTalesMod.settings.bShowDeaths;
            case "KilledMelee":
                return RimTalesMod.settings.bShowDeaths;
            case "KilledCapacity":
                return RimTalesMod.settings.bShowDeaths;
            default:
                //* Don't need to filter anything else
                return true;
        }
    }

}

}
