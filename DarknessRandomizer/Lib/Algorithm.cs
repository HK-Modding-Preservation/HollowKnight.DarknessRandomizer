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
        private readonly WeightedHeap<String> darkCandidates;
        private readonly Dictionary<String, Darkness> clusterDarkness;
        private readonly HashSet<String> semiDarkCandidates;

        public Algorithm(int seed, DarknessRandomizationSettings settings, Graph g)
        {
            this.r = new(seed);
            this.settings = settings;
            this.g = g;

            this.darknessAvailable = 1000;  // FIXME; based on settings
            this.darkCandidates = new();
            this.semiDarkCandidates = new();
            this.clusterDarkness = new();
        }

        public Dictionary<string, Darkness> SelectDarknessLevels()
        {
            // Phase 0: Everything starts as bright.
            foreach (var c in g.Clusters.Keys)
            {
                clusterDarkness[c] = Darkness.Bright;
            }

            // Phase 1: Select source nodes until we run out of darkness.
            foreach (var c in g.SourceNodes)
            {
                // FIXME: Don't allow the starting area to be darkened.
                darkCandidates.Add(c, g.Clusters[c].ProbabilityWeight);
            }
            while (!darkCandidates.IsEmpty() && darknessAvailable > 0)
            {
                SelectNewDarknessNode();
            }

            // Phase 2: Turn the remaining nodes into semi-darkness with high probability.
            var scs = semiDarkCandidates.ToList();
            scs.Sort();
            foreach (var c in scs)
            {
                if (r.Next(0, 100) < g.Clusters[c].SemiDarkProbabilty)
                {
                    clusterDarkness[c] = Darkness.SemiDark;
                }
            }

            // Phase 3: Output the per-scene darkness levels.
            Dictionary<String, Darkness> darknessLevels = new();
            foreach (var e in clusterDarkness)
            {
                var cluster = e.Key;
                var darkness = e.Value;
                foreach (var e2 in g.Clusters[cluster].Scenes)
                {
                    darknessLevels[e2.Key] = e2.Value.Clamp(darkness);
                }
            }
            return darknessLevels;
        }

        private void SelectNewDarknessNode()
        {
            var name = darkCandidates.Remove(r);
            clusterDarkness[name] = Darkness.Dark;
            semiDarkCandidates.Remove(name);

            // This can go negative; fixing that would require pruning the heap of high cost clusters.
            var cluster = g.Clusters[name];
            darknessAvailable -= cluster.CostWeight;

            // Add adjacent clusters if constraints are satisfied.
            foreach (var nname in cluster.AdjacentClusters.Keys)
            {
                if (clusterDarkness[nname] == Darkness.Dark || darkCandidates.Contains(nname))
                {
                    continue;
                }

                var ncluster = g.Clusters[nname];
                if (ncluster.MaximumDarkness >= Darkness.SemiDark)
                {
                    semiDarkCandidates.Add(nname);
                    if (ncluster.MaximumDarkness >= Darkness.Dark && ncluster.AdjacentClusters.All(
                        cr => cr.Value != ClusterRelativity.LevelOrDarker || clusterDarkness[cr.Key] == Darkness.Dark))
                    {
                        darkCandidates.Add(nname, ncluster.ProbabilityWeight);
                    }
                }
            }
        }
    }
}
