using DarknessRandomizer.Data;
using DarknessRandomizer.IC;
using DarknessRandomizer.Lib;
using ItemChanger;
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
        private delegate void LogicOverride(LogicManagerBuilder lmb, string name, LogicClause lc);

        private readonly Dictionary<string, LogicOverride> logicOverridesByName;
        private readonly Dictionary<SceneName, LogicOverride> logicOverridesByTransitionScene;
        private readonly Dictionary<SceneName, LogicOverride> logicOverridesByUniqueScene;

        public LogicOverrides()
        {
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
                { "Void_Heart", CustomSceneLogicEdit(SceneName.AbyssBirthplace, "DARKROOMS + DIFFICULTSKIPS") },

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

                // Using Elegant Key requires lantern.
                { $"{SceneName.CityTollBench.Name()}[left3]", CustomDarkLogicEdit("FALSE") },

                // Flower quest simply requires lantern.
                { "Mask_Shard-Grey_Mourner", (lmb, ln, lc) => lmb.DoLogicEdit(new(ln, "ORIG + LANTERN")) }
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

        private static readonly SimpleToken DarkroomsToken = new("DARKROOMS");

        private Dictionary<string, SceneName> customSceneInferences = new();

        private void DoBenchRandoInterop()
        {
            foreach (var e in BenchRando.BRData.BenchLookup)
            {
                var benchName = e.Key;
                var def = e.Value;
                if (!SceneName.TryGetValue(def.SceneName, out SceneName sceneName)) continue;

                // Make sure we apply darkness logic to other checks in the room obtainable from the bench.
                customSceneInferences[e.Key] = sceneName;

                // Bench checks are obtainable even in dark rooms, if the player has the benchwarp pickup.
                logicOverridesByName[e.Key] = (lmb, name, lc) =>
                {
                    bool inferScene(string term, out SceneName sceneName)
                    {
                        if (term == benchName)
                        {
                            sceneName = default;
                            return false;
                        }
                        return InferSceneName(term, out sceneName);
                    }
                    LogicClauseEditor.EditDarkness(lmb, name, inferScene, tokens => tokens.Add(DarkroomsToken));
                };
            }
        }

        public bool InferSceneName(string term, out SceneName sceneName)
        { 
            if (SceneName.IsTransition(term, out sceneName))
            {
                return true;
            }

            if (customSceneInferences.TryGetValue(term, out sceneName))
            {
                return true;
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

        public bool MaybeInvokeLogicOverride(LogicManagerBuilder lmb, string name, LogicClause lc)
        {
            if (logicOverridesByName.TryGetValue(name, out LogicOverride handler))
            {
                handler.Invoke(lmb, name, lc);
                return true;
            }

            if (InferSceneName(name, out SceneName sceneName)
                && logicOverridesByTransitionScene.TryGetValue(sceneName, out handler))
            {
                handler.Invoke(lmb, name, lc);
                return true;
            }

            // Check for an inferred scene match.
            if (InferSingleSceneName(lc, out SceneName inferred) && logicOverridesByUniqueScene.TryGetValue(inferred, out handler))
            {
                handler.Invoke(lmb, name, lc);
                return true;
            }

            return false;
        }

        private Dictionary<string, LogicClause> logicCache = new();

        private LogicClause GetCachedLogic(string logic)
        {
            if (logicCache.TryGetValue(logic, out LogicClause lc))
            {
                return lc;
            }

            lc = new(logic);
            logicCache[logic] = lc;
            return lc;
        }

        private void NoLogicEdit(LogicManagerBuilder lmb, string name, LogicClause lc) { }

        private LogicOverride CustomDarkLogicEdit(string darkLogic)
        {
            return (lmb, name, lc) => LogicClauseEditor.EditDarkness(lmb, name, InferSceneName, (sink) =>
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
            return (lmb, name, lc) => lmb.DoLogicEdit(
                new(name, $"ORIG + ($DarknessLevel[{sceneName}]<2 | LANTERN | {darkLogic})"));
        }
    }

    internal static class LogicPatcher
    {
        public static void Setup()
        {
            RCData.RuntimeLogicOverride.Subscribe(100.0f, ModifyLMB);
        }

        public static void ModifyLMB(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (RandoInterop.ShatteredLantern)
            {
                var shardsTerm = lmb.GetOrAddTerm("LANTERNSHARDS");
                var lanternTerm = lmb.GetOrAddTerm("LANTERN");
                lmb.AddTemplateItem(new BranchedItemTemplate(LanternShardItem.Name, $"{shardsTerm.Name}<3",
                    new SingleItem(LanternShardItem.Name, new(shardsTerm, 1)),
                    new SingleItem(ItemNames.Lumafly_Lantern, new(lanternTerm, 1))));
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
            // TODO: Special handling for BenchRando, TheRealJournalRando, possibly others?

            // We defer the edits to avoid messing with dictionary iteration order.
            List<Action> edits = new();
            foreach (var e in lmb.LogicLookup)
            {
                var name = e.Key;
                var lc = e.Value;
                edits.Add(() => EditLogicClause(overrides, lmb, name, lc));
            }
            edits.ForEach(e => e.Invoke());
        }

        private static readonly SimpleToken DarkroomsToken = new("DARKROOMS");

        private static void EditLogicClause(LogicOverrides overrides, LogicManagerBuilder lmb, string name, LogicClause lc)
        {
            if (overrides.MaybeInvokeLogicOverride(lmb, name, lc)) return;

            // No special matches, use the default editor.
            LogicClauseEditor.EditDarkness(lmb, name, overrides.InferSceneName, sink => sink.Add(DarkroomsToken));
        }
    }
}
