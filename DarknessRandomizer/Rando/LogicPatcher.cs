using DarknessRandomizer.Data;
using DarknessRandomizer.Lib;
using Modding;
using RandomizerCore;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerCore.LogicItems.Templates;
using RandomizerCore.StringLogic;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DarknessRandomizer.Rando
{
    internal class LogicOverrides
    {
        private delegate void LogicOverride(LogicManagerBuilder lmb, string logicName, LogicClause lc);

        private readonly Dictionary<string, LogicOverride> logicOverridesByName;
        private readonly Dictionary<SceneName, LogicOverride> logicOverridesByTransitionScene;
        private readonly Dictionary<SceneName, LogicOverride> logicOverridesByUniqueScene;

        public SimpleToken LanternToken { get; }

        public LogicOverrides()
        {
            LanternToken = new SimpleToken(RandoInterop.LanternTermName);

            logicOverridesByName = new()
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
                { "Void_Heart", CustomSceneLogicEdit(SceneName.DreamAbyss, "DARKROOMS + DIFFICULTSKIPS") },

                // QG stag checks are free except for these two.
                { "Soul_Totem-Below_Marmu", CustomDarkLogicEdit("DARKROOMS") },
                { $"{SceneName.GardensGardensStag}[top1]", CustomDarkLogicEdit("DARKROOMS") },

                // These bosses are deemed difficult in the dark.
                { "Defeated_Any_Hollow_Knight", CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },
                { "Defeated_Any_Nightmare_King", CustomDarkLogicEdit("FALSE") },
                { "Defeated_Any_Radiance", CustomDarkLogicEdit("FALSE") },
                { "Defeated_Broken_Vessel", CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
                { "Defeated_Brooding_Mawlek", CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
                { "Defeated_Collector", CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },
                { "Defeated_Colosseum_1", CustomSceneLogicEdit(SceneName.EdgeColo1Arena, "DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },
                { "Defeated_Colosseum_2", CustomSceneLogicEdit(SceneName.EdgeColo2Arena, "FALSE") },
                { "Defeated_Colosseum_3", CustomSceneLogicEdit(SceneName.EdgeColo3Arena, "FALSE") },
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

                // Using Elegant Key requires lantern.
                { $"{SceneName.CityTollBench.Name()}[left3]", CustomDarkLogicEdit("FALSE") },

                // Flower quest simply requires lantern.
                { "Mask_Shard-Grey_Mourner", (lmb, ln, lc) => lmb.DoLogicEdit(new(ln, $"ORIG + {LanternToken.Write()}")) }
            };

            logicOverridesByTransitionScene = new()
            {
                // TODO: Fix for bench rando
                { SceneName.GreenpathToll, CustomDarkLogicEdit("FALSE") },

                // The following scenes are trivial to navigate while dark, but may contain a check which is
                // uniquely affected by darkness.
                { SceneName.FungalQueensStation, NoLogicEdit },
                { SceneName.GroundsBlueLake, NoLogicEdit },
                { SceneName.GroundsXero, NoLogicEdit },
            };

            logicOverridesByUniqueScene = new()
            {
                // Checks in these rooms are easy to obtain if the player has isma's tear; there is no danger.
                { SceneName.GreenpathLakeOfUnn, CustomDarkLogicEdit("ACID") },
                { SceneName.GreenpathUnn, CustomDarkLogicEdit("ACID") },

                // Checks in these scenes are free, even if dark.
                { SceneName.BasinCorridortoBrokenVessel, CustomDarkLogicEdit("ANY") },
                { SceneName.CityTollBench, CustomDarkLogicEdit("ANY") },
                // Gardens checks by the stag are free; marmu, marmu totem, and the upper transition are exceptions.
                { SceneName.GardensGardensStag, CustomDarkLogicEdit("ANY") },

                // These scenes have difficult dark platforming.
                { SceneName.CrystalCrystalHeartGauntlet, CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS") },
                { SceneName.CrystalDeepFocusGauntlet, CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS") },
                { SceneName.EdgeWhisperingRoot, CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS") },
                { SceneName.GreenpathSheoGauntlet, CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS") }
            };

            if (ModHooks.GetMod("BenchRando") is Mod)
            {
                DoBenchRandoInterop();
            }
        }

        private readonly Dictionary<string, SceneName> customSceneInferences = new()
        {
            { "Queen's_Gardens_Stag", SceneName.GardensGardensStag }
        };
        private readonly Dictionary<string, SceneNameInferrer> sceneNameInferrerOverrides = new();

        private void DoBenchRandoInterop()
        {
            foreach (var e in BenchRando.BRData.BenchLookup)
            {
                var benchName = e.Key;
                var def = e.Value;
                if (!SceneName.TryGetValue(def.SceneName, out SceneName sceneName)) continue;

                // Make sure we apply darkness logic to other checks in the room obtainable from the bench.
                customSceneInferences[benchName] = sceneName;

                // Bench checks are obtainable even in dark rooms, if the player has the benchwarp pickup.
                sceneNameInferrerOverrides[benchName] = (string term, out SceneName sceneName) => InferSceneName(term, out sceneName) && term != benchName;
            }
        }

        public SceneNameInferrer GetSceneNameInferrer(string logicName) => sceneNameInferrerOverrides.TryGetValue(logicName, out SceneNameInferrer sni) ? sni : InferSceneName;

        // Don't infer scene names for rooms which cannot be dark.
        private static bool SceneCanBeDark(SceneName sceneName) => Data.SceneData.Get(sceneName).MaximumDarkness >= Darkness.Dark && ClusterData.Get(sceneName).MaximumDarkness(DarknessRandomizer.GS.DarknessRandomizationSettings) >= Darkness.Dark;

        private bool InferSceneName(string term, out SceneName sceneName)
        {
            if (SceneName.IsTransition(term, out sceneName))
            {
                return SceneCanBeDark(sceneName);
            }

            if (customSceneInferences.TryGetValue(term, out sceneName))
            {
                return SceneCanBeDark(sceneName);
            }

            sceneName = default;
            return false;
        }

        private bool InferSingleSceneName(LogicClause lc, out SceneName sceneName)
        {
            HashSet<SceneName> ret = new();
            foreach (var token in lc)
            {
                if (token is SimpleToken st)
                {
                    string name = st.Write();
                    if (InferSceneName(name, out SceneName newName))
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

        private static readonly SimpleToken DarkroomsToken = new("DARKROOMS");

        public void EditLogicClause(LogicManagerBuilder lmb, string logicName, LogicClause lc)
        {
            if (logicOverridesByName.TryGetValue(logicName, out LogicOverride handler))
            {
                handler.Invoke(lmb, logicName, lc);
                return;
            }

            if (InferSceneName(logicName, out SceneName sceneName)
                && logicOverridesByTransitionScene.TryGetValue(sceneName, out handler))
            {
                handler.Invoke(lmb, logicName, lc);
                return;
            }

            // Check for an inferred scene match.
            if (InferSingleSceneName(lc, out SceneName inferred) && logicOverridesByUniqueScene.TryGetValue(inferred, out handler))
            {
                handler.Invoke(lmb, logicName, lc);
                return;
            }

            // Do the standard logic edit.
            StandardLogicEdit(lmb, logicName, lc);
        }

        private readonly Dictionary<string, LogicClause> logicCache = new();

        private LogicClause GetCachedLogic(string logic) => logicCache.TryGetValue(logic, out LogicClause lc) ? lc : (logicCache[logic] = new(logic));

        private void NoLogicEdit(LogicManagerBuilder lmb, string logicName, LogicClause lc) { }

        private void StandardLogicEdit(LogicManagerBuilder lmb, string logicName, LogicClause lc) =>
            LogicClauseEditor.EditDarkness(lmb, logicName, LanternToken, GetSceneNameInferrer(logicName), l => l.Add(DarkroomsToken));

        private LogicOverride CustomDarkLogicEdit(string darkLogic)
        {
            return (lmb, logicName, lc) => LogicClauseEditor.EditDarkness(lmb, logicName, LanternToken, GetSceneNameInferrer(logicName), (sink) =>
            {
                LogicClause repl = GetCachedLogic(darkLogic);
                foreach (var t in repl)
                {
                    sink.Add(t);
                }
            });
        }

        private LogicOverride CustomSceneLogicEdit(SceneName sceneName, string darkLogic)
        {
            return (lmb, logicName, lc) =>
            {
                StandardLogicEdit(lmb, logicName, lc);
                if (SceneCanBeDark(sceneName))
                {
                    lmb.DoLogicEdit(new(logicName, $"ORIG + ($DarknessLevel[{sceneName}]<2 | {LanternToken.Write()} | {darkLogic})"));
                }
            };
        }
    }

    internal static class LogicPatcher
    {
        public static void Setup()
        {
            RCData.RuntimeLogicOverride.Subscribe(60f, ModifyLMB);
        }

        public static void ModifyLMB(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (RandoInterop.ShatteredLantern)
            {
                var shardsTerm = lmb.GetOrAddTerm("LANTERNSHARDS");
                var lanternTerm = lmb.GetOrAddTerm(RandoInterop.LanternTermName);
                lmb.AddTemplateItem(new BranchedItemTemplate(RandoInterop.LanternShardItemName, $"{shardsTerm.Name}<3",
                    new SingleItem("LanternShard-GetShard", new(shardsTerm, 1)),
                    new CappedItem("LanternShard-GetLantern", new TermValue[] { new(shardsTerm, 1), new(lanternTerm, 1) }, new(lanternTerm, 1))));
            }

            if (!RandoInterop.RandomizeDarkness) return;

            var newResolver = new DarknessVariableResolver(lmb.VariableResolver);
            lmb.VariableResolver = newResolver;

            LogicOverrides overrides = new();

            // We want to generically modify logic by location (SceneName), but unfortunately the LogicManager is constructed
            // before any of the location info is provided via the RequestBuilder, so we have to be creative.
            //
            // We'll inspect every logic clause for the set of scene transitions it references. If there is only one scene,
            // we will update the logic accordingly. If there are zero scenes, we ignore it, and if there are two or more, we
            // require custom handling.
            //
            // We defer the edits to avoid messing with dictionary iteration order.
            List<string> names = new();
            foreach (var e in lmb.LogicLookup)
            {
                names.Add(e.Key);
            }
            names.ForEach(n => overrides.EditLogicClause(lmb, n, lmb.LogicLookup[n]));
        }
    }
}
