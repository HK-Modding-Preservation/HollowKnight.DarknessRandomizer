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

            g.Clusters[Cluster.GreenpathLeft] = new() {
                Scenes = new() {
                    { Scenes.GreenpathOutsideHornet, new() },
                    { Scenes.GreenpathOutsideStag, new() },
                    { Scenes.GreenpathStag, new() { MaximumDarkness = Darkness.SemiDark } },
                    { Scenes.GreenpathBelowTollBench, new() } },
                AdjacentClusters = new() {
                    { Cluster.Hornet, ClusterRelativity.LevelOrDarker },
                    { Cluster.Sheo, ClusterRelativity.DarkerOrBrighter } },
                ProbabilityWeight = 60,
                CostWeight = 200 };

            g.Clusters[Cluster.Hornet] = new() {
                Scenes = new() { { Scenes.GreenpathHornet, new() } },
                ProbabilityWeight = 120,
                CostWeight = 80 };

            g.Clusters[Cluster.Sheo] = new() {
                Scenes = new() {
                    {
                        Scenes.GreenpathSheoGauntlet,
                        new() { DifficultSkipLocs = new LocationsList("Fungus1_09[left1]") }
                    },
                    { Scenes.GreenpathOutsideSheo, new() },
                    { Scenes.GreenpathSheo, new() { MaximumDarkness = Darkness.SemiDark } } },
                ProbabilityWeight = 80,
                CostWeight = 200 };

            g.Clusters[Cluster.Unn] = new() {
                Scenes = new() {
                    { Scenes.GreenpathLakeOfUnn, new() },
                    { Scenes.GreenpathUnn, new() },
                    { Scenes.GreenpathUnnBench, new() { MaximumDarkness = Darkness.SemiDark} } },
                ProbabilityWeight = 80,
                CostWeight = 50 };

            g.Clusters[Cluster.UnnPass] = new() {
                Scenes = new() {
                    { Scenes.GreenpathCorridortoUnn, new() { MaximumDarkness = Darkness.SemiDark } } },
                AdjacentClusters = new() {
                    { Cluster.Unn, ClusterRelativity.LevelOrDarker },
                    { Cluster.Hornet, ClusterRelativity.LevelOrDarker } } };

            g.Init();
            return g;
        }
    }
}
