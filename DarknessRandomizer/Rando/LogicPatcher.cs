using DarknessRandomizer.Data;
using DarknessRandomizer.Lib;
using RandomizerCore.Logic;
using RandomizerCore.StringLogic;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

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
                lmb, name, new() { SceneName.CrossroadsFalseKnightArena, SceneName.DreamFailedChampion }, true) },
            { "Boss_Essence-Grey_Prince_Zote",
                (lmb, name, lc) => EditLogicClauseByScenes(lmb, name, new() { SceneName.Bretta, SceneName.DreamGreyPrinceZote }, true) },
            { "Boss_Essence-Lost_Kin", (lmb, name, lc) => EditLogicClauseByScenes(
                lmb, name, new() { SceneName.BasinBrokenVesselGrub, SceneName.DreamLostKin }, true) },
            { "Boss_Essence-Soul_Tyrant", (lmb, name, lc) => EditLogicClauseByScenes(
                lmb, name, new() { SceneName.CitySoulMasterArena, SceneName.DreamSoulTyrant }, true) },
            { "Boss_Essence-White_Defender", (lmb, name, lc) => EditLogicClauseByScenes(
                lmb, name, new() { SceneName.WaterwaysDungDefendersCave, SceneName.DreamWhiteDefender }, true) },

            // Modify mantis lords specifically because the rewards are in another room.
            { "Defeated_Mantis_Lords", (lmb, name, lc) => lmb.DoLogicEdit(
                new(name, $"ORIG + ($DarknessNotDarkened[{SceneName.FungalMantisLords}] | LANTERN | DARKROOMS + PROFICIENTCOMBAT)"))},

            // Flower quest just requires lantern, not gonna think any harder about it.
            { "Mask_Shard-Grey_Mourner", (lmb, name, lc) => lmb.DoLogicEdit(new(name, "ORIG + LANTERN")) },
        };

        private static readonly Dictionary<SceneName, LogicOverride> LogicOverridesByTransitionScene = new()
        {
            // TODO: Fix for bench rando
            { SceneName.GreenpathToll, EditLogicClauseByScenesNoDarkroomSkips },

            // The following scenes are trivial to navigate while dark, but may contain a check which is
            // uniquely affected by darkness.
            { SceneName.FungalQueensStation, (lmb, name, lc) => { } },
            { SceneName.GroundsXero, (lmb, name, lc) => { } },
        };

        private static readonly Dictionary<SceneName, LogicOverride> LogicOverridesByUniqueScene = new()
        {
            // Checks in these rooms are easy to obtain if the player has isma's tear; there is no danger.
            { SceneName.GreenpathLakeOfUnn, SafeDarkRoomWithIsmasTear(SceneName.GreenpathLakeOfUnn) },
            { SceneName.GreenpathUnn, SafeDarkRoomWithIsmasTear(SceneName.GreenpathUnn) }
        };

        private delegate bool LogicOverrideMatcher(LogicManagerBuilder lmb, string name, LogicClause lc);

        private static readonly List<LogicOverrideMatcher> LogicOverrideMatchers = new();

        public static void ModifyLMB(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!RandoInterop.IsEnabled()) return;

            var newResolver = new DarknessVariableResolver(lmb.VariableResolver);
            lmb.VariableResolver = newResolver;

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
            if (i != -1 && SceneName.TryGetSceneName(name.Substring(0, i), out SceneName sceneName)
                && LogicOverridesByTransitionScene.TryGetValue(sceneName, out handler))
            {
                handler.Invoke(lmb, name, lc);
                return;
            }

            foreach (var matcher in LogicOverrideMatchers)
            {
                if (matcher.Invoke(lmb, name, lc))
                {
                    return;
                }
            }

            // No special case applies, so we use the default scene inference logic.
            var scenes = InferScenes(lc);
            if (scenes.Count == 1 && LogicOverridesByUniqueScene.TryGetValue(scenes.Single(), out handler))
            {
                handler.Invoke(lmb, name, lc);
                return;
            }

            EditLogicClauseByScenes(lmb, name, InferScenes(lc), true);
        }

        private static HashSet<SceneName> InferScenes(LogicClause lc)
        {
            HashSet<SceneName> sceneNames = new();
            foreach (var token in lc.Tokens)
            {
                if (token is SimpleToken st)
                {
                    string name = st.Write();
                    int i = name.IndexOf("[");
                    if (i != -1)
                    {
                        if (SceneName.TryGetSceneName(name.Substring(0, i), out SceneName sceneName))
                        {
                            sceneNames.Add(sceneName);
                        }
                    }
                }
            }
            return sceneNames;
        }

        private static string GetDarkLogic(IEnumerable<SceneName> scenes, string locName, bool darkrooms)
        {
            var g = Graph.Instance;

            // Hack: check all scenes for difficulty or combat, and take the highest.
            bool difficult = false;
            bool combat = false;
            foreach (var scene in scenes)
            {
                if (g.TryGetSceneData(scene, out Lib.SceneData sData))
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

        private static void EditLogicClauseByScenes(LogicManagerBuilder lmb, string name, HashSet<SceneName> scenes, bool darkrooms)
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

        private static LogicOverride SafeDarkRoomWithIsmasTear(SceneName sceneName)
        {
            return (lmb, name, lc) => lmb.DoLogicEdit(new(name, $"ORIG + ($DarknessNotDarkened[{sceneName}] | LANTERN | ACID)"));
        }
    }
}
