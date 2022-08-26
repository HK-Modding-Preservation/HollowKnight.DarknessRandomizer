using DarknessRandomizer.Rando;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Lib
{
    public class Algorithm
    {
        private readonly Random r;
        private readonly DarknessRandomizationSettings settings;
        private readonly Graph g;

        private int darknessAvailable;
        private readonly WeightedHeap<String> darknessSources;
        private readonly Dictionary<String, Darkness> clusterDarkness;

        public Algorithm(int seed, DarknessRandomizationSettings settings, Graph g)
        {
            this.r = new(seed);
            this.settings = settings;
            this.g = g;

            this.darknessAvailable = 1000;  // FIXME; based on settings
            this.darknessSources = new();
            this.clusterDarkness = new();
        }

        public Dictionary<string, int> SelectDarknessLevels()
        {
            // Phase 0: Everything starts as bright.
            foreach (var c in g.Clusters.Keys)
            {
                clusterDarkness[c] = Darkness.Bright;
            }

            // Phase 1: Select source nodes until we run out of darkness.
            foreach (var cluster in g.SourceNodes)
            {
                // FIXME: Don't allow the starting area to be darkened.
                int weight = g.Clusters[cluster].ProbabilityWeight;
                darknessSources.Add(cluster, weight);
            }
            while (!darknessSources.IsEmpty() && darknessAvailable > 0)
            {
                SelectNewDarknessNode();
            }

            // Phase 2: Turn the remaining nodes into semi-darkness with high probability.
            foreach (var (c, _) in darknessSources.EnumerateSorted())
            {
                var cluster = g.Clusters[c];
                if (r.Next(0, 100) < cluster.SemiDarkProbabilty)
                {
                    clusterDarkness[c] = Darkness.SemiDark;
                }
            }

            // Phase 3: Output the per-scene darkness levels.
            Dictionary<String, int> darknessLevels = new();
            foreach (var e in clusterDarkness)
            {
                var cluster = e.Key;
                var darkness = e.Value;
                foreach (var e2 in g.Clusters[cluster].Scenes)
                {
                    var scene = e2.Key;
                    var sceneData = e2.Value;

                    Darkness d = sceneData.Clamp(darkness);
                    darknessLevels[scene] = (int)d;
                }
            }
            return darknessLevels;
        }

        private void SelectNewDarknessNode()
        {
            var name = darknessSources.Remove(r);
            var cluster = g.Clusters[name];
            clusterDarkness[name] = Darkness.Dark;
            
            // This can go negative; fixing that means we need to prune the map.
            darknessAvailable -= cluster.CostWeight;

            // Add adjacent clusters if constraints are satisfied.
            foreach (var nname in cluster.AdjacentClusters.Keys)
            {
                if (clusterDarkness[name] == Darkness.Dark || darknessSources.Contains(nname))
                {
                    continue;
                }

                var neighbor = g.Clusters[nname];
                if (neighbor.MaximumDarkness < Darkness.Dark)
                {
                    continue;
                }

                if (neighbor.AdjacentClusters.All(
                    cr => cr.Value != ClusterRelativity.LevelOrDarker || clusterDarkness[cr.Key] == Darkness.Dark))
                {
                    darknessSources.Add(nname, neighbor.ProbabilityWeight);
                }
            }
        }
    }
}
