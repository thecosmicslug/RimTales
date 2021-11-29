using System;
using System.Collections.Generic;
using System.Text;
using HugsLib;
using UnityEngine.SceneManagement;
using RimWorld;
using Verse;

namespace RimTales
{

    public class Core : ModBase
    {

        public override string ModIdentifier {
            get { return "RimTales"; }
        }

        public override void SettingsChanged() {
			Log.Message("[RimTales]: SettingsChanged()");
		}

        public override void MapLoaded(Map map)
        {
            Logger.Message("Initialisng Tales...");
            Resources.TaleManager.Clear();
            foreach (Tale tale in Find.TaleManager.AllTalesListForReading){
                Resources.TaleManager.Add(tale);
            }
            Logger.Message("Done! Imported " + Resources.TaleManager.Count + " tales.");

            Resources.TEST_MAP = map;
            base.MapLoaded(map);

            //* One tick per hour
            HugsLibController.Instance.TickDelayScheduler.ScheduleCallback(() =>
            {
                if (Resources.EventManager.Count > 0)
                {
                    foreach (var e in Resources.EventManager)
                    {
                        if (e is AMarriage && RimTalesMod.settings.enableMarriageAnniversary)
                        {
                            e.TryStartEvent(map);
                        }

                        if (e is AMemorialDay && RimTalesMod.settings.enableMemoryDay)
                        {
                            e.TryStartEvent(map);
                        }

                        if (e is ABigThreat && RimTalesMod.settings.enableDaysOfVictory)
                        {
                            e.TryStartEvent(map);
                        }

                        if (e is ADead && RimTalesMod.settings.enableIndividualThoughts)
                        {
                            e.TryStartEvent(map);
                        }
                    }
                }

            }, 2500, null, true);

            //* Mass funeral
            HugsLibController.Instance.TickDelayScheduler.ScheduleCallback(() =>
            {
                if (Resources.deadPawnsForMassFuneralBuried.Count <= 0 || !RimTalesMod.settings.enableFunerals)
                {
                    return;
                }

                if (Resources.dateLastFuneral == null ||
                    Utils.CurrentDay() != Resources.dateLastFuneral.GetDate().day &&
                    Utils.CurrentQuadrum() != Resources.dateLastFuneral.GetDate().quadrum &&
                    Utils.CurrentYear() != Resources.dateLastFuneral.GetDate().year)
                {
                }

                if (!MassFuneral.TryStartMassFuneral(map))
                {
                    return;
                }

                Resources.deadPawnsForMassFuneralBuried.Clear();
                Resources.dateLastFuneral = Utils.CurrentDate();
            }, 2500, null, true);
        }

        public override void SceneLoaded(Scene scene)
        {
            //* Dirty hacks for deleting static lists. Don't look. 
            base.SceneLoaded(scene);
            if (!GenScene.InEntryScene)
            {
                return;
            }

            Resources.EventManager.Clear();
        }

        public override void WorldLoaded()
        {
            base.WorldLoaded();
            var unused = Find.World.GetComponent<Saves>();
        }
    }
}