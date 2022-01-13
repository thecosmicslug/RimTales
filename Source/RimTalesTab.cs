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

    private static String StrFilter = "";
    private int iFilterCount = 0;

    //* Called when opening window, Set it all up.
    public override void PreOpen(){

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
        Log.Message("[RimTales]: Showing Main Tab GUI.");
        curTab = RimTab.Log;
        base.PostOpen();
    }

    //* Called when closing window.
    public override void PostClose(){
        bTabOpen=false;
        Log.Message("[RimTales]: Closing Main Tab GUI.");
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
        Widgets.Label(val2, "RimTales: " + "RT_TheStoryExporter".Translate());

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
        Rect positionLabel1 = new Rect(position.x + 100, 55f, 200F, 30F);
        Widgets.Label(positionLabel1, "RT_FilterSettings".Translate());
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
        Rect positionLabel2 = new Rect(positionLabel1.x + 300, 55f, 220F, 30F);
        Text.Font = GameFont.Medium;
        
        Widgets.Label(positionLabel2, "RT_AnniversarySettings".Translate());
        Widgets.DrawLineHorizontal(positionLabel2.x, 80f, 200);
        Text.Font = GameFont.Small;

        //* Anniversary Settings
        Rect position8 = new Rect(positionLabel2.x, 90f, 160F, 30F);
        Rect position9 = new Rect(positionLabel2.x, 130f, 160F, 30F);
        Rect position10 = new Rect(positionLabel2.x, 170f, 160F, 30F);
        Rect position11 = new Rect(positionLabel2.x, 205f, 160F, 30F);
        Rect position12 = new Rect(positionLabel2.x, 245f, 160F, 30F);

        //* Filter settings
        Widgets.CheckboxLabeled(position2, "RT_ShowDeaths".Translate(), ref RimTalesMod.settings.bShowDeaths);
        Widgets.CheckboxLabeled(position3, "RT_ShowVommit".Translate(), ref RimTalesMod.settings.bShowVommit);
        Widgets.CheckboxLabeled(position4, "RT_ShowWounded".Translate(), ref RimTalesMod.settings.bShowWounded);
        Widgets.CheckboxLabeled(position5, "RT_ShowAnimalTales".Translate(), ref RimTalesMod.settings.bShowAnimalTales);
        Widgets.CheckboxLabeled(position6, "RT_ShowChitChat".Translate(), ref RimTalesMod.settings.bShowChitChat);
        Widgets.CheckboxLabeled(position7, "RT_ShowPlayedGame".Translate(), ref RimTalesMod.settings.bShowPlayedGame);

        //* Aniversary Settings
        Widgets.CheckboxLabeled(position8, "RT_EnableMarriageAnniversary".Translate(), ref RimTalesMod.settings.enableMarriageAnniversary);
        Widgets.CheckboxLabeled(position9, "RT_EnableMemoryDay".Translate(), ref RimTalesMod.settings.enableMemoryDay);
        Widgets.CheckboxLabeled(position10, "RT_EnableFunerals".Translate(), ref RimTalesMod.settings.enableFunerals);
        Widgets.CheckboxLabeled(position11, "RT_EnableDaysOfVictory".Translate(), ref RimTalesMod.settings.enableDaysOfVictory);
        Widgets.CheckboxLabeled(position12, "RT_EnableIndividualThoughts".Translate(), ref RimTalesMod.settings.enableIndividualThoughts);

        //* Filter label
        Rect position13 = new Rect(positionLabel1.x, 310f, 100F, 30F);
        Widgets.Label(position13, "RT_FilterCount".Translate());

        //* tale count textbox
        Rect position14 = new Rect(positionLabel1.x + 100, 305f, 80F, 30F);

        //* Check its numbers only or we get error!
        try{
            iFilterCount = Convert.ToInt32(Widgets.TextField(position14, iFilterCount.ToString()));
        }catch (FormatException){
            iFilterCount = Convert.ToInt32(Widgets.TextField(position14, RimTalesMod.settings.iFilterCount.ToString()));
        }catch (OverflowException){
            iFilterCount = Convert.ToInt32(Widgets.TextField(position14, RimTalesMod.settings.iFilterCount.ToString()));
        }

        //* Print Anniversary events to debug log.
        Rect position15 = new Rect(positionLabel2.x, 305f, 160F, 30F);
        if(Widgets.ButtonText(position15, "RT_DebugPrintEvents".Translate())){
            Core.PrintEventLog();
        }

        //* Wipe Anniversary EventLog.
        Rect position16 = new Rect(positionLabel2.x, 340f, 160F, 30F);
        if(Widgets.ButtonText(position16, "RT_DebugWipeEvents".Translate())){
            Core.WipeEventLog();
        }

        //* Print Anniversary events to debug log.
        Rect position17 = new Rect(positionLabel2.x, 380f, 160F, 30F);
        Widgets.CheckboxLabeled(position17, "RT_VerboseLogging".Translate(), ref RimTalesMod.settings.bVerboseLogging);

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

        Rect positionLabel1 = new Rect(position.x + 150, 55f, 200F, 30F);
        Widgets.Label(position, "RimTales by TheCosmicSlug");
        Widgets.DrawLineHorizontal(positionLabel1.x, 80f, 200);
        Text.Font = GameFont.Small;

        //* Filter Settings
        Rect position2 = new Rect(position.x, 90f, 1000F, 30F);
        Widgets.Label(position2, "This mod was Inspired by two abandoned mods, 'Tales Log [Retold]' 'RimStory' and  which I felt were both lacking features but good in their own way.");

        Rect position3 = new Rect(position.x, 130f, 1000F, 30F);
        Widgets.Label(position3, "So I started to combine the two, removing the UI from RimStory and integrating it with 'Tales Log [Retold]'");

        Rect position4 = new Rect(position.x, 170f, 1000F, 30F);
        Widgets.Label(position4, "RimTales takes the best bits from both mods, condenses it into one interface while massivly expanding the Events & Incidents it records.");

        Rect position5 = new Rect(position.x, 205f, 1000F, 30F);
        Widgets.Label(position5, "The Internals of this project have changed drastically since I started by tweaking 'Tales Log [Retold]'");

        Rect position6 = new Rect(position.x, 245f, 1000F, 30F);
        Widgets.Label(position6, "This version no longer relies on the TaleManager to retrieve the events, Rimworld deletes them after sometime in-game and that bugged me!");

        Rect position7 = new Rect(position.x, 275f, 1000F, 30F);
        Widgets.Label(position7, "This version uses a Harmony hook to catch all tales and log them permamently, the same with the incidents.");

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
        Widgets.Label(position2, "RT_Filter".Translate());

        //* Filter textbox
        Rect position3 = new Rect(120f, 5f, 220F, 30F);
        string StrFilter2 = StrFilter;

        StrFilter2 = Widgets.TextField(position3, StrFilter);
        if (StrFilter2 != StrFilter){
            StrFilter = StrFilter2;
            ImportStringTales();
        }

        //* Colour list toggle
        Rect position4 = new Rect(350, 5f, 100F, 30F);
        
        Widgets.CheckboxLabeled(position4, "RT_UseColour".Translate(), ref RimTalesMod.settings.bUseColour);

        //* Tale Counter
        Rect position5 = new Rect(500, 10f, 250F, 30F);
        Widgets.Label(position5, "RT_TaleCount".Translate(tales.Count,Resources.TaleManager.Count));

        //* Save button
        Rect position6 = new Rect(720f, 5f , 100f, 30f);
        if(Widgets.ButtonText(position6, "RT_SaveList".Translate())){
            saveTales();
        }

        //* Save ALL button
        Rect position7 = new Rect(830f, 5f , 120f, 30f);
        if(Widgets.ButtonText(position7, "RT_SaveAllList".Translate())){
            SaveAllTales();
        }

        //* The Log
        Rect outRect = new Rect(0f, 40f, position.width, position.height - 45f); // - 50f);
        Rect rect = new Rect(0f, 0f, position.width - 16f, this.scrollViewHeight);
        Widgets.BeginScrollView(outRect, ref this.scrollPosition, rect);

        //* Check if filter settings changed
        if (bTabShowVommit != RimTalesMod.settings.bShowVommit || bTabShowDeaths != RimTalesMod.settings.bShowDeaths || bTabShowWounded != RimTalesMod.settings.bShowWounded || bTabShowAnimalTales != RimTalesMod.settings.bShowAnimalTales || bTabShowChitChat != RimTalesMod.settings.bShowChitChat || bTabShowPlayedGame != RimTalesMod.settings.bShowPlayedGame || iFilterCount != RimTalesMod.settings.iFilterCount){
            Log.Message("[RimTales]: Filters changed, Importing tales again.");
            RimTalesMod.settings.iFilterCount = iFilterCount;
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
                tales.Add(taleTMP.date + ": " + taleTMP.ShortSummary);
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
                        output.WriteLine(TaleTMP.date + ": " + TaleTMP.ShortSummary);
                    }
                }
            }
        }
        Log.Message("[RimTales]: Filtered Tales exported to " + outputFile);
        outputMsg = "RT_SaveTales".Translate(outputFile) + System.Environment.NewLine + System.Environment.NewLine;
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
                output.WriteLine(taleTMP.date + ": " + taleTMP.ShortSummary);
            }
        }
        Log.Message("[RimTales]: ALL Tales exported to " + outputFile);
        outputMsg = "RT_SaveAllTales".Translate(outputFile) + System.Environment.NewLine + System.Environment.NewLine;
        Dialog_MessageBox window = new Dialog_MessageBox(outputMsg, "OK!");
		Find.WindowStack.Add(window);
    }
    
    //* Filter out unwanted tales based on type
    private static bool CheckTaleFilters(TaleStorage TaleTMP){
        // TODO: Add more Tale filters.
        switch (TaleTMP.def)
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
        iFilterCount = RimTalesMod.settings.iFilterCount;

        tales.Clear();
        int TaleCount = 0;
        if (Resources.TaleManager.Count == 0){
            return;
        }

        string strTemp ="";
        for (int i = Resources.TaleManager.Count-1; i >= 0; i--)
        {
            if(CheckTaleFilters(Resources.TaleManager[i])==true){
                if (CheckStringFilters(Resources.TaleManager[i].ShortSummary) == true){
                    strTemp = Resources.TaleManager[i].date + ": " + Resources.TaleManager[i].ShortSummary;
                    tales.Add(strTemp);
                    strTemp="";
                    TaleCount = TaleCount + 1;
                    if (iFilterCount != 0){
                        if (TaleCount == iFilterCount){
                            break;
                        }
                    }
                }
            }
        }

    }
}

}
