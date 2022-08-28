using DarknessRandomizer.Lib;

namespace DarknessRandomizer.Data
{
    public static class GraphData
    {
        private static readonly Lib.SceneData SemiDark = new() { MaximumDarkness = Darkness.SemiDark };

        public static Graph LoadGraph()
        {
            Graph g = new();

            g.Clusters[LegacyCluster.BlackEggRadiance] = new() {
                Scenes = new() { { SceneName.EggRadiance, new() } },
                AdjacentClusters = new() { { LegacyCluster.BlackEggTemple, RelativeDarkness.Darker } },
                CursedOnly = true,
                CostWeight = 300 };

            g.Clusters[LegacyCluster.BlackEggTemple] = new() {
                Scenes = new() {
                    { SceneName.EggBench, SemiDark },
                    { SceneName.BlackEggTemple, SemiDark },
                    { SceneName.EggHollowKnight, new() } },
                CursedOnly = true,
                CostWeight = 300 };

            g.Clusters[LegacyCluster.BlueLake] = new() {
                Scenes = new() {
                    { SceneName.GroundsBlueLake, new() },
                    { SceneName.GroundsCorridorBelowXero, new() } },
                AdjacentClusters = new() {
                    { LegacyCluster.CrossroadsLower, RelativeDarkness.Any },
                    // FIXME: EastCityElevator
                },
                ProbabilityWeight = 80,
                CostWeight = 60 };

            g.Clusters[LegacyCluster.CityAboveLemm] = new()
            {
                Scenes = new()
                {
                    { SceneName.CityEggAboveLemm, new() },
                    { SceneName.CityGrubAboveLemm, new() },
                    { SceneName.CityTollBench, new() } },
                AdjacentClusters = new()
                {
                    { LegacyCluster.CityFountain, RelativeDarkness.Any },
                },
                OverrideIsDarknessSource = false,
                ProbabilityWeight = 25,
                CostWeight = 200
            };



            g.Clusters[LegacyCluster.CityElegantWarrior] = new()
            {
                Scenes = new()
                {
                    { SceneName.CityShadeSoulArena, new() }
                },
                ProbabilityWeight = 150
            };

            g.Clusters[LegacyCluster.CityFountain] = new()
            {
                Scenes = new()
                {
                    { SceneName.CityHollowKnightFountain, SemiDark },
                    { SceneName.CityCorridortoSpire, SemiDark }
                }
            };

            g.Clusters[LegacyCluster.CliffsBaldur] = new() {
                Scenes = new() { { SceneName.CliffsBaldursShell, new() } },
                AdjacentClusters = new() {
                    { LegacyCluster.CliffsMain, RelativeDarkness.Darker },
                    { LegacyCluster.GreenpathWest, RelativeDarkness.Darker } },
                ProbabilityWeight = 80 };

            g.Clusters[LegacyCluster.CliffsJonis] = new() {
                Scenes = new() { { SceneName.CliffsJonisDark, new() } },
                AdjacentClusters = new() { { LegacyCluster.CliffsMain, RelativeDarkness.Brighter } },
                ProbabilityWeight = 80 };

            g.Clusters[LegacyCluster.CliffsMain] = new() {
                Scenes = new() {
                    { SceneName.CliffsMain, new() },
                    { SceneName.CliffsGrimmLantern, SemiDark },
                    { SceneName.CliffsGorb, new() },
                    { SceneName.CliffsMato, SemiDark },
                    { SceneName.CliffsStagNest, SemiDark } },
                AdjacentClusters = new() { { LegacyCluster.KingsPass, RelativeDarkness.Any } },
                ProbabilityWeight = 60,
                CostWeight = 250,
                SemiDarkProbabilty = 40 };

            g.Clusters[LegacyCluster.CrossroadsAncestralMound] = new() {
                Scenes = new() {
                    { SceneName.CrossroadsOutsideMound, SemiDark },
                    { SceneName.CrossroadsAncestralMound, new() } },
                AdjacentClusters = new() {
                    { LegacyCluster.CrossroadsWest, RelativeDarkness.Brighter },
                    { LegacyCluster.CrossroadsFalseKnight, RelativeDarkness.Any } },
                ProbabilityWeight = 60,
                CostWeight = 120 };

            g.Clusters[LegacyCluster.CrossroadsCanyonBridge] = new() {
                Scenes = new() {
                    { SceneName.CrossroadsAcidGrub, new() },
                    { SceneName.CrossroadsCorridortoAcidGrub, new() } },
                AdjacentClusters = new() { { LegacyCluster.FogCanyonEast, RelativeDarkness.Darker } },
                ProbabilityWeight = 40,
                CostWeight = 60 };

            g.Clusters[LegacyCluster.CrossroadsEntrance] = new() {
                Scenes = new() {
                    { SceneName.CrossroadsWell, new() },
                    { SceneName.CrossroadsOutsideTemple, SemiDark } },
                AdjacentClusters = new() {
                    { LegacyCluster.CrossroadsUpper, RelativeDarkness.Darker },
                    { LegacyCluster.CrossroadsWest, RelativeDarkness.Darker } },
                ProbabilityWeight = 5,
                CostWeight = 25 };

            g.Clusters[LegacyCluster.CrossroadsFailedChamp] = new() {
                Scenes = new() { { SceneName.DreamFailedChampion, new() { ProficientCombatLocs = LocationSet.All() } } },
                AdjacentClusters = new() { { LegacyCluster.CrossroadsFalseKnight, RelativeDarkness.Darker } },
                CursedOnly = true,
                ProbabilityWeight = 80,
                CostWeight = 200 };

            g.Clusters[LegacyCluster.CrossroadsFalseKnight] = new() {
                Scenes = new() {
                    { SceneName.CrossroadsFalseKnightArena, new() },
                    { SceneName.CrossroadsOutsideFalseKnight, new() } },
                AdjacentClusters = new() { { LegacyCluster.CrossroadsAncestralMound, RelativeDarkness.Any } },
                ProbabilityWeight = 120 };

            g.Clusters[LegacyCluster.CrossroadsGlowingWomb] = new() {
                Scenes = new() { { SceneName.CrossroadsGlowingWombArena, new() } },
                AdjacentClusters = new() { { LegacyCluster.CrossroadsFalseKnight, RelativeDarkness.Brighter } } };

            g.Clusters[LegacyCluster.CrossroadsGreenpathBridge] = new() {
                Scenes = new() { { SceneName.CrossroadsGreenpathEntrance, new() } },
                AdjacentClusters = new() {
                    { LegacyCluster.CrossroadsWest, RelativeDarkness.Any },
                    { LegacyCluster.GreenpathEntrance, RelativeDarkness.Any } },
                OverrideIsDarknessSource = false,
                ProbabilityWeight = 15,
                CostWeight = 60 };

            g.Clusters[LegacyCluster.CrossroadsFungalBridge] = new()
            {
                Scenes = new()
                {
                    { SceneName.CrossroadsGoamJournal, SemiDark },
                    { SceneName.CrossroadsFungalEntrance, SemiDark }
                },
                AdjacentClusters = new()
                {
                    { LegacyCluster.CrossroadsWest, RelativeDarkness.Any },
                    { LegacyCluster.FungalUpper, RelativeDarkness.Any }
                }
            };

            g.Clusters[LegacyCluster.CrossroadsLower] = new() {
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
                    { LegacyCluster.CrossroadsSpikeGrub, RelativeDarkness.Darker },
                    { LegacyCluster.WestCityElevator, RelativeDarkness.Darker },
                    { LegacyCluster.CrossroadsFalseKnight, RelativeDarkness.Any },
                    { LegacyCluster.CrossroadsPeaksToll, RelativeDarkness.Any },
                    { LegacyCluster.CrossroadsUpper, RelativeDarkness.Any } },
                ProbabilityWeight = 15,
                CostWeight = 200 };

            g.Clusters[LegacyCluster.CrossroadsMawlek] = new() {
                Scenes = new() {
                    { SceneName.CrossroadsMawlekEntrance, new() },
                    { SceneName.CrossroadsMawlekMiddle, new() },
                    { SceneName.CrossroadsMawlekBoss, new() { ProficientCombatLocs = LocationSet.All() } } },
                AdjacentClusters = new() {
                    { LegacyCluster.CrossroadsWest, RelativeDarkness.Brighter } },
                ProbabilityWeight = 30,
                CostWeight = 150 };

            g.Clusters[LegacyCluster.CrossroadsPeaksBridge] = new() {
                Scenes = new() {
                    { SceneName.CrossroadsMyla, SemiDark },
                    { SceneName.CrystalDiveEntrance, SemiDark } },
                AdjacentClusters = new() { { LegacyCluster.CrossroadsUpper, RelativeDarkness.Darker } } };

            g.Clusters[LegacyCluster.CrossroadsPeaksToll] = new() {
                Scenes = new() { { SceneName.CrossroadsPeakDarkToll, new() { MinimumDarkness = Darkness.SemiDark } } },
                AdjacentClusters = new() { { LegacyCluster.CrossroadsLower, RelativeDarkness.Any } },
                ProbabilityWeight = 500,
                CostWeight = 50 };

            g.Clusters[LegacyCluster.CrossroadsSpikeGrub] = new() {
                Scenes = new() { { SceneName.CrossroadsSpikeGrub, new() } },
                ProbabilityWeight = 40,
                CostWeight = 40 };

            g.Clusters[LegacyCluster.CrossroadsUpper] = new() {
                Scenes = new() {
                    { SceneName.CrossroadsCorridorRightofTemple, new() },
                    { SceneName.CrossroadsGuardedGrub, new() },
                    { SceneName.CrossroadsOutsideMyla, new() },
                    { SceneName.CrossroadsAboveLever, new() },
                    { SceneName.CrossroadsCorridorRightofCentralGrub, new() } },
                AdjacentClusters = new() {
                    { LegacyCluster.CrossroadsWest, RelativeDarkness.Any },
                    { LegacyCluster.CrossroadsLower, RelativeDarkness.Any },
                    { LegacyCluster.CrossroadsPeaksBridge, RelativeDarkness.Any } },
                ProbabilityWeight = 15,
                CostWeight = 200 };

            g.Clusters[LegacyCluster.CrossroadsWest] = new() {
                Scenes = new()
                {
                    { SceneName.CrossroadsCentralGrub, new() },
                    { SceneName.CrossroadsGruzzer, new() },
                    { SceneName.CrossroadsCornifer, new() },
                    { SceneName.CrossroadsAspidArena, new() },
                    { SceneName.CrossroadsGoamMaskShard, new() }
                },
                ProbabilityWeight = 5,
                CostWeight = 120 };

            g.Clusters[LegacyCluster.CrystalPeakCrown] = new() {
                Scenes = new() {
                    { SceneName.CrystalCrownWhisperingRoot, new() },
                    { SceneName.CrystalCrownGrub, new() },
                    { SceneName.CrystalCrownClimb, new() },
                    { SceneName.CrystalCrownPeak, new() } },
                AdjacentClusters = new() { { LegacyCluster.CrystalPeakUpper, RelativeDarkness.Brighter } },
                ProbabilityWeight = 150,
                CostWeight = 200 };

            g.Clusters[LegacyCluster.CrystalPeakCrystallizedMound] = new() {
                Scenes = new() { { SceneName.CrystalOutsideMound, SemiDark }, { SceneName.CrystalMound, new() } },
                ProbabilityWeight = 125 };

            g.Clusters[LegacyCluster.CrystalPeakDarkRoom] = new() {
                Scenes = new() { { SceneName.CrystalDarkRoom, new() } },
                AdjacentClusters = new() { { LegacyCluster.CrystalPeakCrystallizedMound, RelativeDarkness.Any } },
                ProbabilityWeight = 250,
                CostWeight = 250 };

            g.Clusters[LegacyCluster.CrystalPeakDeepFocus] = new() {
                Scenes = new() {
                    { SceneName.CrystalDeepFocus, SemiDark },
                    { SceneName.CrystalDeepFocusGauntlet, new() { ProficientCombatLocs = LocationSet.All() } } },
                AdjacentClusters = new() { { LegacyCluster.CrystalPeakWest, RelativeDarkness.Any } },
                ProbabilityWeight = 150 };

            g.Clusters[LegacyCluster.CrystalPeakDirtmouthBridge] = new() {
                Scenes = new() {
                    { SceneName.CrystalCornifer, SemiDark },
                    { SceneName.CrystalMimic, new() },
                    { SceneName.CrystalElevatorEntrance, new() } } };

            g.Clusters[LegacyCluster.CrystalPeakGuardian] = new() {
                Scenes = new() {
                    { SceneName.CrystalGuardianBench, new() },
                    { SceneName.CrystalEnragedGuardianArena, new() { ProficientCombatLocs = LocationSet.All() } } },
                CostWeight = 200 };

            g.Clusters[LegacyCluster.CrystalPeakHeart] = new() {
                Scenes = new() { { SceneName.CrystalCrystalHeartGauntlet, new() { DifficultSkipLocs = LocationSet.All() } } },
                AdjacentClusters = new() { { LegacyCluster.CrystalPeakUpper, RelativeDarkness.Brighter } },
                ProbabilityWeight = 150,
                CostWeight = 200 };

            g.Clusters[LegacyCluster.CrystalPeakLower] = new() {
                Scenes = new() {
                    { SceneName.CrystalEntranceConveyors, new() },
                    { SceneName.CrystalDarkBench, new() },
                    { SceneName.CrystalMainEntrance, new() },
                    { SceneName.CrystalCorridortoSpikeGrub, new() },
                    { SceneName.CrystalChestCrushers, new() } },
                AdjacentClusters = new() {
                    { LegacyCluster.CrystalPeakDarkRoom, RelativeDarkness.Any },
                    { LegacyCluster.CrossroadsPeaksBridge, RelativeDarkness.Any },
                    { LegacyCluster.CrossroadsPeaksToll, RelativeDarkness.Any },
                    { LegacyCluster.CrystalPeakUpper, RelativeDarkness.Darker } },
                ProbabilityWeight = 15,
                CostWeight = 200 };

            g.Clusters[LegacyCluster.CrystalPeakUpper] = new() {
                Scenes = new() {
                    { SceneName.CrystalEastTall, new() },
                    { SceneName.CrystalTopCorridor, SemiDark } },
                AdjacentClusters = new() {
                    { LegacyCluster.CrystalPeakHeart, RelativeDarkness.Brighter },
                    { LegacyCluster.CrystalPeakGuardian, RelativeDarkness.Any },
                    { LegacyCluster.CrystalPeakWest, RelativeDarkness.Any },
                    { LegacyCluster.CrystalPeakLower, RelativeDarkness.Any } },
                ProbabilityWeight = 50,
                CostWeight = 150 };

            g.Clusters[LegacyCluster.CrystalPeakWest] = new() {
                Scenes = new() {
                    { SceneName.CrystalLeftOfGuardian, new() },
                    { SceneName.CrystalSpikeGrub, new() },
                    { SceneName.CrystalAboveSpikeGrub, new() } },
                AdjacentClusters = new() {
                    { LegacyCluster.CrystalPeakDirtmouthBridge, RelativeDarkness.Any },
                    { LegacyCluster.CrystalPeakGuardian, RelativeDarkness.Darker },
                    { LegacyCluster.CrystalPeakLower, RelativeDarkness.Brighter } },
                ProbabilityWeight = 50,
                CostWeight = 200 };

            g.Clusters[LegacyCluster.DirtmouthGPZ] = new() {
                Scenes = new() {
                    { SceneName.Bretta, new() {
                        MaximumDarkness = Darkness.SemiDark,
                        ProficientCombatLocs = new("Boss_Essence-Grey_Prince_Zote") } },
                    { SceneName.BrettaBasement, SemiDark },
                    { SceneName.GPZ, new() } },
                CursedOnly = true,
                ProbabilityWeight = 40,
                CostWeight = 200 };

            g.Clusters[LegacyCluster.DirtmouthGrimm] = new() {
                Scenes = new() {
                    { SceneName.GrimmTent, new() { ProficientCombatLocs = LocationSet.All() } },
                    { SceneName.GrimmNKG, new() } },
                CursedOnly = true,
                ProbabilityWeight = 50,
                CostWeight = 200 };

            g.Clusters[LegacyCluster.FogCanyonArchives] = new() {
                Scenes = new() {
                    { SceneName.FogArchivesBench, SemiDark },
                    { SceneName.FogUumuuArena, new() { ProficientCombatLocs = LocationSet.All() } } },
                AdjacentClusters = new() { { LegacyCluster.FogCanyonEast, RelativeDarkness.Any } },
                CostWeight = 150 };

            g.Clusters[LegacyCluster.FogCanyonEast] = new() {
                Scenes = new() {
                    { SceneName.FogCorridortoCornifer, new() },
                    { SceneName.FogCorridortoArchives, new() },
                    { SceneName.FogEastTall, new() } },
                AdjacentClusters = new() { { LegacyCluster.FogCanyonWest, RelativeDarkness.Any } },
                ProbabilityWeight = 50,
                CostWeight = 200 };

            g.Clusters[LegacyCluster.FogCanyonOvergrownMound] = new() {
                Scenes = new() {
                    { SceneName.FogOvergrownMoundEntrance, new() },
                    { SceneName.FogOvergrownMound, new() } },
                CostWeight = 60 };

            g.Clusters[LegacyCluster.FogCanyonNotch] = new() {
                Scenes = new() { { SceneName.FogCharmNotch, new() { DifficultSkipLocs = LocationSet.All() } } },
                AdjacentClusters = new() { { LegacyCluster.FogCanyonEast, RelativeDarkness.Brighter } } };

            g.Clusters[LegacyCluster.FogCanyonWest] = new() {
                Scenes = new() {
                    { SceneName.FogCornifer, new() },
                    { SceneName.FogUpperWestTall, new() },
                    { SceneName.FogLowerWestTall, new() },
                    { SceneName.FogCorridortoOvergrownMound, new() },
                    { SceneName.FogLifeblood, SemiDark },
                    { SceneName.FogMillibelle, SemiDark } },
                AdjacentClusters = new() {
                    { LegacyCluster.GreenpathOutsideNoEyes, RelativeDarkness.Any },
                    { LegacyCluster.FogCanyonOvergrownMound, RelativeDarkness.Darker } },
                ProbabilityWeight = 15,
                CostWeight = 300 };

            g.Clusters[LegacyCluster.FungalCore] = new() {
                Scenes = new() { { SceneName.FungalCoreUpper, new() }, { SceneName.FungalCoreLower, new() } },
                AdjacentClusters = new() { { LegacyCluster.FungalLowerHub, RelativeDarkness.Brighter } } };

            g.Clusters[LegacyCluster.FungalCorniferHub] = new()
            {
                Scenes = new() { { SceneName.FungalCornifer, new() } }
            };

            g.Clusters[LegacyCluster.FungalElderHuWing] = new()
            {
                Scenes = new()
                {
                    { SceneName.FungalElderHu, new() },
                    { SceneName.FungalOutsideElderHu, new() },
                    { SceneName.FungalClothCorridor, SemiDark },
                    { SceneName.FungalShrumalWarriorAcidBridge, new() }
                },
                AdjacentClusters = new()
                {
                    { LegacyCluster.FungalUpper, RelativeDarkness.Brighter },
                    { LegacyCluster.FungalPilgrimsWay, RelativeDarkness.Any }
                },
                ProbabilityWeight = 50,
                CostWeight = 150
            };

            g.Clusters[LegacyCluster.FungalEntrance] = new()
            {
                Scenes = new()
                {
                    { SceneName.FungalOutsideQueens, new() },
                    { SceneName.FungalShrumalWarriorLoop, new() },
                    { SceneName.FungalBelowOgres, new() },
                },
                AdjacentClusters = new()
                {
                    { LegacyCluster.FungalShroomals, RelativeDarkness.Darker },
                    { LegacyCluster.FungalCorniferHub, RelativeDarkness.Brighter }
                },
                ProbabilityWeight = 25
            };

            g.Clusters[LegacyCluster.FungalLowerHub] = new()
            {
                Scenes = new()
                {
                    { SceneName.FungalEpogo, new() },
                    { SceneName.FungalAboveMantisVillage, new() }
                },
                AdjacentClusters = new()
                {
                    { LegacyCluster.FungalPilgrimsWay, RelativeDarkness.Any },
                    { LegacyCluster.FungalCore, RelativeDarkness.Darker },
                    { LegacyCluster.FungalMantisVillage, RelativeDarkness.Any }
                },
                ProbabilityWeight = 25
            };

            g.Clusters[LegacyCluster.FungalMantisLords] = new()
            {
                Scenes = new()
                {
                    { SceneName.FungalMantisLords, new() },
                    { SceneName.FungalMantisRewards, SemiDark }
                },
                AdjacentClusters = new()
                {
                    { LegacyCluster.FungalMantisVillage, RelativeDarkness.Brighter }
                },
                CostWeight = 250
            };

            g.Clusters[LegacyCluster.FungalMantisVillage] = new()
            {
                Scenes = new()
                {
                    { SceneName.FungalMantisVillage, new() },
                    { SceneName.FungalMantisCorridor, new() },
                    { SceneName.FungalBrettaBench, new() },
                    { SceneName.FungalDashmaster, new() }
                },
                AdjacentClusters = new()
                {
                    { LegacyCluster.FungalMantisLords, RelativeDarkness.Darker },
                    { LegacyCluster.FungalLowerHub, RelativeDarkness.Any }
                },
                ProbabilityWeight = 40,
                CostWeight = 200 };

            g.Clusters[LegacyCluster.FungalQueenStation] = new()
            {
                Scenes = new()
                {
                    { SceneName.FungalQueensStation, new() },
                    { SceneName.FungalQueensStag, SemiDark },
                    { SceneName.FungalWilloh, SemiDark }
                },
                AdjacentClusters = new()
                {
                    { LegacyCluster.FogCanyonWest, RelativeDarkness.Any },
                    { LegacyCluster.FungalEntrance, RelativeDarkness.Any }
                },
                OverrideIsDarknessSource = false,
                ProbabilityWeight = 50,
                CostWeight = 50
            };

            g.Clusters[LegacyCluster.FungalPilgrimsWay] = new()
            {
                Scenes = new()
                {
                    { SceneName.FungalPilgrimsWay, new() },
                    { SceneName.FungalLeftOfPilgrimsWay, new() }
                },
                ProbabilityWeight = 25 };

            g.Clusters[LegacyCluster.FungalShroomals] = new()
            {
                Scenes = new() { { SceneName.FungalShrumalOgres, new() } },
                AdjacentClusters = new()
                {
                    { LegacyCluster.FungalEntrance, RelativeDarkness.Brighter },
                    { LegacyCluster.FungalUpper, RelativeDarkness.Brighter }
                },
                ProbabilityWeight = 80,
                CostWeight = 120
            };

            g.Clusters[LegacyCluster.FungalSporeShroom] = new() {
                Scenes = new() {
                    { SceneName.FungalSporeShroom, new() },
                    { SceneName.FungalRightOfSporeShroom, new() },
                    { SceneName.FungalDeepnestFall, SemiDark } },
                AdjacentClusters = new() {
                    { LegacyCluster.FungalCorniferHub, RelativeDarkness.Brighter } },
                ProbabilityWeight = 80,
                CostWeight = 150 };

            g.Clusters[LegacyCluster.FungalUpper] = new()
            {
                Scenes = new()
                {
                    { SceneName.FungalOutsideLegEater, new() },
                    { SceneName.FungalLegEater, SemiDark },
                    { SceneName.FungalLegEaterRoot, new() } },
                AdjacentClusters = new()
                {
                    { LegacyCluster.CrossroadsFungalBridge, RelativeDarkness.Any },
                    { LegacyCluster.FogCanyonEast, RelativeDarkness.Any },
                    { LegacyCluster.FungalElderHuWing, RelativeDarkness.Darker }
                },
                ProbabilityWeight = 10,
                CostWeight = 200 };

            g.Clusters[LegacyCluster.GreenpathCliffsBridge] = new() {
                Scenes = new() {
                    { SceneName.GreenpathVengeflyKing, new() },
                    { SceneName.GreenpathMossKnightArena, new() } },
                AdjacentClusters = new() {
                    { LegacyCluster.GreenpathWest, RelativeDarkness.Darker },
                    { LegacyCluster.CliffsMain, RelativeDarkness.Any } } };

            g.Clusters[LegacyCluster.GreenpathEntrance] = new() {
                Scenes = new() {
                    { SceneName.GreenpathWaterfallBench, SemiDark },
                    { SceneName.GreenpathEntrance, new() } },
                AdjacentClusters = new() { { LegacyCluster.GreenpathUpper, RelativeDarkness.Darker } },
                ProbabilityWeight = 25,
                CostWeight = 50 };

            g.Clusters[LegacyCluster.GreenpathHornet] = new() {
                Scenes = new() { { SceneName.GreenpathHornet, new() } },
                ProbabilityWeight = 120,
                CostWeight = 80 };

            g.Clusters[LegacyCluster.GreenpathLower] = new() {
                Scenes = new() {
                    { SceneName.GreenpathAcidBridge, new() },
                    { SceneName.GreenpathAboveSanctuaryBench, new() },
                    { SceneName.GreenpathOutsideHunter, new() },
                    { SceneName.GreenpathHunter, SemiDark } },
                AdjacentClusters = new() { { LegacyCluster.GreenpathOutsideNoEyes, RelativeDarkness.Any } },
                ProbabilityWeight = 15,
                CostWeight = 200,
                SemiDarkProbabilty = 80 };

            g.Clusters[LegacyCluster.GreenpathMMC] = new() {
                Scenes = new() {
                    { SceneName.GreenpathMassiveMossCharger, new() },
                    { SceneName.GreenpathMMCCorridor, new() } },
                AdjacentClusters = new() { { LegacyCluster.GreenpathOutsideNoEyes, RelativeDarkness.Brighter } },
                ProbabilityWeight = 80,
                CostWeight = 120 };

            g.Clusters[LegacyCluster.GreenpathNoEyes] = new() {
                Scenes = new() { { SceneName.GreenpathStoneSanctuary, new() } } };

            g.Clusters[LegacyCluster.GreenpathOutsideNoEyes] = new() {
                Scenes = new() {
                    { SceneName.GreenpathAboveFogCanyon, new() },
                    { SceneName.GreenpathSanctuaryBench, SemiDark },
                    { SceneName.GreenpathStoneSanctuaryEntrance, new() { MinimumDarkness = Darkness.SemiDark } } },
                AdjacentClusters = new() { { LegacyCluster.GreenpathNoEyes, RelativeDarkness.Any } },
                CostWeight = 140 };

            g.Clusters[LegacyCluster.GreenpathSheo] = new() {
                Scenes = new() {
                    {
                        SceneName.GreenpathSheoGauntlet,
                        new() { DifficultSkipLocs = new("Fungus1_09[left1]") }
                    },
                    { SceneName.GreenpathOutsideSheo, new() },
                    { SceneName.GreenpathSheo, SemiDark } },
                ProbabilityWeight = 80,
                CostWeight = 200 };

            g.Clusters[LegacyCluster.GreenpathThorns] = new() {
                Scenes = new() { { SceneName.GreenpathThornsofAgony, new() } },
                CostWeight = 80 };

            g.Clusters[LegacyCluster.GreenpathUnn] = new() {
                Scenes = new() {
                    { SceneName.GreenpathLakeOfUnn, new() },
                    { SceneName.GreenpathUnn, new() },
                    { SceneName.GreenpathUnnBench, new() { MaximumDarkness = Darkness.SemiDark} } },
                ProbabilityWeight = 80,
                CostWeight = 50 };

            g.Clusters[LegacyCluster.GreenpathUnnPass] = new() {
                Scenes = new() {
                    { SceneName.GreenpathCorridortoUnn, SemiDark } },
                AdjacentClusters = new() {
                    { LegacyCluster.GreenpathUnn, RelativeDarkness.Darker },
                    { LegacyCluster.GreenpathHornet, RelativeDarkness.Darker } } };

            g.Clusters[LegacyCluster.GreenpathUpper] = new() {
                Scenes = new() {
                    { SceneName.GreenpathOutsideThorns, new() },
                    { SceneName.GreenpathToll, new() },
                    { SceneName.GreenpathStorerooms, new() },
                    { SceneName.GreenpathFirstHornetSighting, new() },
                    { SceneName.GreenpathCornifer, new() },
                    { SceneName.GreenpathChargerCorridor, new() } },
                AdjacentClusters = new() {
                    { LegacyCluster.GreenpathCliffsBridge, RelativeDarkness.Any },
                    { LegacyCluster.GreenpathLower, RelativeDarkness.Any },
                    { LegacyCluster.GreenpathThorns, RelativeDarkness.Darker } },
                ProbabilityWeight = 15,
                CostWeight = 300,
                SemiDarkProbabilty = 80 };

            g.Clusters[LegacyCluster.GreenpathWest] = new() {
                Scenes = new() {
                    { SceneName.GreenpathOutsideHornet, new() },
                    { SceneName.GreenpathOutsideStag, new() },
                    { SceneName.GreenpathStag, SemiDark },
                    { SceneName.GreenpathBelowTollBench, new() } },
                AdjacentClusters = new() {
                    { LegacyCluster.GreenpathHornet, RelativeDarkness.Darker },
                    { LegacyCluster.GreenpathSheo, RelativeDarkness.Any } },
                ProbabilityWeight = 60,
                CostWeight = 200,
                SemiDarkProbabilty = 80 };

            g.Clusters[LegacyCluster.KingsPass] = new() {
                Scenes = new() { { SceneName.KingsPass, new() } },
                CostWeight = 50 };

            g.Clusters[LegacyCluster.RestingGroundsCatacombs] = new() {
                Scenes = new() {
                    { SceneName.GroundsCrypts, new() },
                    { SceneName.GroundsOutsideGreyMourner, SemiDark } },
                // FIXME: EastCityElevator
            };

            g.Clusters[LegacyCluster.RestingGroundsDreamNail] = new() {
                Scenes = new() {
                    { SceneName.DreamNail, new() },
                    { SceneName.GroundsDreamNailEntrance, SemiDark } },
                AdjacentClusters = new() { { LegacyCluster.RestingGroundsMain, RelativeDarkness.Brighter } },
                ProbabilityWeight = 150,
                CostWeight = 25 };

            g.Clusters[LegacyCluster.RestingGroundsMain] = new() {
                Scenes = new() {
                    { SceneName.GroundsWhisperingRoot, SemiDark },
                    { SceneName.GroundsDreamshield, SemiDark },
                    { SceneName.GroundsSpiritsGlade, SemiDark } },
                AdjacentClusters = new() { { LegacyCluster.RestingGroundsCatacombs, RelativeDarkness.Darker } } };

            g.Clusters[LegacyCluster.RestingGroundsXero] = new() {
                Scenes = new() { { SceneName.GroundsXero, new() } },
                AdjacentClusters = new() {
                    { LegacyCluster.RestingGroundsDreamNail, RelativeDarkness.Any },
                    { LegacyCluster.BlueLake, RelativeDarkness.Any } } };

            g.Clusters[LegacyCluster.WestCityElevator] = new() {
                Scenes = new() {
                    { SceneName.CrossroadsCorridortoElevator, SemiDark },
                    { SceneName.CrossroadsElevator, SemiDark },
                    { SceneName.CityLeftElevator, new() } },
                AdjacentClusters = new() {
                    { LegacyCluster.CrossroadsLower, RelativeDarkness.Any } },
                ProbabilityWeight = 50,
                CostWeight = 200 };

            g.Init();
            return g;
        }
    }
}
