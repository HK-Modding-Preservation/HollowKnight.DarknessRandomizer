using DarknessRandomizer.Data;
using DarknessRandomizer.Lib;
using RandomizerCore.Logic;
using RandomizerCore.StringLogic;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using System;
using System.Collections.Generic;

namespace DarknessRandomizer.Rando
{
    internal static class LogicPatcher
    {
        public static void Setup()
        {
            RCData.RuntimeLogicOverride.Subscribe(100.0f, ModifyLMB);
        }

        private delegate Action LogicOverride(LogicManagerBuilder lmb, string name, LogicClause lc);
        private delegate bool LogicOverrideMatcher(LogicManagerBuilder lmb, string name, LogicClause lc, out Action edit);

        private static readonly Dictionary<string, LogicOverride> LogicOverrides = new()
        {
            { "Fungus1_31[top1]", GreenpathTollBenchOverride },
            { "Fungus1_31[right1]", GreenpathTollBenchOverride },
            { "Fungus1_31[bot1]", GreenpathTollBenchOverride }
        };

        private static readonly List<LogicOverrideMatcher> LogicOverrideMatchers = new();

        public static void ModifyLMB(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!RandoInterop.IsEnabled()) return;

            RandoInterop.Initialize(gs.Seed);

            // We want to generically modify logic by location (SceneName), but unfortunately the LogicManager is constructed
            // before any of the location info is provided via the RequestBuilder, so we have to be creative.
            //
            // We'll inspect every logic clause for the set of scene transitions it references. If there is only one scene,
            // we will update the logic accordingly. If there are zero scenes, we ignore it, and if there are two or more, we
            // require custom handling.
            //
            // TODO: Special handling for BenchRando, TheRealJournalRando, possibly others?

            // We defer the edits to avoid messing with dictionary iteration order.
            List<Action> edits = new();
            foreach (var e in lmb.LogicLookup)
            {
                edits.Add(InferLogicEdit(lmb, e.Key, e.Value));
            }
            edits.ForEach(e => e.Invoke());
        }

        private static Action InferLogicEdit(LogicManagerBuilder lmb, string name, LogicClause lc)
        {
            if (LogicOverrides.TryGetValue(name, out LogicOverride handler))
            {
                return handler.Invoke(lmb, name, lc);
            }
            foreach (var matcher in LogicOverrideMatchers)
            {
                if (matcher.Invoke(lmb, name, lc, out Action action))
                {
                    return action;
                }
            }

            HashSet<string> scenes = InferScenes(lc);
            return () => EditSceneClause(lmb, name, scenes);
        }

        private static HashSet<string> InferScenes(LogicClause lc)
        {
            HashSet<string> scenes = new();
            foreach (var token in lc.Tokens)
            {
                if (token is SimpleToken st)
                {
                    string name = st.Write();
                    int i = name.IndexOf("[");
                    if (i != -1)
                    {
                        string maybeScene = name.Substring(0, i);
                        if (Scenes.IsScene(maybeScene))
                        {
                            scenes.Add(maybeScene);
                        }
                    }
                }
            }
            return scenes;
        }

        private static string GetDarkLogic(IEnumerable<string> scenes, string locName)
        {
            var g = Graph.Instance;

            // Hack: check all scenes for difficulty or combat, and take the highest.
            bool difficult = false;
            bool combat = false;
            foreach (var scene in scenes)
            {
                if (g.TryGetSceneData(scene, out Scene sData))
                {
                    difficult |= sData.DifficultSkipLocs.Contains(locName);
                    combat |= sData.ProficientCombatLocs.Contains(locName);
                }
            }

            return difficult ?
                (combat ? "SPICYCOMBATSKIPS" : "DIFFICULTSKIPS") :
                (combat ? "PROFICIENTCOMBAT" : "DARKROOMS");
        }

        private static readonly HashSet<string> VanillaDarkScenes = new()
        {
            Scenes.CliffsJonisDark,
            Scenes.CrossroadsPeakDarkToll,
            Scenes.CrystalDarkRoom,
            Scenes.DeepnestFirstDevout,
            Scenes.DeepnestMidwife,
            Scenes.DeepnestOutsideGalien,
            Scenes.DeepnestOutsideMaskMaker,
            Scenes.DeepnestWhisperingRoot,
            Scenes.GreenpathStoneSanctuary
        };

        private static void EditSceneClause(LogicManagerBuilder lmb, string name, HashSet<string> scenes)
        {
            if (scenes.Count == 0)
            {
                return;
            }

            bool anyDarkness = false;
            bool anyNewDarkness = false;
            bool anyBrightness = false;
            bool anyNewBrightness = false;
            foreach (var scene in scenes)
            {
                if (RandoInterop.LS.DarknessOverrides.TryGetValue(scene, out Darkness d))
                {
                    if (d == Darkness.Dark)
                    {
                        anyDarkness |= true;
                        anyNewDarkness |= !VanillaDarkScenes.Contains(scene);
                    }
                    else
                    {
                        anyBrightness |= true;
                        anyNewBrightness |= VanillaDarkScenes.Contains(scene);
                    }
                }
            }

            if (anyNewDarkness)
            {
                lmb.DoLogicEdit(new(name, $"ORIG + (LANTERN | {GetDarkLogic(scenes, name)})"));
            }
            else if (anyNewBrightness && !anyDarkness)
            {
                lmb.DoSubst(new(name, "LANTERN", "ANY"));
            }
        }

        // TODO: Fix with bench rando
        private static Action GreenpathTollBenchOverride(LogicManagerBuilder lmb, string name, LogicClause lc)
        {
            if (RandoInterop.LS.DarknessOverrides[Scenes.GreenpathToll] != Darkness.Dark)
            {
                return () => { };
            }

            // The bench toll can't be opened without lantern, so we need to remove 'DARKROOMS' from the logic override.
            return () => lmb.DoLogicEdit(new(name, "ORIG + LANTERN"));
        }
    }
}
