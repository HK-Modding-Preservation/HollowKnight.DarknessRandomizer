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
            { "Defeated_Elder_Hu", EditLogicClauseBySceneInferenceNoDarkrooms },
            { "Defeated_Galien", EditLogicClauseBySceneInferenceNoDarkrooms },
            { "Defeated_Gorb", EditLogicClauseBySceneInferenceNoDarkrooms },
            { "Defeated_Markoth", EditLogicClauseBySceneInferenceNoDarkrooms },
            { "Defeated_Marmu", EditLogicClauseBySceneInferenceNoDarkrooms },
            { "Defeated_No_Eyes", EditLogicClauseBySceneInferenceNoDarkrooms },
            { "Defeated_Xero", EditLogicClauseBySceneInferenceNoDarkrooms },

            // Dream bosses are coded specially because the checks are located where the dream nail is swung,
            // but we care whether or not the actual fight room is dark. So we have to account for both rooms.
            { "Defeated_Failed_Champion", CustomSceneLogicEdit(SceneName.DreamFailedChampion, "SPICYCOMBATSKIPS") },
            { "Defeated_Grey_Prince_Zote", CustomSceneLogicEdit(SceneName.GPZ, "SPICYCOMBATSKIPS") },
            { "Defeated_Lost_Kin", CustomSceneLogicEdit(SceneName.DreamLostKin, "SPICYCOMBATSKIPS") },
            { "Defeated_Soul_Tyrant", CustomSceneLogicEdit(SceneName.DreamSoulTyrant, "SPICYCOMBATSKIPS") },
            { "Defeated_White_Defender", CustomSceneLogicEdit(SceneName.DreamWhiteDefender, "SPICYCOMBATSKIPS") },

            // Flower quest just requires lantern, not gonna think any harder about it.
            { "Mask_Shard-Grey_Mourner", CustomDarkLogicEdit("FALSE") },
        };

        private static readonly Dictionary<SceneName, LogicOverride> LogicOverridesByTransitionScene = new()
        {
            // TODO: Fix for bench rando
            { SceneName.GreenpathToll, EditLogicClauseBySceneInferenceNoDarkrooms },

            // The following scenes are trivial to navigate while dark, but may contain a check which is
            // uniquely affected by darkness.
            { SceneName.FungalQueensStation, (lmb, name, lc) => { } },
            { SceneName.GroundsXero, (lmb, name, lc) => { } },
        };

        private static readonly Dictionary<SceneName, LogicOverride> LogicOverridesByUniqueScene = new()
        {
            // Checks in these rooms are easy to obtain if the player has isma's tear; there is no danger.
            { SceneName.GreenpathLakeOfUnn, CustomDarkLogicEdit("ACID") },
            { SceneName.GreenpathUnn, CustomDarkLogicEdit("ACID") }
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

            if (SceneName.IsTransition(name, out SceneName sceneName)
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
            var si = InferScenes(lc);
            if (si.NumScenes == 1 && LogicOverridesByUniqueScene.TryGetValue(si.Single(), out handler))
            {
                handler.Invoke(lmb, name, lc);
                return;
            }

            EditLogicClauseBySceneInference(lmb, name, si, (ln, sn, si) => AddDarkLogic(ln, sn, true, si));
        }

        private class SceneInference
        {
            public bool HasLantern = false;
            public Dictionary<SceneName, HashSet<string>> TransitionTermsBySceneName = new();

            public int NumScenes => TransitionTermsBySceneName.Count;

            public IEnumerable<SceneName> Scenes => TransitionTermsBySceneName.Keys;

            public SceneName Single() => TransitionTermsBySceneName.Keys.Single();
        }

        private static SceneInference InferScenes(LogicClause lc)
        {
            SceneInference si = new();
            foreach (var token in lc.Tokens)
            {
                if (token is SimpleToken st)
                {
                    string name = st.Write();
                    if (SceneName.IsTransition(name, out SceneName sceneName))
                    {
                        si.TransitionTermsBySceneName.GetOrCreate(sceneName).Add(name);
                    }
                    else if (name == "LANTERN")
                    {
                        si.HasLantern = true;
                    }
                }
            }
            return si;
        }

        private static readonly SimpleToken DarkroomsToken = new("DARKROOMS");
        private static readonly SimpleToken DifficultSkipsToken = new("DIFFICULTSKIPS");
        private static readonly SimpleToken ProficientCombatToken = new("PROFICIENTCOMBAT");

        private static void AddDarkLogic(string logicName, SceneName scene, bool darkrooms, List<LogicToken> sink)
        {
            var g = Graph.Instance;

            // Hack: check all scenes for difficulty or combat, and take the highest.
            bool difficult = false;
            bool combat = false;
            if (g.TryGetSceneData(scene, out Lib.SceneData sData))
            {
                difficult = sData.DifficultSkipLocs.Contains(logicName);
                combat = sData.ProficientCombatLocs.Contains(logicName);
            }

            int tokens = (darkrooms ? 1 : 0) + (difficult ? 1 : 0) + (combat ? 1 : 0);
            if (darkrooms)
            {
                sink.Add(DarkroomsToken);
            }    
            if (difficult)
            {
                sink.Add(DifficultSkipsToken);
            }
            if (combat)
            {
                sink.Add(ProficientCombatToken);
            }

            if (tokens == 0)
            {
                sink.Add(ConstToken.True);
            }
            else
            {
                for (int i = 1; i < tokens; i++)
                {
                    sink.Add(OperatorToken.AND);
                }
            }
        }

        private static void EditLogicClauseBySceneInference(LogicManagerBuilder lmb, string name, SceneInference si, DarknessLogicAdder dla)
        {
            if (si.NumScenes == 0)
            {
                return;
            }

            LogicClauseEditor.EditDarkness(lmb, name, dla);
        }

        private static void EditLogicClauseBySceneInferenceNoDarkrooms(LogicManagerBuilder lmb, string name, LogicClause lc)
        {
            EditLogicClauseBySceneInference(lmb, name, InferScenes(lc), (ln, sn, si) => AddDarkLogic(ln, sn, false, si));
        }

        private static LogicOverride CustomDarkLogicEdit(string darkLogic)
        {
            LogicClause lc = new(darkLogic);
            return (lmb, name, lc) => EditLogicClauseBySceneInference(lmb, name, InferScenes(lc), (ln, sn, si) =>
            {
                foreach (var t in lc)
                {
                    si.Add(t);
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
