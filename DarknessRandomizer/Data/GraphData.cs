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
        public static Graph LoadGraph()
        {
            Graph g = new();

            g.Clusters[Cluster.CliffsBaldur] = new() {
                Scenes = new() { { Scenes.CliffsBaldursShell, new() } },
                AdjacentClusters = new() {
                    { Cluster.CliffsMain, ClusterRelativity.Darkwards },
                    { Cluster.GreenpathLeft, ClusterRelativity.Darkwards } },
                ProbabilityWeight = 80 };

            g.Clusters[Cluster.CliffsJonis] = new() {
                Scenes = new() { { Scenes.CliffsJonisDark, new() } },
                AdjacentClusters = new() { { Cluster.CliffsMain, ClusterRelativity.Brightwards } },
                ProbabilityWeight = 80 };

            g.Clusters[Cluster.CliffsMain] = new() {
                Scenes = new() {
                    { Scenes.CliffsMain, new() },
                    { Scenes.CliffsGrimmLantern, new() { MaximumDarkness = Darkness.SemiDark } },
                    { Scenes.CliffsGorb, new() },
                    { Scenes.CliffsMato, new() { MaximumDarkness = Darkness.SemiDark } },
                    { Scenes.CliffsStagNest, new() { MaximumDarkness = Darkness.SemiDark } } },
                AdjacentClusters = new() { { Cluster.KingsPass, ClusterRelativity.None } },
                ProbabilityWeight = 60,
                CostWeight = 250,
                SemiDarkProbabilty = 40 };

            g.Clusters[Cluster.GreenpathCliffsBridge] = new() {
                Scenes = new() {
                    { Scenes.GreenpathVengeflyKing, new() },
                    { Scenes.GreenpathMossKnightArena, new() } },
                AdjacentClusters = new() {
                    { Cluster.GreenpathLeft, ClusterRelativity.Darkwards },
                    { Cluster.CliffsMain, ClusterRelativity.None } } };

            g.Clusters[Cluster.GreenpathHornet] = new() {
                Scenes = new() { { Scenes.GreenpathHornet, new() } },
                ProbabilityWeight = 120,
                CostWeight = 80 };

            g.Clusters[Cluster.GreenpathLeft] = new() {
                Scenes = new() {
                    { Scenes.GreenpathOutsideHornet, new() },
                    { Scenes.GreenpathOutsideStag, new() },
                    { Scenes.GreenpathStag, new() { MaximumDarkness = Darkness.SemiDark } },
                    { Scenes.GreenpathBelowTollBench, new() } },
                AdjacentClusters = new() {
                    { Cluster.GreenpathHornet, ClusterRelativity.Darkwards },
                    { Cluster.GreenpathSheo, ClusterRelativity.None } },
                ProbabilityWeight = 60,
                CostWeight = 200,
                SemiDarkProbabilty = 80 };

            g.Clusters[Cluster.GreenpathLower] = new() {
                Scenes = new() {
                    { Scenes.GreenpathChargerCorridor, new() },
                    { Scenes.GreenpathAboveSanctuaryBench, new() },
                    { Scenes.GreenpathOutsideHunter, new() },
                    { Scenes.GreenpathHunter, new() { MaximumDarkness = Darkness.SemiDark } } },
                AdjacentClusters = new() { { Cluster.GreenpathOutsideNoEyes, ClusterRelativity.None } },
                ProbabilityWeight = 60,
                CostWeight = 200,
                SemiDarkProbabilty = 80 };

            g.Clusters[Cluster.GreenpathMMC] = new() {
                Scenes = new() {
                    { Scenes.GreenpathMassiveMossCharger, new() },
                    { Scenes.GreenpathMMCCorridor, new() } },
                AdjacentClusters = new() { { Cluster.GreenpathOutsideNoEyes, ClusterRelativity.Brightwards } },
                ProbabilityWeight = 80,
                CostWeight = 120 };

            g.Clusters[Cluster.GreenpathNoEyes] = new() {
                Scenes = new() { { Scenes.GreenpathStoneSanctuary, new() } } };

            g.Clusters[Cluster.GreenpathOutsideNoEyes] = new() {
                Scenes = new() {
                    { Scenes.GreenpathAboveFogCanyon, new() },
                    { Scenes.GreenpathSanctuaryBench, new() { MaximumDarkness = Darkness.SemiDark } },
                    { Scenes.GreenpathStoneSanctuaryEntrance, new() { MinimumDarkness = Darkness.SemiDark } } },
                AdjacentClusters = new() { { Cluster.GreenpathNoEyes, ClusterRelativity.None } },
                CostWeight = 140 };

            g.Clusters[Cluster.GreenpathSheo] = new() {
                Scenes = new() {
                    {
                        Scenes.GreenpathSheoGauntlet,
                        new() { DifficultSkipLocs = new LocationsList("Fungus1_09[left1]") }
                    },
                    { Scenes.GreenpathOutsideSheo, new() },
                    { Scenes.GreenpathSheo, new() { MaximumDarkness = Darkness.SemiDark } } },
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
                    { Scenes.GreenpathCorridortoUnn, new() { MaximumDarkness = Darkness.SemiDark } } },
                AdjacentClusters = new() {
                    { Cluster.GreenpathUnn, ClusterRelativity.Darkwards },
                    { Cluster.GreenpathHornet, ClusterRelativity.Darkwards } } };

            g.Clusters[Cluster.GreenpathUpper] = new() {
                Scenes = new() {
                    { Scenes.GreenpathOutsideThorns, new() },
                    { Scenes.GreenpathToll, new() },
                    { Scenes.GreenpathStorerooms, new() },
                    { Scenes.GreenpathFirstHornetSighting, new() },
                    { Scenes.GreenpathCornifer, new() },
                    { Scenes.GreenpathChargerCorridor, new() } },
                AdjacentClusters = new() {
                    { Cluster.GreenpathCliffsBridge, ClusterRelativity.None },
                    { Cluster.GreenpathLower, ClusterRelativity.None },
                    { Cluster.GreenpathThorns, ClusterRelativity.Darkwards } },
                ProbabilityWeight = 20,
                CostWeight = 300,
                SemiDarkProbabilty = 30 };

            g.Clusters[Cluster.KingsPass] = new() {
                Scenes = new() { { Scenes.KingsPass, new() } },
                CostWeight = 50 };

            g.Init();
            return g;
        }
    }
}
