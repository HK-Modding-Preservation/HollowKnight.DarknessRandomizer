using DarknessRandomizer.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Data
{
    public static class GraphData
    {
        private static readonly Scene SemiDark = new() { MaximumDarkness = Darkness.SemiDark };

        public static Graph LoadGraph()
        {
            Graph g = new();

            g.Clusters[Cluster.BlackEggRadiance] = new() {
                Scenes = new() { { Scenes.EggRadiance, new() } },
                AdjacentClusters = new() { { Cluster.BlackEggTemple, RelativeDarkness.Darker } },
                CursedOnly = true,
                CostWeight = 300 };

            g.Clusters[Cluster.BlackEggTemple] = new() {
                Scenes = new() {
                    { Scenes.EggBench, SemiDark },
                    { Scenes.BlackEggTemple, SemiDark },
                    { Scenes.EggHollowKnight, new() } },
                CursedOnly = true,
                CostWeight = 300 };

            g.Clusters[Cluster.CliffsBaldur] = new() {
                Scenes = new() { { Scenes.CliffsBaldursShell, new() } },
                AdjacentClusters = new() {
                    { Cluster.CliffsMain, RelativeDarkness.Darker },
                    { Cluster.GreenpathWest, RelativeDarkness.Darker } },
                ProbabilityWeight = 80 };

            g.Clusters[Cluster.CliffsJonis] = new() {
                Scenes = new() { { Scenes.CliffsJonisDark, new() } },
                AdjacentClusters = new() { { Cluster.CliffsMain, RelativeDarkness.Brighter } },
                ProbabilityWeight = 80 };

            g.Clusters[Cluster.CliffsMain] = new() {
                Scenes = new() {
                    { Scenes.CliffsMain, new() },
                    { Scenes.CliffsGrimmLantern, SemiDark },
                    { Scenes.CliffsGorb, new() },
                    { Scenes.CliffsMato, SemiDark },
                    { Scenes.CliffsStagNest, SemiDark } },
                AdjacentClusters = new() { { Cluster.KingsPass, RelativeDarkness.Any } },
                ProbabilityWeight = 60,
                CostWeight = 250,
                SemiDarkProbabilty = 40 };

            g.Clusters[Cluster.CrossroadsAncestralMound] = new() {
                Scenes = new() {
                    { Scenes.CrossroadsOutsideMound, SemiDark },
                    { Scenes.CrossroadsAncestralMound, new() } },
                AdjacentClusters = new() {
                    { Cluster.CrossroadsWest, RelativeDarkness.Brighter },
                    { Cluster.CrossroadsFalseKnight, RelativeDarkness.Any } },
                ProbabilityWeight = 60,
                CostWeight = 120 };

            g.Clusters[Cluster.CrossroadsCanyonBridge] = new() {
                Scenes = new() {
                    { Scenes.CrossroadsAcidGrub, new() },
                    { Scenes.CrossroadsCorridortoAcidGrub, new() } },
                AdjacentClusters = new() { { Cluster.FogCanyonEast, RelativeDarkness.Darker } },
                ProbabilityWeight = 40,
                CostWeight = 60 };

            g.Clusters[Cluster.CrossroadsFailedChamp] = new() {
                Scenes = new() { { Scenes.DreamFailedChampion, new() { ProficientCombatLocs = LocationSet.ALL } } },
                AdjacentClusters = new() { { Cluster.CrossroadsFalseKnight, RelativeDarkness.Darker } },
                CursedOnly = true,
                ProbabilityWeight = 80,
                CostWeight = 200 };

            g.Clusters[Cluster.CrossroadsFalseKnight] = new() {
                Scenes = new() {
                    { Scenes.CrossroadsFalseKnightArena, new() },
                    { Scenes.CrossroadsOutsideFalseKnight, new() } },
                AdjacentClusters = new() { { Cluster.CrossroadsAncestralMound, RelativeDarkness.Any } },
                ProbabilityWeight = 120 };

            g.Clusters[Cluster.CrossroadsGlowingWomb] = new() {
                Scenes = new() { { Scenes.CrossroadsGlowingWombArena, new() } },
                AdjacentClusters = new() { { Cluster.CrossroadsFalseKnight, RelativeDarkness.Brighter } } };

            g.Clusters[Cluster.CrossroadsGreenpathBridge] = new() {
                Scenes = new() { { Scenes.CrossroadsGreenpathEntrance, new() } },
                AdjacentClusters = new() {
                    { Cluster.CrossroadsWest, RelativeDarkness.Any },
                    { Cluster.GreenpathEntrance, RelativeDarkness.Any } },
                OverrideIsDarknessSource = false,
                ProbabilityWeight = 15,
                CostWeight = 60 };

            g.Clusters[Cluster.CrossroadsLower] = new() {
                Scenes = new() {
                    { Scenes.CrossroadsGruzMother, new() },
                    { Scenes.CrossroadsOutsideStag, new() },
                    { Scenes.CrossroadsStag, SemiDark },
                    { Scenes.CrossroadsBeforeGruzMother, new() },
                    { Scenes.CrossroadsRightOfMaskShard, new() },
                    { Scenes.CrossroadsOutsideTram, new() },
                    { Scenes.CrossroadsCorridortoTram, new() },
                    { Scenes.CrossroadsRescueSly, SemiDark },
                    { Scenes.CrossroadsMenderbug, SemiDark },
                    { Scenes.Salubra, SemiDark } },
                AdjacentClusters = new() {
                    { Cluster.CrossroadsSpikeGrub, RelativeDarkness.Darker },
                    { Cluster.WestCityElevator, RelativeDarkness.Darker },
                    { Cluster.CrossroadsFalseKnight, RelativeDarkness.Any },
                    { Cluster.CrossroadsPeaksToll, RelativeDarkness.Any },
                    { Cluster.CrossroadsUpper, RelativeDarkness.Any } },
                ProbabilityWeight = 15,
                CostWeight = 200 };

            g.Clusters[Cluster.CrossroadsMawlek] = new() {
                Scenes = new() {
                    { Scenes.CrossroadsMawlekEntrance, new() },
                    { Scenes.CrossroadsMawlekMiddle, new() },
                    { Scenes.CrossroadsMawlekBoss, new() } },
                AdjacentClusters = new() {
                    { Cluster.CrossroadsWest, RelativeDarkness.Brighter } },
                ProbabilityWeight = 30,
                CostWeight = 150 };

            g.Clusters[Cluster.CrossroadsPeaksBridge] = new() {
                Scenes = new() {
                    { Scenes.CrossroadsMyla, SemiDark },
                    { Scenes.CrystalDiveEntrance, SemiDark } },
                AdjacentClusters = new() { { Cluster.CrossroadsUpper, RelativeDarkness.Darker } } };

            g.Clusters[Cluster.CrossroadsPeaksToll] = new() {
                Scenes = new() { { Scenes.CrossroadsPeakDarkToll, new() { MinimumDarkness = Darkness.SemiDark } } },
                AdjacentClusters = new() { { Cluster.CrossroadsLower, RelativeDarkness.Any } },
                ProbabilityWeight = 500,
                CostWeight = 50 };

            g.Clusters[Cluster.CrossroadsSpikeGrub] = new() {
                Scenes = new() { { Scenes.CrossroadsSpikeGrub, new() } },
                ProbabilityWeight = 40,
                CostWeight = 40 };

            g.Clusters[Cluster.CrossroadsUpper] = new() {
                Scenes = new() {
                    { Scenes.CrossroadsOutsideTemple, SemiDark },
                    { Scenes.CrossroadsCorridorRightofTemple, new() },
                    { Scenes.CrossroadsGuardedGrub, new() },
                    { Scenes.CrossroadsOutsideMyla, new() },
                    { Scenes.CrossroadsAboveLever, new() },
                    { Scenes.CrossroadsCorridorRightofCentralGrub, new() } },
                AdjacentClusters = new() {
                    { Cluster.CrossroadsEntrance, RelativeDarkness.Any },
                    { Cluster.CrossroadsWest, RelativeDarkness.Any },
                    { Cluster.CrossroadsLower, RelativeDarkness.Any },
                    { Cluster.CrossroadsPeaksBridge, RelativeDarkness.Any } },
                ProbabilityWeight = 15,
                CostWeight = 200 };

            g.Clusters[Cluster.CrossroadsWest] = new() {
                Scenes = new() {
                    { Scenes.CrossroadsCentralGrub, new() },
                    { Scenes.CrossroadsGruzzer, new() },
                    { Scenes.CrossroadsCornifer, new() },
                    { Scenes.CrossroadsAspidArena, new() },
                    { Scenes.CrossroadsGoamMaskShard, new() },
                    { Scenes.CrossroadsFungalEntrance, new() } },
                ProbabilityWeight = 5,
                CostWeight = 120 };

            /*CrystalPeakCrown,
        CrystalPeakCrystallizedMound,
        CrystalPeakDarkRoom,
        CrystalPeakDeepFocus,
        CrystalPeakDirtmouthBridge,
        CrystalPeakGuardian,
        CrystalPeakHeart,
        CrystalPeakLower,
        CrystalPeakUpper,
        CrystalPeakWest,*/

            g.Clusters[Cluster.DirtmouthGPZ] = new() {
                Scenes = new() {
                    { Scenes.Bretta, new() {
                        MaximumDarkness = Darkness.SemiDark,
                        ProficientCombatLocs = new LocationsList("Boss_Essence-Grey_Prince_Zote") } },
                    { Scenes.BrettaBasement, SemiDark },
                    { Scenes.GPZ, new() } },
                CursedOnly = true,
                ProbabilityWeight = 40,
                CostWeight = 200 };

            g.Clusters[Cluster.DirtmouthGrimm] = new() {
                Scenes = new() {
                    { Scenes.GrimmTent, new() { ProficientCombatLocs = LocationSet.ALL } },
                    { Scenes.GrimmNKG, new() } },
                CursedOnly = true,
                ProbabilityWeight = 50,
                CostWeight = 200 };

            g.Clusters[Cluster.FogCanyonArchives] = new() {
                Scenes = new() {
                    { Scenes.FogArchivesBench, SemiDark },
                    { Scenes.FogUumuuArena, new() { ProficientCombatLocs = LocationSet.ALL } } },
                AdjacentClusters = new() { { Cluster.FogCanyonEast, RelativeDarkness.Any } },
                CostWeight = 150 };

            g.Clusters[Cluster.FogCanyonEast] = new() {
                Scenes = new() {
                    { Scenes.FogCorridortoCornifer, new() },
                    { Scenes.FogCorridortoArchives, new() },
                    { Scenes.FogEastTall, new() } },
                AdjacentClusters = new() { { Cluster.FogCanyonWest, RelativeDarkness.Any } },
                ProbabilityWeight = 50,
                CostWeight = 200 };

            g.Clusters[Cluster.FogCanyonOvergrownMound] = new() {
                Scenes = new() {
                    { Scenes.FogOvergrownMoundEntrance, new() },
                    { Scenes.FogOvergrownMound, new() } },
                CostWeight = 60 };

            g.Clusters[Cluster.FogCanyonNotch] = new() {
                Scenes = new() { { Scenes.FogCharmNotch, new() { DifficultSkipLocs = LocationSet.ALL } } },
                AdjacentClusters = new() { { Cluster.FogCanyonEast, RelativeDarkness.Brighter } } };

            g.Clusters[Cluster.FogCanyonWest] = new() {
                Scenes = new() {
                    { Scenes.FogCornifer, new() },
                    { Scenes.FogUpperWestTall, new() },
                    { Scenes.FogLowerWestTall, new() },
                    { Scenes.FogCorridortoOvergrownMound, new() },
                    { Scenes.FogLifeblood, SemiDark },
                    { Scenes.FogMillibelle, SemiDark } },
                AdjacentClusters = new() {
                    { Cluster.GreenpathOutsideNoEyes, RelativeDarkness.Any },
                    { Cluster.FogCanyonOvergrownMound, RelativeDarkness.Darker } },
                ProbabilityWeight = 15,
                CostWeight = 300 };

            g.Clusters[Cluster.GreenpathCliffsBridge] = new() {
                Scenes = new() {
                    { Scenes.GreenpathVengeflyKing, new() },
                    { Scenes.GreenpathMossKnightArena, new() } },
                AdjacentClusters = new() {
                    { Cluster.GreenpathWest, RelativeDarkness.Darker },
                    { Cluster.CliffsMain, RelativeDarkness.Any } } };

            g.Clusters[Cluster.GreenpathEntrance] = new() {
                Scenes = new() {
                    { Scenes.GreenpathWaterfallBench, SemiDark },
                    { Scenes.GreenpathEntrance, new() } },
                AdjacentClusters = new() { { Cluster.GreenpathUpper, RelativeDarkness.Darker } },
                ProbabilityWeight = 25,
                CostWeight = 50 };

            g.Clusters[Cluster.GreenpathHornet] = new() {
                Scenes = new() { { Scenes.GreenpathHornet, new() } },
                ProbabilityWeight = 120,
                CostWeight = 80 };

            g.Clusters[Cluster.GreenpathLower] = new() {
                Scenes = new() {
                    { Scenes.GreenpathAcidBridge, new() },
                    { Scenes.GreenpathAboveSanctuaryBench, new() },
                    { Scenes.GreenpathOutsideHunter, new() },
                    { Scenes.GreenpathHunter, SemiDark } },
                AdjacentClusters = new() { { Cluster.GreenpathOutsideNoEyes, RelativeDarkness.Any } },
                ProbabilityWeight = 15,
                CostWeight = 200,
                SemiDarkProbabilty = 80 };

            g.Clusters[Cluster.GreenpathMMC] = new() {
                Scenes = new() {
                    { Scenes.GreenpathMassiveMossCharger, new() },
                    { Scenes.GreenpathMMCCorridor, new() } },
                AdjacentClusters = new() { { Cluster.GreenpathOutsideNoEyes, RelativeDarkness.Brighter } },
                ProbabilityWeight = 80,
                CostWeight = 120 };

            g.Clusters[Cluster.GreenpathNoEyes] = new() {
                Scenes = new() { { Scenes.GreenpathStoneSanctuary, new() } } };

            g.Clusters[Cluster.GreenpathOutsideNoEyes] = new() {
                Scenes = new() {
                    { Scenes.GreenpathAboveFogCanyon, new() },
                    { Scenes.GreenpathSanctuaryBench, SemiDark },
                    { Scenes.GreenpathStoneSanctuaryEntrance, new() { MinimumDarkness = Darkness.SemiDark } } },
                AdjacentClusters = new() { { Cluster.GreenpathNoEyes, RelativeDarkness.Any } },
                CostWeight = 140 };

            g.Clusters[Cluster.GreenpathSheo] = new() {
                Scenes = new() {
                    {
                        Scenes.GreenpathSheoGauntlet,
                        new() { DifficultSkipLocs = new LocationsList("Fungus1_09[left1]") }
                    },
                    { Scenes.GreenpathOutsideSheo, new() },
                    { Scenes.GreenpathSheo, SemiDark } },
                ProbabilityWeight = 80,
                CostWeight = 200 };

            g.Clusters[Cluster.GreenpathThorns] = new() {
                Scenes = new() { { Scenes.GreenpathThornsofAgony, new() } },
                CostWeight = 80 };

            g.Clusters[Cluster.GreenpathUnn] = new() {
                Scenes = new() {
                    { Scenes.GreenpathLakeOfUnn, new() },
                    { Scenes.GreenpathUnn, new() },
                    { Scenes.GreenpathUnnBench, new() { MaximumDarkness = Darkness.SemiDark} } },
                ProbabilityWeight = 80,
                CostWeight = 50 };

            g.Clusters[Cluster.GreenpathUnnPass] = new() {
                Scenes = new() {
                    { Scenes.GreenpathCorridortoUnn, SemiDark } },
                AdjacentClusters = new() {
                    { Cluster.GreenpathUnn, RelativeDarkness.Darker },
                    { Cluster.GreenpathHornet, RelativeDarkness.Darker } } };

            g.Clusters[Cluster.GreenpathUpper] = new() {
                Scenes = new() {
                    { Scenes.GreenpathOutsideThorns, new() },
                    { Scenes.GreenpathToll, new() },
                    { Scenes.GreenpathStorerooms, new() },
                    { Scenes.GreenpathFirstHornetSighting, new() },
                    { Scenes.GreenpathCornifer, new() },
                    { Scenes.GreenpathChargerCorridor, new() } },
                AdjacentClusters = new() {
                    { Cluster.GreenpathCliffsBridge, RelativeDarkness.Any },
                    { Cluster.GreenpathLower, RelativeDarkness.Any },
                    { Cluster.GreenpathThorns, RelativeDarkness.Darker } },
                ProbabilityWeight = 15,
                CostWeight = 300,
                SemiDarkProbabilty = 80 };

            g.Clusters[Cluster.GreenpathWest] = new() {
                Scenes = new() {
                    { Scenes.GreenpathOutsideHornet, new() },
                    { Scenes.GreenpathOutsideStag, new() },
                    { Scenes.GreenpathStag, SemiDark },
                    { Scenes.GreenpathBelowTollBench, new() } },
                AdjacentClusters = new() {
                    { Cluster.GreenpathHornet, RelativeDarkness.Darker },
                    { Cluster.GreenpathSheo, RelativeDarkness.Any } },
                ProbabilityWeight = 60,
                CostWeight = 200,
                SemiDarkProbabilty = 80 };

            g.Clusters[Cluster.KingsPass] = new() {
                Scenes = new() { { Scenes.KingsPass, new() } },
                CostWeight = 50 };

            g.Init();
            return g;
        }
    }
}
