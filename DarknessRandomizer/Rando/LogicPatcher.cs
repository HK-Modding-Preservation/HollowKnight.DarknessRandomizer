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
        private readonly Dictionary<SceneName, LogicOverride> logicOverridesBySceneTransition;
        private readonly Dictionary<SceneName, LogicOverride> logicOverridesByUniqueScene;

        public SimpleToken LanternToken { get; }

        public LogicOverrides(LogicManagerBuilder lmb)
        {
            LanternToken = new SimpleToken(RandoInterop.LanternTermName);

            logicOverridesByName = new()
            {
                // Dream warriors do not appear in dark rooms without lantern.
                { WaypointName.DefeatedElderHu, CustomDarkLogicEdit("FALSE") },
                { WaypointName.DefeatedGalien, CustomDarkLogicEdit("FALSE") },
                { WaypointName.DefeatedGorb, CustomDarkLogicEdit("FALSE") },
                { WaypointName.DefeatedMarkoth, CustomDarkLogicEdit("FALSE") },
                { WaypointName.DefeatedMarmu, CustomDarkLogicEdit("FALSE") },
                { WaypointName.DefeatedNoEyes, CustomDarkLogicEdit("FALSE") },
                { WaypointName.DefeatedXero, CustomDarkLogicEdit("FALSE") },

                // Dream bosses are coded specially because the checks are located where the dream nail is swung,
                // but we care whether or not the actual fight room is dark. So we have to account for both rooms.
                { WaypointName.DefeatedFailedChampion, CustomSceneLogicEdit(SceneName.DreamFailedChampion, "FALSE") },
                { WaypointName.DefeatedGreyPrinceZote, CustomSceneLogicEdit(SceneName.DreamGreyPrinceZote, "FALSE") },
                { WaypointName.DefeatedLostKin, CustomSceneLogicEdit(SceneName.DreamLostKin, "DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },
                { WaypointName.DefeatedSoulTyrant, CustomSceneLogicEdit(SceneName.DreamSoulTyrant, "DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },
                { WaypointName.DefeatedWhiteDefender, CustomSceneLogicEdit(SceneName.DreamWhiteDefender, "DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },

                // These checks are free in the dark.
                { "Bench-Ancestral_Mound", NoLogicEdit },
                { "Dashmaster", NoLogicEdit },
                { "Mask_Shard-Deepnest", NoLogicEdit },
                { "Vengeful_Spirit", NoLogicEdit },

                // This check is special - ITEMRANDO assumes it's always open, even with infection, because even with infection you
                // can always go around itemless. We can't assume that, because parts of the route might be dark.
                { $"{SceneName.CrossroadsOutsideMound}[right1]", (lmb, ln, lc) => lmb.DoLogicEdit(new(ln, $"({SceneName.CrossroadsOutsideMound}[left1] | {SceneName.CrossroadsOutsideMound}[door1]) + ROOMRANDO")) },

                // Specific checks with difficult platforming.
                { "Void_Heart", CustomSceneLogicEdit(SceneName.DreamAbyss, "DARKROOMS + DIFFICULTSKIPS") },

                // QG stag checks are free except for these two.
                { "Soul_Totem-Below_Marmu", CustomDarkLogicEdit("DARKROOMS") },
                { $"{SceneName.GardensGardensStag}[top1]", CustomDarkLogicEdit("DARKROOMS") },

                // Basin toll bench requires lantern.
                { "Bench-Basin_Toll", CustomDarkLogicEdit("Bench-Basin_Toll") },

                // Cornifer bench makes the left transition free, but not vice versa.
                { $"{SceneName.GardensCornifer}[left1]", SkipDarkLogicFor("Bench-Gardens_Cornifer") },
                { "Queen's_Gardens_Map", SkipDarkLogicFor("Bench-Gardens_Cornifer") },

                // These checks are specifically dark-guarded in darkrooms.
                { "Rancid_Egg-Blue_Lake", StandardLogicEdit },
                { "Mask_Shard-Queen's_Station", StandardLogicEdit },

                // Greenpath toll bench requires lantern.
                { "Bench-Greenpath_Toll", CustomDarkLogicEdit("Bench-Greenpath_Toll") },

                // Dream nail has a custom scene which may be dark.
                { "Dream_Nail", CustomSceneLogicEdit(SceneName.DreamNail, "DARKROOMS") },

                // These checks are free from bench-rando benches.
                { "Crystal_Heart", SkipDarkLogicFor("Bench-Mining_Golem") },
                { "Isma's_Tear", SkipDarkLogicFor("Bench-Isma's_Grove") },
                { "King's_Idol-Deepnest", SkipDarkLogicFor("Bench-Zote's_Folly") },
                { "Tram_Pass", SkipDarkLogicFor("Bench-Destroyed_Tram") },

                // These bosses are deemed difficult in the dark.
                { "Defeated_Any_Hollow_Knight", CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },
                { "Defeated_Any_Nightmare_King", CustomDarkLogicEdit("FALSE") },
                { "Defeated_Any_Radiance", CustomDarkLogicEdit("FALSE") },
                { WaypointName.DefeatedBrokenVessel, CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
                { WaypointName.DefeatedBroodingMawlek, CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
                { WaypointName.DefeatedCollector, CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },
                { WaypointName.DefeatedColosseum1, CustomSceneLogicEdit(SceneName.EdgeColo1Arena, "DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },
                { WaypointName.DefeatedColosseum2, CustomSceneLogicEdit(SceneName.EdgeColo2Arena, "FALSE") },
                { "Defeated_Colosseum_3", CustomSceneLogicEdit(SceneName.EdgeColo3Arena, "FALSE") },
                { WaypointName.DefeatedCrystalGuardian, CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
                { WaypointName.DefeatedEnragedGuardian, CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },
                { WaypointName.DefeatedFlukemarm, CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
                { WaypointName.DefeatedHiveKnight, CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },
                { WaypointName.DefeatedHornet2, CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
                { WaypointName.DefeatedGrimm, CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
                { WaypointName.DefeatedMantisLords, CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
                { WaypointName.DefeatedNosk, CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
                { WaypointName.DefeatedPaleLurker, CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
                { WaypointName.DefeatedSoulMaster, CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
                { WaypointName.DefeatedTraitorLord, CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },
                { WaypointName.DefeatedUumuu, CustomDarkLogicEdit("DARKROOMS + PROFICIENTCOMBAT") },
                { WaypointName.DefeatedWatcherKnights, CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS + PROFICIENTCOMBAT") },

                // These two checks in the city toll room require lantern. All else is free.
                { "Bench-City_Toll", CustomDarkLogicEdit("Bench-City_Toll") },
                { $"{SceneName.CityTollBench.Name()}[left3]", CustomDarkLogicEdit("FALSE") },

                // Flower quest simply requires lantern.
                { "Mask_Shard-Grey_Mourner", (lmb, ln, lc) => lmb.DoLogicEdit(new(ln, $"ORIG + {LanternToken.Write()}")) }
            };

            FreeDarkroomsClique($"{SceneName.AbyssLighthouseClimb}[right3]", "Bench-Abyss_Workshop");
            FreeDarkroomsClique($"{SceneName.BasinBrokenVesselGrub}[bot2]", "Bench-Far_Basin");
            FreeDarkroomsClique($"{SceneName.BasinFountain}[top1]", "Bench-Basin_Hub");
            FreeDarkroomsClique($"{SceneName.CityGrubBelowKings}[left1]", "Bench-Flooded_Stag");
            FreeDarkroomsClique($"{SceneName.CityOutsideNailsmith}[door1]", "Bench-Nailsmith");
            FreeDarkroomsClique($"{SceneName.CityQuirrelBench}[bot1]", "Bench-Quirrel");
            FreeDarkroomsClique($"{SceneName.CitySanctumSpellTwister}[right1]", "Bench-Inner_Sanctum");
            FreeDarkroomsClique($"{SceneName.CliffsMain}[right1]", "Bench-Cliffs_Overhang");
            FreeDarkroomsClique($"{SceneName.CliffsMain}[right3]", "Bench-Blasted_Plains");
            FreeDarkroomsClique($"{SceneName.CrossroadsGreenpathEntrance}[right1]", "Bench-Pilgrim's_Start");
            FreeDarkroomsClique($"{SceneName.CrossroadsGruzMother}[door_charmshop]", $"{SceneName.CrossroadsGruzMother}[right1]", "Bench-Salubra");
            FreeDarkroomsClique($"{SceneName.CrystalDarkBench}[left1]", "Bench-Peak_Dark_Room");
            FreeDarkroomsClique($"{SceneName.CrystalGuardianBench}[left1]", $"{SceneName.CrystalGuardianBench}[right1]", "Bench-Crystal_Guardian");
            FreeDarkroomsClique($"{SceneName.DeepnestLowerCornifer}[top2]", $"{SceneName.DeepnestLowerCornifer}[right1]", "Bench-Deepnest_Gate");
            FreeDarkroomsClique($"{SceneName.DeepnestHotSpring}[left1]", $"{SceneName.DeepnestHotSpring}[right1]", "Bench-Deepnest_Hot_Springs");
            FreeDarkroomsClique($"{SceneName.DeepnestWeaversDen}[left1]", "Bench-Bench-Weaver's_Den");
            FreeDarkroomsClique($"{SceneName.DeepnestNoskArena}[left1]", "Bench-Nosk's_Lair");
            FreeDarkroomsClique($"{SceneName.DeepnestZoteArena}[bot1]", "Bench-Zote's_Folly");
            FreeDarkroomsClique($"{SceneName.EdgeHornetSentinelArena}[left1]", "Bench-Hornet's_Outpost");
            FreeDarkroomsClique($"{SceneName.EdgeOutsideOro}[door1]", $"{SceneName.EdgeOutsideOro}[right1]", "Bench-Oro");
            FreeDarkroomsClique($"{SceneName.EdgePaleLurker}[left1]", "Bench-Lurker's_Overlook");
            FreeDarkroomsClique($"{SceneName.EdgeWhisperingRoot}[left1]", "Bench-Edge_Summit");
            FreeDarkroomsClique($"{SceneName.FogOvergrownMound}[left1]", "Bench-Overgrown_Mound");
            FreeDarkroomsClique($"{SceneName.FungalBrettaBench}[left3]", "Bench-Bretta");
            FreeDarkroomsClique($"{SceneName.FungalCoreUpper}[right1]", "Bench-Fungal_Core");
            FreeDarkroomsClique($"{SceneName.FungalLeftOfPilgrimsWay}[bot1]", $"{SceneName.FungalLeftOfPilgrimsWay}[right1]", "Bench-Pilgrim's_End");
            FreeDarkroomsClique($"{SceneName.GardensBeforePetraArena}[left1]", "Bench-Gardens_Atrium");
            FreeDarkroomsClique($"{SceneName.GreenpathSheo}[door1]", "Bench-Sheo");
            FreeDarkroomsClique($"{SceneName.GreenpathSheoGauntlet}[right1]", "Bench-Duranda's_Trial");
            FreeDarkroomsClique($"{SceneName.GroundsCrypts}[top1]", "Bench-Crypts");
            FreeDarkroomsClique($"{SceneName.GroundsSpiritsGlade}[left1]", "Bench-Spirits'_Glade");
            FreeDarkroomsClique($"{SceneName.HiveOutsideShortcut}[right2]", "Bench-Hive_Hideaway");
            FreeDarkroomsClique($"{SceneName.KingsPass}[top2]", "Bench-King's_Pass");
            FreeDarkroomsClique($"{SceneName.WaterwaysHiddenGrub}[bot1]", $"{SceneName.WaterwaysHiddenGrub}[right1]");

            logicOverridesBySceneTransition = new()
            {
                // This gets overridden for bench rando.
                { SceneName.GreenpathToll, CustomDarkLogicEdit("FALSE") },

                // The following scenes are trivial to navigate while dark, but may contain a check which is
                // uniquely affected by darkness.
                { SceneName.GroundsXero, NoLogicEdit },
                { SceneName.FungalQueensStation, NoLogicEdit }
            };

            logicOverridesByUniqueScene = new()
            {
                // Checks in these rooms are easy to obtain if the player has isma's tear; there is no danger.
                { SceneName.GreenpathLakeOfUnn, CustomDarkLogicEdit("DARKROOMS | ACID") },
                { SceneName.GreenpathUnn, CustomDarkLogicEdit("DARKROOMS | ACID") },

                // Checks in these scenes are free, even if dark.
                { SceneName.CityTollBench, CustomDarkLogicEdit("ANY") },
                // Gardens checks by the stag are free; marmu, marmu totem, and the upper transition are exceptions.
                { SceneName.GardensGardensStag, CustomDarkLogicEdit("ANY") },
                { SceneName.GroundsBlueLake, CustomDarkLogicEdit("ANY") },

                // These scenes have difficult dark platforming.
                { SceneName.CrystalCrystalHeartGauntlet, CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS") },
                { SceneName.CrystalDeepFocusGauntlet, CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS") },
                { SceneName.EdgeWhisperingRoot, CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS") },
                { SceneName.GreenpathSheoGauntlet, CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS") },
                { SceneName.POPEntrance, CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS") },
                { SceneName.POPFinal, CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS") },
                { SceneName.POPLever, CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS") },
                { SceneName.POPVertical, CustomDarkLogicEdit("DARKROOMS + DIFFICULTSKIPS") },
            };

            if (ModHooks.GetMod("BenchRando") is Mod)
            {
                DoBenchRandoInterop(lmb);
            }

            if (ModHooks.GetMod("RandomizableLevers") is Mod && LeversAreEnabled())
            {
                DoLeverRandoInterop(lmb);
            }
            else
            {
                // We only care about the pilgrims way right transition.
                logicOverridesByName[$"{SceneName.FungalPilgrimsWay}[right1]"] =
                    CustomDarkLogicEdit($"DARKROOMS | {SceneName.FungalPilgrimsWay}[left1] + (ACID | RIGHTSUPERDASH)");
            }
        }

        private readonly Dictionary<string, SceneName> customSceneInferences = new()
        {
            { "Queen's_Gardens_Stag", SceneName.GardensGardensStag }
        };
        private readonly Dictionary<string, SceneNameInferrer> sceneNameInferrerOverrides = new();

        private void DoBenchRandoInterop(LogicManagerBuilder lmb)
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

            // Apply custom overrides only applicable in bench rando.
            if (lmb.LogicLookup.ContainsKey("Bench-Greenpath_Toll"))
            {
                logicOverridesByUniqueScene[SceneName.GreenpathToll] = CustomDarkLogicEdit("DARKROOMS | Bench-Greenpath_Toll");
            }
        }

        private static bool LeversAreEnabled() => RandomizableLevers.RandomizableLevers.GS.RandoSettings.RandomizeLevers;

        private void DoLeverRandoInterop(LogicManagerBuilder lmb)
        {
            string leftTransition = $"{SceneName.FungalPilgrimsWay}[left1]";
            // Apply custom lever logic to Pilgrim's way, which is partially dark.
            logicOverridesByName["Lever-Pilgrim's_Way_Left"] =
                CustomDarkLogicEdit($"DARKROOMS | {leftTransition} + (ACID | RIGHTSUPERDASH | Lever-Pilgrim's_Way_Left)");
            logicOverridesByName["Lever-Pilgrim's_Way_Right"] =
                CustomDarkLogicEdit($"DARKROOMS | {leftTransition} + (ACID | RIGHTSUPERDASH | Lever-Pilgrim's_Way_Left + Lever-Pilgrim's_Way_Right)");
            logicOverridesByName[$"{SceneName.FungalPilgrimsWay}[right1]"] =
                CustomDarkLogicEdit($"DARKROOMS | {leftTransition} + (ACID | RIGHTSUPERDASH | Lever-Pilgrim's_Way_Left + Lever-Pilgrim's_Way_Right)");
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
                && logicOverridesBySceneTransition.TryGetValue(sceneName, out handler))
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

        // Specifies that each logic name given is accessible from the others even in darkrooms.
        private void FreeDarkroomsClique(params string[] logicNames)
        {
            HashSet<string> all = new(logicNames);
            foreach (var ln in logicNames)
            {
                HashSet<string> others = new(all);
                others.Remove(ln);
                logicOverridesByName[ln] = SkipDarkLogicFor(others.ToArray());
            }
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

        private LogicOverride SkipDarkLogicFor(params string[] locTerms)
        {
            HashSet<string> set = new(locTerms);
            return (lmb, logicName, lc) => {
                var orig = GetSceneNameInferrer(logicName);
                bool inferScene(string term, out SceneName sceneName)
                {
                    if (set.Contains(term))
                    {
                        sceneName = default;
                        return false;
                    }
                    return orig(term, out sceneName);
                }

                LogicClauseEditor.EditDarkness(lmb, logicName, LanternToken, inferScene, (sink) => sink.Add(DarkroomsToken));
            };
        }

        private LogicOverride CustomExtraLogicEdit(LogicOverride extra) => (lmb, logicName, lc) =>
            {
                StandardLogicEdit(lmb, logicName, lc);
                extra.Invoke(lmb, logicName, lc);
            };

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

            LogicOverrides overrides = new(lmb);

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
