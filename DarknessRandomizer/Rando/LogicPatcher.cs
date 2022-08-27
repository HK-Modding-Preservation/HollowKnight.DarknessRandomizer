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

        private delegate void LogicOverride(LogicManagerBuilder lmb, string name, LogicClause lc);

        private static readonly Dictionary<string, LogicOverride> LogicOverridesByName = new()
        {
            { "Boss_Essence-Elder_Hu", EditLogicClauseByScenesNoDarkroomSkips },
            { "Boss_Essence-Galien", EditLogicClauseByScenesNoDarkroomSkips },
            { "Boss_Essence-Gorb", EditLogicClauseByScenesNoDarkroomSkips },
            { "Boss_Essence-Markoth", EditLogicClauseByScenesNoDarkroomSkips },
            { "Boss_Essence-Marmu", EditLogicClauseByScenesNoDarkroomSkips },
            { "Boss_Essence-No_Eyes", EditLogicClauseByScenesNoDarkroomSkips },
            { "Boss_Essence-Xero", EditLogicClauseByScenesNoDarkroomSkips },

            // Dream bosses are coded specially because the checks are located where the dream nail is swung,
            // but we care whether or not the actual fight room is dark. So we have to account for both rooms.
            { "Boss_Essence-Failed_Champion", (lmb, name, lc) => EditLogicClauseByScenes(
                lmb, name, new() { Scenes.CrossroadsFalseKnightArena, Scenes.DreamFailedChampion }, true) },
            { "Boss_Essence-Grey_Prince_Zote",
                (lmb, name, lc) => EditLogicClauseByScenes(lmb, name, new() { Scenes.Bretta, Scenes.DreamGreyPrinceZote }, true) },
            { "Boss_Essence-Lost_Kin", (lmb, name, lc) => EditLogicClauseByScenes(
                lmb, name, new() { Scenes.BasinBrokenVesselGrub, Scenes.DreamLostKin }, true) },
            { "Boss_Essence-Soul_Tyrant", (lmb, name, lc) => EditLogicClauseByScenes(
                lmb, name, new() { Scenes.CitySoulMasterArena, Scenes.DreamSoulTyrant }, true) },
            { "Boss_Essence-White_Defender", (lmb, name, lc) => EditLogicClauseByScenes(
                lmb, name, new() { Scenes.WaterwaysDungDefendersCave, Scenes.DreamWhiteDefender }, true) },

            // Flower quest just requires lantern, not gonna think any harder about it.
            { "Mask_Shard-Grey_Mourner", (lmb, name, lc) => lmb.DoLogicEdit(new(name, "ORIG + LANTERN")) },
        };

        private static readonly Dictionary<string, LogicOverride> LogicOverridesByTransitionScene = new()
        {
            // TODO: Fix for bench rando
            { Scenes.GreenpathToll, EditLogicClauseByScenesNoDarkroomSkips },
        };

        private delegate bool LogicOverrideMatcher(LogicManagerBuilder lmb, string name, LogicClause lc);

        private static readonly List<LogicOverrideMatcher> LogOverrideMatchers = new();

        public static void ModifyLMB(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!RandoInterop.IsEnabled()) return;

            lmb.VariableResolver = new DarknessVariableResolver(lmb.VariableResolver);

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
                var name = e.Key;
                var lc = e.Value;
                edits.Add(() => EditLogicClause(lmb, name, lc));
            }
            edits.ForEach(e => e.Invoke());
        }

        private static void EditLogicClause(LogicManagerBuilder lmb, string name, LogicClause lc)
        {
            if (LogicOverridesByName.TryGetValue(name, out LogicOverride handler))
            {
                handler.Invoke(lmb, name, lc);
                return;
            }

            int i = name.IndexOf('[');
            if (i != -1)
            {
                var scene = name.Substring(0, i);
                if (Scenes.IsScene(scene) && LogicOverridesByTransitionScene.TryGetValue(scene, out handler))
                {
                    handler.Invoke(lmb, name, lc);
                    return;
                }
            }

            foreach (var matcher in LogOverrideMatchers)
            {
                if (matcher.Invoke(lmb, name, lc))
                {
                    return;
                }
            }

            // No special case applies, so we use the default scene inference logic.
            EditLogicClauseByScenes(lmb, name, InferScenes(lc), true);
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

        private static string GetDarkLogic(IEnumerable<string> scenes, string locName, bool darkrooms)
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

            string dark = darkrooms ? "DARKROOMS" : "ANY";
            return difficult ?
                (combat ? $"{dark} + SPICYCOMBATSKIPS" : $"{dark} + DIFFICULTSKIPS") :
                (combat ? $"{dark} + PROFICIENTCOMBAT" : dark);
        }

        private static void EditLogicClauseByScenes(LogicManagerBuilder lmb, string name, HashSet<string> scenes, bool darkrooms)
        {
            if (scenes.Count == 0)
            {
                return;
            }

            // Relax the LANTERN constraint for scenes which have been brightened.
            lmb.DoSubst(new(name, "LANTERN", $"(LANTERN | $DarknessBrightened[{String.Join(",", scenes)}])"));

            // Add a darkness constraint for scenes which have been darkened.
            lmb.DoLogicEdit(new(
                name, $"ORIG + ($DarknessNotDarkened[{String.Join(",", scenes)}] | LANTERN | {GetDarkLogic(scenes, name, darkrooms)})"));
        }

        private static void EditLogicClauseByScenesNoDarkroomSkips(LogicManagerBuilder lmb, string name, LogicClause lc)
        {
            EditLogicClauseByScenes(lmb, name, InferScenes(lc), false);
        }
    }
}
