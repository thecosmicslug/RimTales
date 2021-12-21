using System;
using System.Collections.Generic;
using System.IO;
using RimWorld;
using Verse;
using UnityEngine;

namespace RimTales {

    public class RimTalesTab : MainTabWindow
{
    public static bool bTabOpen = false; 
    private static List<String> tales = new List<String>();
    private Vector2 scrollPosition = Vector2.zero;
    private float scrollViewHeight;
    private static String StrFilter = "";

    private enum RimTab : byte
    {
        Log,
        Settings,
        About
    }

    private static RimTab curTab = RimTab.Log;
    private List<TabRecord> tabs = new List<TabRecord>();
    public override Vector2 RequestedTabSize => new Vector2(1000f, 480f);

    private bool bTabShowVommit = true;
    private bool bTabShowDeaths = true;
    private bool bTabShowWounded = true;
    private bool bTabShowAnimalTales= true;
    private bool bTabShowChitChat= true;
    private bool bTabShowPlayedGame= true;

    //* Called when opening window, Set it all up.
    public override void PreOpen(){

        StrFilter = "";
        ImportStringTales();

        tabs.Clear();
		tabs.Add(new TabRecord("Log", delegate
			{
				curTab = RimTab.Log;
			}, () => curTab == RimTab.Log));
			tabs.Add(new TabRecord("Settings", delegate
			{
				curTab = RimTab.Settings;
			}, () => curTab == RimTab.Settings));
			tabs.Add(new TabRecord("About", delegate
			{
				curTab = RimTab.About;
			}, () => curTab == RimTab.About));

        scrollPosition = Vector2.zero;
        bTabOpen=true;
        base.PreOpen();
    }

    public override void PostOpen(){
        if (Prefs.DevMode){
            Log.Message("[RimTales]: Showing Main Tab GUI.");
        }
        base.PostOpen();
    }

    //* Called when closing window.
    public override void PostClose(){
        bTabOpen=false;
        if (Prefs.DevMode){
            Log.Message("[RimTales]: Closing Main Tab GUI.");
        }
        base.PostClose();
    }

    //* Draw our Tabs on the Window.
    public override void DoWindowContents(Rect rect){

        //* Stop Tabs getting cut off?
        Rect val = rect;
		val.yMin = rect.yMin + 32f;

        //* Window Title
        Text.Font = GameFont.Medium;
        Rect val2 = rect;
        val2.x = val.x + 670;
        Widgets.Label(val2, "RimTales: The Story Exporter.");

        //* The Tabs
        Text.Font = GameFont.Small;
		TabDrawer.DrawTabs(val, tabs);
        
		switch (curTab)
			{
			case RimTab.Log:
				DoLogPage(val);
				break;

			case RimTab.Settings:
				DoSettingsPage(val);
				break;

			case RimTab.About:
				DoAboutPage(val);
				break;
			}

    }

    //* Settings Display Tab
    private void DoSettingsPage(Rect fillRect){
        
        Widgets.DrawBoxSolidWithOutline(fillRect, Widgets.MenuSectionBGFillColor, new Color(0.5f, 0.5f, 0.5f, 1f), 1);
        Rect position = fillRect.ContractedBy(5f);
        GUI.BeginGroup(position);
        GUI.color = Color.white;
        Text.Font = GameFont.Medium;

        //* Filter Label
        Rect positionLabel1 = new Rect(position.x + 150, 55f, 200F, 30F);
        Widgets.Label(positionLabel1, "List Filter Settings:");
        Widgets.DrawLineHorizontal(positionLabel1.x, 80f, 200);
        Text.Font = GameFont.Small;

        //* Filter Settings
        Rect position2 = new Rect(positionLabel1.x, 90f, 160F, 30F);
        Rect position3 = new Rect(positionLabel1.x, 130f, 160F, 30F);
        Rect position4 = new Rect(positionLabel1.x, 170f, 160F, 30F);
        Rect position5 = new Rect(positionLabel1.x, 205f, 160F, 30F);
        Rect position6 = new Rect(positionLabel1.x, 245f, 160F, 30F);
        Rect position7 = new Rect(positionLabel1.x, 275f, 160F, 30F);

        //* Aniversary Label
        Rect positionLabel2 = new Rect(positionLabel1.x + 350, 55f, 220F, 30F);
        Text.Font = GameFont.Medium;
        Widgets.Label(positionLabel2, "Anniversary Settings:");
        Widgets.DrawLineHorizontal(positionLabel2.x, 80f, 200);
        Text.Font = GameFont.Small;

        //* Anniversary Settings
        Rect position8 = new Rect(positionLabel2.x, 90f, 160F, 30F);
        Rect position9 = new Rect(positionLabel2.x, 130f, 160F, 30F);
        Rect position10 = new Rect(positionLabel2.x, 170f, 160F, 30F);
        Rect position11 = new Rect(positionLabel2.x, 205f, 160F, 30F);
        Rect position12 = new Rect(positionLabel2.x, 245f, 160F, 30F);

        //* Filter settings
        Widgets.CheckboxLabeled(position2, "EnShowDeaths".Translate(), ref RimTalesMod.settings.bShowDeaths);
        Widgets.CheckboxLabeled(position3, "EnShowVommit".Translate(), ref RimTalesMod.settings.bShowVommit);
        Widgets.CheckboxLabeled(position4, "EnShowWounded".Translate(), ref RimTalesMod.settings.bShowWounded);
        Widgets.CheckboxLabeled(position5, "EnShowAnimalTales".Translate(), ref RimTalesMod.settings.bShowAnimalTales);
        Widgets.CheckboxLabeled(position6, "EnShowChitChat".Translate(), ref RimTalesMod.settings.bShowChitChat);
        Widgets.CheckboxLabeled(position7, "EnShowPlayedGame".Translate(), ref RimTalesMod.settings.bShowPlayedGame);
 
        //* Aniversary Settings
        Widgets.CheckboxLabeled(position8, "enableMarriageAnniversary".Translate(), ref RimTalesMod.settings.enableMarriageAnniversary);
        Widgets.CheckboxLabeled(position9, "enableMemoryDay".Translate(), ref RimTalesMod.settings.enableMemoryDay);
        Widgets.CheckboxLabeled(position10, "enableFunerals".Translate(), ref RimTalesMod.settings.enableFunerals);
        Widgets.CheckboxLabeled(position11, "enableDaysOfVictory".Translate(), ref RimTalesMod.settings.enableDaysOfVictory);
        Widgets.CheckboxLabeled(position12, "enableIndividualThoughts".Translate(), ref RimTalesMod.settings.enableIndividualThoughts);
        
        GUI.EndGroup();
        RimTalesMod.settings.Write();
    }

    //* About RimTales Tab
    private void DoAboutPage(Rect fillRect){
        
        Widgets.DrawBoxSolidWithOutline(fillRect, Widgets.MenuSectionBGFillColor, new Color(0.5f, 0.5f, 0.5f, 1f), 1);
        Rect position = fillRect.ContractedBy(5f);
        GUI.BeginGroup(position);
        GUI.color = Color.white;
        Text.Font = GameFont.Small;
        Widgets.Label(position, "RimTales by TheCosmicSlug");
        // TODO: Add credits & thanks to the About tab.
        GUI.EndGroup();
    }

    //* Main Log Display Tab
    private void DoLogPage(Rect fillRect){

        //* Setup Display
        Widgets.DrawBoxSolidWithOutline(fillRect, Widgets.MenuSectionBGFillColor, new Color(0.5f, 0.5f, 0.5f, 1f), 1);
        Rect position = fillRect.ContractedBy(5f);
        GUI.BeginGroup(position);
        GUI.color = Color.white;
        Text.Font = GameFont.Small;
        
        //* Filter label
        Rect position2 = new Rect(10f, 10f, 110F, 30F);
        Widgets.Label(position2, "EnFilter".Translate());

        //* Filter textbox
        Rect position3 = new Rect(120f, 5f, 220F, 30F);
        StrFilter = Widgets.TextField(position3, StrFilter);

        //* Colour list toggle
        Rect position4 = new Rect(350, 5f, 100F, 30F);
        Widgets.CheckboxLabeled(position4, "Color List", ref RimTalesMod.settings.bUseColour);

        //* Tale Counter
        Rect position5 = new Rect(500, 10f, 250F, 30F);
        Widgets.Label(position5, "Showing " + tales.Count + "/" +  Resources.TaleManager.Count + " Tales.");

        //* Save button
        Rect position6 = new Rect(720f, 5f , 100f, 30f);
        if(Widgets.ButtonText(position6, "EnSaveList".Translate())){
            saveTales();
        }

        //* Save ALL button
        Rect position7 = new Rect(830f, 5f , 120f, 30f);
        if(Widgets.ButtonText(position7, "EnSaveAllList".Translate())){
            SaveAllTales();
        }

        //* The Log
        Rect outRect = new Rect(0f, 40f, position.width, position.height - 45f); // - 50f);
        Rect rect = new Rect(0f, 0f, position.width - 16f, this.scrollViewHeight);
        Widgets.BeginScrollView(outRect, ref this.scrollPosition, rect);

        //* Check if filter settings changed
        if (bTabShowVommit != RimTalesMod.settings.bShowVommit || bTabShowDeaths != RimTalesMod.settings.bShowDeaths || bTabShowWounded != RimTalesMod.settings.bShowWounded || bTabShowAnimalTales != RimTalesMod.settings.bShowAnimalTales || bTabShowChitChat != RimTalesMod.settings.bShowChitChat || bTabShowPlayedGame != RimTalesMod.settings.bShowPlayedGame ){
            if (Prefs.DevMode){
                Log.Message("[RimTales]: Filters changed, Importing tales again.");
            }
            ImportStringTales();
        };

        //* Check we have some tales to show
        if (tales.Count == 0){
            Widgets.EndScrollView();
            GUI.EndGroup();
            return;
        }

        float num = 0f;
        foreach (String tale in tales)
        {
            //* Colour the list.
            GUI.color = Color.white;
            if (RimTalesMod.settings.bUseColour == true){
                // TODO: Add more Colouring to the list.
                if (tale.Contains("ecruit") || tale.Contains("nimal") || tale.Contains("unted")) GUI.color = Color.green;
                if (tale.Contains("Traded") || tale.Contains("Struck")) GUI.color = Color.gray;
                if (tale.Contains("risoner") || tale.Contains("aked") || tale.Contains("Gave up") || tale.Contains("Wounded")) GUI.color = Color.yellow;
                if (tale.Contains("arriage") || tale.Contains("Breakup") || tale.Contains("lover")) GUI.color = Color.magenta;
                if (tale.Contains("Death") || tale.Contains("Kidnap") || tale.Contains("Berserk") || tale.Contains("Kill") || tale.Contains("kill") || tale.Contains("Raid") || tale.Contains("Human") || tale.Contains("Downed")) GUI.color = Color.red;
                if (tale.Contains("Research") || tale.Contains("Landed") || tale.Contains("urgery")) GUI.color = Color.cyan;
            }
            
            Rect rect2 = new Rect(5f, num, rect.width, 30f);
            if (Mouse.IsOver(rect2)){
                GUI.DrawTexture(rect2, TexUI.HighlightTex);
            }
            Widgets.Label(rect2, tale);
            num += 30f;
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
            if (CheckStringFilters(taleTMP.ShortSummary) == true){
                tales.Reverse();
                tales.Add(taleTMP.ShortSummary);
                tales.Reverse();
            }
        }
    }

    //* Export Filtered Tales to disk
    private void saveTales(){
        
        String outputFile;
        String outputMsg;
        outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "rimworld_tales.txt");
        using (var output = new StreamWriter(outputFile, false)){
        
            foreach (TaleStorage TaleTMP in Resources.TaleManager)
            {
                if(CheckTaleFilters(TaleTMP) == true){
                    if (CheckStringFilters(TaleTMP.ShortSummary) == true){
                        output.WriteLine(TaleTMP.ShortSummary);
                    }
                }
            }
        }
        
        Log.Message("[RimTales]: Filtered Tales exported to " + outputFile);
        outputMsg = "Tales saved to " + outputFile + System.Environment.NewLine + System.Environment.NewLine;
        Dialog_MessageBox window = new Dialog_MessageBox(outputMsg, "OK!");
		Find.WindowStack.Add(window);

    }

    //* Export ALL Tales to disk
    private void SaveAllTales(){

        String outputFile;
        String outputMsg;
        outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "rimworld_all_tales.txt");

        using (var output = new StreamWriter(outputFile, false)){
            foreach (TaleStorage taleTMP in Resources.TaleManager)
            {
                output.WriteLine(taleTMP.ShortSummary);
            }
        }

        if (Prefs.DevMode){
            Log.Message("[RimTales]: Filtered Tales exported to " + outputFile);
        }
        outputMsg = "ALL Tales saved to " + outputFile + System.Environment.NewLine + System.Environment.NewLine;
        Dialog_MessageBox window = new Dialog_MessageBox(outputMsg, "OK!");
		Find.WindowStack.Add(window);
    }
    
    //* Filter out unwanted tales based on type
    private static bool CheckTaleFilters(TaleStorage TaleTMP){
        // TODO: Add more Tale filters.
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
                return true;
        }
    }

    //* Filter out unwanted tales based on string
    private static bool CheckStringFilters(string tale){
        if(StrFilter == ""){
            return true;
        }else if (tale.IndexOf(StrFilter, StringComparison.OrdinalIgnoreCase) >= 0){
            return true;
        }
        return false;
    }

    //* Import Strings from the tale-list
    private void ImportStringTales(){

        //* store filter options so we can check for changes.
        bTabShowVommit = RimTalesMod.settings.bShowVommit;
        bTabShowDeaths = RimTalesMod.settings.bShowDeaths;
        bTabShowWounded = RimTalesMod.settings.bShowWounded;
        bTabShowAnimalTales = RimTalesMod.settings.bShowAnimalTales;
        bTabShowChitChat = RimTalesMod.settings.bShowChitChat;
        bTabShowPlayedGame = RimTalesMod.settings.bShowPlayedGame;

        tales.Clear();
        int TaleCount = 0;
        if (Resources.TaleManager.Count == 0){
            return;
        }

        for (int i = Resources.TaleManager.Count-1; i >= 0; i--)
        {
            if(CheckTaleFilters(Resources.TaleManager[i])==true){
                if (CheckStringFilters(Resources.TaleManager[i].ShortSummary) == true){
                    tales.Add(Resources.TaleManager[i].ShortSummary);
                    TaleCount = TaleCount + 1;
                    // TODO: Make the amount of tales displayed customisable.
                    if (TaleCount == 50){
                        break;
                    }
                }
            }
        }

    }
}

}
