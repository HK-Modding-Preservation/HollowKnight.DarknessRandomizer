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
            // Dream warriors do not appear in dark rooms without lantern.
            { "Defeated_Elder_Hu", CustomDarkLogicEdit("FALSE") },
            { "Defeated_Galien", CustomDarkLogicEdit("FALSE") },
            { "Defeated_Gorb", CustomDarkLogicEdit("FALSE") },
            { "Defeated_Markoth", CustomDarkLogicEdit("FALSE") },
            { "Defeated_Marmu", CustomDarkLogicEdit("FALSE") },
            { "Defeated_No_Eyes", CustomDarkLogicEdit("FALSE") },
            { "Defeated_Xero", CustomDarkLogicEdit("FALSE") },

            // Dream bosses are coded specially because the checks are located where the dream nail is swung,
            // but we care whether or not the actual fight room is dark. So we have to account for both rooms.
            { "Defeated_Failed_Champion", CustomSceneLogicEdit(SceneName.DreamFailedChampion, "FALSE") },
            { "Defeated_Grey_Prince_Zote", CustomSceneLogicEdit(SceneName.DreamGreyPrinceZote, "FALSE") },
            { "Defeated_Lost_Kin", CustomSceneLogicEdit(SceneName.DreamLostKin, "DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },
            { "Defeated_Soul_Tyrant", CustomSceneLogicEdit(SceneName.DreamSoulTyrant, "DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },
            { "Defeated_White_Defender", CustomSceneLogicEdit(SceneName.DreamWhiteDefender, "DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },

            // Specific checks with difficult platforming.
            { "Void_Heart", CustomSceneLogicEdit(SceneName.AbyssBirthplace, "DARKROOMS + DIFFICULTSKIPS") },

            // These bosses are deemed difficult in the dark.
            { "Defeated_Any_Hollow_Knight", CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },
            { "Defeated_Any_Nightmare_King", CustomDarkLogicEdit("FALSE") },
            { "Defeated_Any_Radiance", CustomDarkLogicEdit("FALSE") },
            { "Defeated_Broken_Vessel", CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
            { "Defeated_Brooding_Mawlek", CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
            { "Defeated_Collector", CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },
            { "Defeated_Colloseum_1", CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },
            { "Defeated_Colloseum_2", CustomDarkLogicEdit("FALSE") },
            { "Defeated_Colloseum_3", CustomDarkLogicEdit("FALSE") },
            { "Defeated_Crystal_Guardian", CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
            { "Defeated_Enraged_Guardian", CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },
            { "Defeated_Flukemarm", CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
            { "Defeated_Hive_Knight", CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },
            { "Defeated_Hornet_Sentinel", CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
            { "Defeated_Grimm", CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
            { "Defeated_Mantis_Lords", CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
            { "Defeated_Nosk", CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
            { "Defeated_Pale_Lurker", CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
            { "Defeated_Soul_Master", CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
            { "Defeated_Traitor_Lord", CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },
            { "Defeated_Uumuu", CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
            { "Defeated_Watcher_Knights", CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },

            // Flower quest simply requires lantern.
            { "Mask_Shard-Grey_Mourner", (lmb, ln, lc) => lmb.DoLogicEdit(new(ln, "ORIG + LANTERN")) }
        };

        private static readonly Dictionary<SceneName, LogicOverride> LogicOverridesByTransitionScene = new()
        {
            // TODO: Fix for bench rando
            { SceneName.GreenpathToll, CustomDarkLogicEdit("FALSE") },

            // The following scenes are trivial to navigate while dark, but may contain a check which is
            // uniquely affected by darkness.
            { SceneName.FungalQueensStation, NoLogicEdit },
            { SceneName.GroundsBlueLake, NoLogicEdit },
            { SceneName.GroundsXero, NoLogicEdit },
        };

        private static readonly Dictionary<SceneName, LogicOverride> LogicOverridesByUniqueScene = new()
        {
            // Checks in these rooms are easy to obtain if the player has isma's tear; there is no danger.
            { SceneName.GreenpathLakeOfUnn, CustomDarkLogicEdit("ACID") },
            { SceneName.GreenpathUnn, CustomDarkLogicEdit("ACID") },

            // These scenes have difficult dark platforming.
            { SceneName.CrystalCrystalHeartGauntlet, CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS") },
            { SceneName.CrystalDeepFocusGauntlet, CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS") },
            { SceneName.EdgeWhisperingRoot, CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS") },
            { SceneName.GreenpathSheoGauntlet, CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS") }
        };

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

        private static readonly SimpleToken DarkroomsToken = new("DARKROOMS");

        private static void EditLogicClause(LogicManagerBuilder lmb, string name, LogicClause lc)
        {
            if (LogicOverridesByName.TryGetValue(name, out LogicOverride handler))
            {
                handler.Invoke(lmb, name, lc);
                return;
            }

            if (SceneName.IsTransition(name, out SceneName sceneName)
                && LogicOverridesByTransitionScene.TryGetValue(sceneName, out handler))
            {
                handler.Invoke(lmb, name, lc);
                return;
            }

            // Check for an inferred scene match.
            if (InferSingleScene(lc, out SceneName inferred) && LogicOverridesByUniqueScene.TryGetValue(inferred, out handler))
            {
                handler.Invoke(lmb, name, lc);
                return;
            }

            // No special matches, use the default editor.
            LogicClauseEditor.EditDarkness(lmb, name, (sink) => sink.Add(DarkroomsToken));
        }

        private static bool InferSingleScene(LogicClause lc, out SceneName sceneName)
        {
            HashSet<SceneName> ret = new();
            foreach (var token in lc)
            {
                if (token is SimpleToken st)
                {
                    string name = st.Write();
                    if (SceneName.IsTransition(name, out SceneName newName))
                    {
                        ret.Add(newName);
                    }
                }
            }

            if (ret.Count == 1)
            {
                sceneName = ret.Single();
                return true;
            }

            sceneName = default;
            return false;
        }

        private static void NoLogicEdit(LogicManagerBuilder lmb, string name, LogicClause lc) { }

        private static LogicOverride CustomDarkLogicEdit(string darkLogic)
        {
            return (lmb, name, lc) => LogicClauseEditor.EditDarkness(lmb, name, (sink) =>
            {
                LogicClause repl = new(darkLogic);
                foreach (var t in repl)
                {
                    sink.Add(t);
                }
            });
        }

        private static LogicOverride CustomSceneLogicEdit(SceneName sceneName, string darkLogic)
        {
            return (lmb, name, lc) => lmb.DoLogicEdit(
                new(name, $"ORIG + ($DarknessLevel[{sceneName}]<2 | LANTERN | {darkLogic})"));
        }
    }
}
