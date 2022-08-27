using DarknessRandomizer.Lib;

namespace DarknessRandomizer.Data
{
    public static class GraphData
    {
        private static readonly Lib.SceneData SemiDark = new() { MaximumDarkness = Darkness.SemiDark };

        public static Graph LoadGraph()
        {
            Graph g = new();

            g.Clusters[Cluster.BlackEggRadiance] = new() {
                Scenes = new() { { SceneName.EggRadiance, new() } },
                AdjacentClusters = new() { { Cluster.BlackEggTemple, RelativeDarkness.Darker } },
                CursedOnly = true,
                CostWeight = 300 };

            g.Clusters[Cluster.BlackEggTemple] = new() {
                Scenes = new() {
                    { SceneName.EggBench, SemiDark },
                    { SceneName.BlackEggTemple, SemiDark },
                    { SceneName.EggHollowKnight, new() } },
                CursedOnly = true,
                CostWeight = 300 };

            g.Clusters[Cluster.BlueLake] = new() {
                Scenes = new() {
                    { SceneName.GroundsBlueLake, new() },
                    { SceneName.GroundsCorridorBelowXero, new() } },
                AdjacentClusters = new() {
                    { Cluster.CrossroadsLower, RelativeDarkness.Any },
                    // FIXME: EastCityElevator
                },
                ProbabilityWeight = 80,
                CostWeight = 60 };

            g.Clusters[Cluster.CliffsBaldur] = new() {
                Scenes = new() { { SceneName.CliffsBaldursShell, new() } },
                AdjacentClusters = new() {
                    { Cluster.CliffsMain, RelativeDarkness.Darker },
                    { Cluster.GreenpathWest, RelativeDarkness.Darker } },
                ProbabilityWeight = 80 };

            g.Clusters[Cluster.CliffsJonis] = new() {
                Scenes = new() { { SceneName.CliffsJonisDark, new() } },
                AdjacentClusters = new() { { Cluster.CliffsMain, RelativeDarkness.Brighter } },
                ProbabilityWeight = 80 };

            g.Clusters[Cluster.CliffsMain] = new() {
                Scenes = new() {
                    { SceneName.CliffsMain, new() },
                    { SceneName.CliffsGrimmLantern, SemiDark },
                    { SceneName.CliffsGorb, new() },
                    { SceneName.CliffsMato, SemiDark },
                    { SceneName.CliffsStagNest, SemiDark } },
                AdjacentClusters = new() { { Cluster.KingsPass, RelativeDarkness.Any } },
                ProbabilityWeight = 60,
                CostWeight = 250,
                SemiDarkProbabilty = 40 };

            g.Clusters[Cluster.CrossroadsAncestralMound] = new() {
                Scenes = new() {
                    { SceneName.CrossroadsOutsideMound, SemiDark },
                    { SceneName.CrossroadsAncestralMound, new() } },
                AdjacentClusters = new() {
                    { Cluster.CrossroadsWest, RelativeDarkness.Brighter },
                    { Cluster.CrossroadsFalseKnight, RelativeDarkness.Any } },
                ProbabilityWeight = 60,
                CostWeight = 120 };

            g.Clusters[Cluster.CrossroadsCanyonBridge] = new() {
                Scenes = new() {
                    { SceneName.CrossroadsAcidGrub, new() },
                    { SceneName.CrossroadsCorridortoAcidGrub, new() } },
                AdjacentClusters = new() { { Cluster.FogCanyonEast, RelativeDarkness.Darker } },
                ProbabilityWeight = 40,
                CostWeight = 60 };

            g.Clusters[Cluster.CrossroadsEntrance] = new() {
                Scenes = new() {
                    { SceneName.CrossroadsWell, new() },
                    { SceneName.CrossroadsOutsideTemple, SemiDark } },
                AdjacentClusters = new() {
                    { Cluster.CrossroadsUpper, RelativeDarkness.Darker },
                    { Cluster.CrossroadsWest, RelativeDarkness.Darker } },
                ProbabilityWeight = 5,
                CostWeight = 25 };

            g.Clusters[Cluster.CrossroadsFailedChamp] = new() {
                Scenes = new() { { SceneName.DreamFailedChampion, new() { ProficientCombatLocs = LocationSet.ALL } } },
                AdjacentClusters = new() { { Cluster.CrossroadsFalseKnight, RelativeDarkness.Darker } },
                CursedOnly = true,
                ProbabilityWeight = 80,
                CostWeight = 200 };

            g.Clusters[Cluster.CrossroadsFalseKnight] = new() {
                Scenes = new() {
                    { SceneName.CrossroadsFalseKnightArena, new() },
                    { SceneName.CrossroadsOutsideFalseKnight, new() } },
                AdjacentClusters = new() { { Cluster.CrossroadsAncestralMound, RelativeDarkness.Any } },
                ProbabilityWeight = 120 };

            g.Clusters[Cluster.CrossroadsGlowingWomb] = new() {
                Scenes = new() { { SceneName.CrossroadsGlowingWombArena, new() } },
                AdjacentClusters = new() { { Cluster.CrossroadsFalseKnight, RelativeDarkness.Brighter } } };

            g.Clusters[Cluster.CrossroadsGreenpathBridge] = new() {
                Scenes = new() { { SceneName.CrossroadsGreenpathEntrance, new() } },
                AdjacentClusters = new() {
                    { Cluster.CrossroadsWest, RelativeDarkness.Any },
                    { Cluster.GreenpathEntrance, RelativeDarkness.Any } },
                OverrideIsDarknessSource = false,
                ProbabilityWeight = 15,
                CostWeight = 60 };

            g.Clusters[Cluster.CrossroadsLower] = new() {
                Scenes = new() {
                    { SceneName.CrossroadsGruzMother, new() },
                    { SceneName.CrossroadsOutsideStag, new() },
                    { SceneName.CrossroadsStag, SemiDark },
                    { SceneName.CrossroadsBeforeGruzMother, new() },
                    { SceneName.CrossroadsRightOfMaskShard, new() },
                    { SceneName.CrossroadsOutsideTram, new() },
                    { SceneName.CrossroadsCorridortoTram, new() },
                    { SceneName.CrossroadsRescueSly, SemiDark },
                    { SceneName.CrossroadsMenderbug, SemiDark },
                    { SceneName.Salubra, SemiDark } },
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
                    { SceneName.CrossroadsMawlekEntrance, new() },
                    { SceneName.CrossroadsMawlekMiddle, new() },
                    { SceneName.CrossroadsMawlekBoss, new() } },
                AdjacentClusters = new() {
                    { Cluster.CrossroadsWest, RelativeDarkness.Brighter } },
                ProbabilityWeight = 30,
                CostWeight = 150 };

            g.Clusters[Cluster.CrossroadsPeaksBridge] = new() {
                Scenes = new() {
                    { SceneName.CrossroadsMyla, SemiDark },
                    { SceneName.CrystalDiveEntrance, SemiDark } },
                AdjacentClusters = new() { { Cluster.CrossroadsUpper, RelativeDarkness.Darker } } };

            g.Clusters[Cluster.CrossroadsPeaksToll] = new() {
                Scenes = new() { { SceneName.CrossroadsPeakDarkToll, new() { MinimumDarkness = Darkness.SemiDark } } },
                AdjacentClusters = new() { { Cluster.CrossroadsLower, RelativeDarkness.Any } },
                ProbabilityWeight = 500,
                CostWeight = 50 };

            g.Clusters[Cluster.CrossroadsSpikeGrub] = new() {
                Scenes = new() { { SceneName.CrossroadsSpikeGrub, new() } },
                ProbabilityWeight = 40,
                CostWeight = 40 };

            g.Clusters[Cluster.CrossroadsUpper] = new() {
                Scenes = new() {
                    { SceneName.CrossroadsCorridorRightofTemple, new() },
                    { SceneName.CrossroadsGuardedGrub, new() },
                    { SceneName.CrossroadsOutsideMyla, new() },
                    { SceneName.CrossroadsAboveLever, new() },
                    { SceneName.CrossroadsCorridorRightofCentralGrub, new() } },
                AdjacentClusters = new() {
                    { Cluster.CrossroadsWest, RelativeDarkness.Any },
                    { Cluster.CrossroadsLower, RelativeDarkness.Any },
                    { Cluster.CrossroadsPeaksBridge, RelativeDarkness.Any } },
                ProbabilityWeight = 15,
                CostWeight = 200 };

            g.Clusters[Cluster.CrossroadsWest] = new() {
                Scenes = new() {
                    { SceneName.CrossroadsCentralGrub, new() },
                    { SceneName.CrossroadsGruzzer, new() },
                    { SceneName.CrossroadsCornifer, new() },
                    { SceneName.CrossroadsAspidArena, new() },
                    { SceneName.CrossroadsGoamMaskShard, new() },
                    { SceneName.CrossroadsFungalEntrance, new() } },
                ProbabilityWeight = 5,
                CostWeight = 120 };

            g.Clusters[Cluster.CrystalPeakCrown] = new() {
                Scenes = new() {
                    { SceneName.CrystalCrownWhisperingRoot, new() },
                    { SceneName.CrystalCrownGrub, new() },
                    { SceneName.CrystalCrownClimb, new() },
                    { SceneName.CrystalCrownPeak, new() } },
                AdjacentClusters = new() { { Cluster.CrystalPeakUpper, RelativeDarkness.Brighter } },
                ProbabilityWeight = 150,
                CostWeight = 200 };

            g.Clusters[Cluster.CrystalPeakCrystallizedMound] = new() {
                Scenes = new() { { SceneName.CrystalOutsideMound, SemiDark }, { SceneName.CrystalMound, new() } },
                ProbabilityWeight = 125 };

            g.Clusters[Cluster.CrystalPeakDarkRoom] = new() {
                Scenes = new() { { SceneName.CrystalDarkRoom, new() } },
                AdjacentClusters = new() { { Cluster.CrystalPeakCrystallizedMound, RelativeDarkness.Any } },
                ProbabilityWeight = 250,
                CostWeight = 250 };

            g.Clusters[Cluster.CrystalPeakDeepFocus] = new() {
                Scenes = new() {
                    { SceneName.CrystalDeepFocus, SemiDark },
                    { SceneName.CrystalDeepFocusGauntlet, new() { ProficientCombatLocs = LocationSet.ALL } } },
                AdjacentClusters = new() { { Cluster.CrystalPeakWest, RelativeDarkness.Any } },
                ProbabilityWeight = 150 };

            g.Clusters[Cluster.CrystalPeakDirtmouthBridge] = new() {
                Scenes = new() {
                    { SceneName.CrystalCornifer, SemiDark },
                    { SceneName.CrystalMimic, new() },
                    { SceneName.CrystalElevatorEntrance, new() } } };

            g.Clusters[Cluster.CrystalPeakGuardian] = new() {
                Scenes = new() {
                    { SceneName.CrystalGuardianBench, new() },
                    { SceneName.CrystalEnragedGuardianArena, new() { ProficientCombatLocs = LocationSet.ALL } } },
                CostWeight = 200 };

            g.Clusters[Cluster.CrystalPeakHeart] = new() {
                Scenes = new() { { SceneName.CrystalCrystalHeartGauntlet, new() { DifficultSkipLocs = LocationSet.ALL } } },
                AdjacentClusters = new() { { Cluster.CrystalPeakUpper, RelativeDarkness.Brighter } },
                ProbabilityWeight = 150,
                CostWeight = 200 };

            g.Clusters[Cluster.CrystalPeakLower] = new() {
                Scenes = new() {
                    { SceneName.CrystalEntranceConveyors, new() },
                    { SceneName.CrystalDarkBench, new() },
                    { SceneName.CrystalMainEntrance, new() },
                    { SceneName.CrystalCorridortoSpikeGrub, new() },
                    { SceneName.CrystalChestCrushers, new() } },
                AdjacentClusters = new() {
                    { Cluster.CrystalPeakDarkRoom, RelativeDarkness.Any },
                    { Cluster.CrossroadsPeaksBridge, RelativeDarkness.Any },
                    { Cluster.CrossroadsPeaksToll, RelativeDarkness.Any },
                    { Cluster.CrystalPeakUpper, RelativeDarkness.Darker } },
                ProbabilityWeight = 15,
                CostWeight = 200 };

            g.Clusters[Cluster.CrystalPeakUpper] = new() {
                Scenes = new() {
                    { SceneName.CrystalEastTall, new() },
                    { SceneName.CrystalTopCorridor, SemiDark } },
                AdjacentClusters = new() {
                    { Cluster.CrystalPeakHeart, RelativeDarkness.Brighter },
                    { Cluster.CrystalPeakGuardian, RelativeDarkness.Any },
                    { Cluster.CrystalPeakWest, RelativeDarkness.Any },
                    { Cluster.CrystalPeakLower, RelativeDarkness.Any } },
                ProbabilityWeight = 50,
                CostWeight = 150 };

            g.Clusters[Cluster.CrystalPeakWest] = new() {
                Scenes = new() {
                    { SceneName.CrystalLeftOfGuardian, new() },
                    { SceneName.CrystalSpikeGrub, new() },
                    { SceneName.CrystalAboveSpikeGrub, new() } },
                AdjacentClusters = new() {
                    { Cluster.CrystalPeakDirtmouthBridge, RelativeDarkness.Any },
                    { Cluster.CrystalPeakGuardian, RelativeDarkness.Darker },
                    { Cluster.CrystalPeakLower, RelativeDarkness.Brighter } },
                ProbabilityWeight = 50,
                CostWeight = 200 };

            g.Clusters[Cluster.DirtmouthGPZ] = new() {
                Scenes = new() {
                    { SceneName.Bretta, new() {
                        MaximumDarkness = Darkness.SemiDark,
                        ProficientCombatLocs = new LocationsList("Boss_Essence-Grey_Prince_Zote") } },
                    { SceneName.BrettaBasement, SemiDark },
                    { SceneName.GPZ, new() } },
                CursedOnly = true,
                ProbabilityWeight = 40,
                CostWeight = 200 };

            g.Clusters[Cluster.DirtmouthGrimm] = new() {
                Scenes = new() {
                    { SceneName.GrimmTent, new() { ProficientCombatLocs = LocationSet.ALL } },
                    { SceneName.GrimmNKG, new() } },
                CursedOnly = true,
                ProbabilityWeight = 50,
                CostWeight = 200 };

            g.Clusters[Cluster.FogCanyonArchives] = new() {
                Scenes = new() {
                    { SceneName.FogArchivesBench, SemiDark },
                    { SceneName.FogUumuuArena, new() { ProficientCombatLocs = LocationSet.ALL } } },
                AdjacentClusters = new() { { Cluster.FogCanyonEast, RelativeDarkness.Any } },
                CostWeight = 150 };

            g.Clusters[Cluster.FogCanyonEast] = new() {
                Scenes = new() {
                    { SceneName.FogCorridortoCornifer, new() },
                    { SceneName.FogCorridortoArchives, new() },
                    { SceneName.FogEastTall, new() } },
                AdjacentClusters = new() { { Cluster.FogCanyonWest, RelativeDarkness.Any } },
                ProbabilityWeight = 50,
                CostWeight = 200 };

            g.Clusters[Cluster.FogCanyonOvergrownMound] = new() {
                Scenes = new() {
                    { SceneName.FogOvergrownMoundEntrance, new() },
                    { SceneName.FogOvergrownMound, new() } },
                CostWeight = 60 };

            g.Clusters[Cluster.FogCanyonNotch] = new() {
                Scenes = new() { { SceneName.FogCharmNotch, new() { DifficultSkipLocs = LocationSet.ALL } } },
                AdjacentClusters = new() { { Cluster.FogCanyonEast, RelativeDarkness.Brighter } } };

            g.Clusters[Cluster.FogCanyonWest] = new() {
                Scenes = new() {
                    { SceneName.FogCornifer, new() },
                    { SceneName.FogUpperWestTall, new() },
                    { SceneName.FogLowerWestTall, new() },
                    { SceneName.FogCorridortoOvergrownMound, new() },
                    { SceneName.FogLifeblood, SemiDark },
                    { SceneName.FogMillibelle, SemiDark } },
                AdjacentClusters = new() {
                    { Cluster.GreenpathOutsideNoEyes, RelativeDarkness.Any },
                    { Cluster.FogCanyonOvergrownMound, RelativeDarkness.Darker } },
                ProbabilityWeight = 15,
                CostWeight = 300 };

            g.Clusters[Cluster.GreenpathCliffsBridge] = new() {
                Scenes = new() {
                    { SceneName.GreenpathVengeflyKing, new() },
                    { SceneName.GreenpathMossKnightArena, new() } },
                AdjacentClusters = new() {
                    { Cluster.GreenpathWest, RelativeDarkness.Darker },
                    { Cluster.CliffsMain, RelativeDarkness.Any } } };

            g.Clusters[Cluster.GreenpathEntrance] = new() {
                Scenes = new() {
                    { SceneName.GreenpathWaterfallBench, SemiDark },
                    { SceneName.GreenpathEntrance, new() } },
                AdjacentClusters = new() { { Cluster.GreenpathUpper, RelativeDarkness.Darker } },
                ProbabilityWeight = 25,
                CostWeight = 50 };

            g.Clusters[Cluster.GreenpathHornet] = new() {
                Scenes = new() { { SceneName.GreenpathHornet, new() } },
                ProbabilityWeight = 120,
                CostWeight = 80 };

            g.Clusters[Cluster.GreenpathLower] = new() {
                Scenes = new() {
                    { SceneName.GreenpathAcidBridge, new() },
                    { SceneName.GreenpathAboveSanctuaryBench, new() },
                    { SceneName.GreenpathOutsideHunter, new() },
                    { SceneName.GreenpathHunter, SemiDark } },
                AdjacentClusters = new() { { Cluster.GreenpathOutsideNoEyes, RelativeDarkness.Any } },
                ProbabilityWeight = 15,
                CostWeight = 200,
                SemiDarkProbabilty = 80 };

            g.Clusters[Cluster.GreenpathMMC] = new() {
                Scenes = new() {
                    { SceneName.GreenpathMassiveMossCharger, new() },
                    { SceneName.GreenpathMMCCorridor, new() } },
                AdjacentClusters = new() { { Cluster.GreenpathOutsideNoEyes, RelativeDarkness.Brighter } },
                ProbabilityWeight = 80,
                CostWeight = 120 };

            g.Clusters[Cluster.GreenpathNoEyes] = new() {
                Scenes = new() { { SceneName.GreenpathStoneSanctuary, new() } } };

            g.Clusters[Cluster.GreenpathOutsideNoEyes] = new() {
                Scenes = new() {
                    { SceneName.GreenpathAboveFogCanyon, new() },
                    { SceneName.GreenpathSanctuaryBench, SemiDark },
                    { SceneName.GreenpathStoneSanctuaryEntrance, new() { MinimumDarkness = Darkness.SemiDark } } },
                AdjacentClusters = new() { { Cluster.GreenpathNoEyes, RelativeDarkness.Any } },
                CostWeight = 140 };

            g.Clusters[Cluster.GreenpathSheo] = new() {
                Scenes = new() {
                    {
                        SceneName.GreenpathSheoGauntlet,
                        new() { DifficultSkipLocs = new LocationsList("Fungus1_09[left1]") }
                    },
                    { SceneName.GreenpathOutsideSheo, new() },
                    { SceneName.GreenpathSheo, SemiDark } },
                ProbabilityWeight = 80,
                CostWeight = 200 };

            g.Clusters[Cluster.GreenpathThorns] = new() {
                Scenes = new() { { SceneName.GreenpathThornsofAgony, new() } },
                CostWeight = 80 };

            g.Clusters[Cluster.GreenpathUnn] = new() {
                Scenes = new() {
                    { SceneName.GreenpathLakeOfUnn, new() },
                    { SceneName.GreenpathUnn, new() },
                    { SceneName.GreenpathUnnBench, new() { MaximumDarkness = Darkness.SemiDark} } },
                ProbabilityWeight = 80,
                CostWeight = 50 };

            g.Clusters[Cluster.GreenpathUnnPass] = new() {
                Scenes = new() {
                    { SceneName.GreenpathCorridortoUnn, SemiDark } },
                AdjacentClusters = new() {
                    { Cluster.GreenpathUnn, RelativeDarkness.Darker },
                    { Cluster.GreenpathHornet, RelativeDarkness.Darker } } };

            g.Clusters[Cluster.GreenpathUpper] = new() {
                Scenes = new() {
                    { SceneName.GreenpathOutsideThorns, new() },
                    { SceneName.GreenpathToll, new() },
                    { SceneName.GreenpathStorerooms, new() },
                    { SceneName.GreenpathFirstHornetSighting, new() },
                    { SceneName.GreenpathCornifer, new() },
                    { SceneName.GreenpathChargerCorridor, new() } },
                AdjacentClusters = new() {
                    { Cluster.GreenpathCliffsBridge, RelativeDarkness.Any },
                    { Cluster.GreenpathLower, RelativeDarkness.Any },
                    { Cluster.GreenpathThorns, RelativeDarkness.Darker } },
                ProbabilityWeight = 15,
                CostWeight = 300,
                SemiDarkProbabilty = 80 };

            g.Clusters[Cluster.GreenpathWest] = new() {
                Scenes = new() {
                    { SceneName.GreenpathOutsideHornet, new() },
                    { SceneName.GreenpathOutsideStag, new() },
                    { SceneName.GreenpathStag, SemiDark },
                    { SceneName.GreenpathBelowTollBench, new() } },
                AdjacentClusters = new() {
                    { Cluster.GreenpathHornet, RelativeDarkness.Darker },
                    { Cluster.GreenpathSheo, RelativeDarkness.Any } },
                ProbabilityWeight = 60,
                CostWeight = 200,
                SemiDarkProbabilty = 80 };

            g.Clusters[Cluster.KingsPass] = new() {
                Scenes = new() { { SceneName.KingsPass, new() } },
                CostWeight = 50 };

            g.Clusters[Cluster.RestingGroundsCatacombs] = new() {
                Scenes = new() {
                    { SceneName.GroundsCrypts, new() },
                    { SceneName.GroundsOutsideGreyMourner, SemiDark } },
                // FIXME: EastCityElevator
            };

            g.Clusters[Cluster.RestingGroundsDreamNail] = new() {
                Scenes = new() {
                    { SceneName.DreamNail, new() },
                    { SceneName.GroundsDreamNailEntrance, SemiDark } },
                AdjacentClusters = new() { { Cluster.RestingGroundsMain, RelativeDarkness.Brighter } },
                ProbabilityWeight = 150,
                CostWeight = 25 };

            g.Clusters[Cluster.RestingGroundsMain] = new() {
                Scenes = new() {
                    { SceneName.GroundsWhisperingRoot, SemiDark },
                    { SceneName.GroundsDreamshield, SemiDark },
                    { SceneName.GroundsSpiritsGlade, SemiDark } },
                AdjacentClusters = new() { { Cluster.RestingGroundsCatacombs, RelativeDarkness.Darker } } };

            g.Clusters[Cluster.RestingGroundsXero] = new() {
                Scenes = new() { { SceneName.GroundsXero, new() } },
                AdjacentClusters = new() {
                    { Cluster.RestingGroundsDreamNail, RelativeDarkness.Any },
                    { Cluster.BlueLake, RelativeDarkness.Any } } };

            g.Clusters[Cluster.WestCityElevator] = new() {
                Scenes = new() {
                    { SceneName.CrossroadsCorridortoElevator, SemiDark },
                    { SceneName.CrossroadsElevator, SemiDark },
                    { SceneName.CityLeftElevator, new() } },
                AdjacentClusters = new() {
                    { Cluster.CrossroadsLower, RelativeDarkness.Any } },
                ProbabilityWeight = 50,
                CostWeight = 200 };

            g.Init();
            return g;
        }
    }
}
