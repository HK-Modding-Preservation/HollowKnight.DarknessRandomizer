using DarknessRandomizer.Data;
using DarknessRandomizer.Rando;
using RandomizerMod.RandomizerData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DarknessRandomizer.Lib
{
    public record AlgorithmStats
    {
        public Dictionary<Cluster, Darkness> ClusterSelections;
        public int DarknessSpent;
        public int DarknessRemaining;
    }

    public class Algorithm
    {
        private readonly Random r;
        private readonly DarknessRandomizationSettings settings;
        private readonly Graph g;

        private int darknessAvailable;
        private readonly Dictionary<Cluster, Darkness> clusterDarkness;
        private readonly WeightedHeap<Cluster> darkCandidates;
        private readonly HashSet<Cluster> semiDarkCandidates;
        private readonly HashSet<Cluster> forbiddenClusters;

        public Algorithm(int seed, StartDef startDef, DarknessRandomizationSettings settings, Graph g)
        {
            this.r = new(seed);
            this.settings = settings;
            this.g = g;

            this.darknessAvailable = 1000;  // FIXME; based on settings
            this.clusterDarkness = new();
            this.darkCandidates = new();
            this.semiDarkCandidates = new();
            this.forbiddenClusters = new();

            foreach (var c in Starts.GetStartClusters(startDef.Name))
            {
                this.forbiddenClusters.Add(c);
            }
            
            // Always include the local cluster, even in TRANDO.
            if (SceneName.TryGetSceneName(startDef.SceneName, out SceneName sceneName) && g.TryGetCluster(sceneName, out Cluster cluster))
            {
                this.forbiddenClusters.Add(cluster);
            }
        }

        public void SelectDarknessLevels(out DarknessDictionary darknessOverrides, out AlgorithmStats stats)
        {
            // Phase 0: Everything starts as bright.
            foreach (var c in g.Clusters.Keys)
            {
                clusterDarkness[c] = Darkness.Bright;
            }

            // Phase 1: Select source nodes until we run out of darkness.
            foreach (var e in g.Clusters)
            {
                var cluster = e.Key;
                var data = e.Value;

                if (!forbiddenClusters.Contains(cluster))
                {
                    darkCandidates.Add(cluster, data.ProbabilityWeight);
                }
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
            darknessOverrides = new();
            foreach (var e in clusterDarkness)
            {
                var cluster = e.Key;
                var darkness = e.Value;
                foreach (var e2 in g.Clusters[cluster].Scenes)
                {
                    darknessOverrides[e2.Key] = e2.Value.Clamp(darkness);
                }
            }

            stats = new();
            stats.ClusterSelections = new(clusterDarkness);
            stats.DarknessSpent = 0;
            stats.DarknessRemaining = 0;
            foreach (var e in stats.ClusterSelections)
            {
                var cluster = g.Clusters[e.Key];
                if (e.Value == Darkness.Dark)
                {
                    stats.DarknessSpent += cluster.CostWeight;
                }
                else if (cluster.CanBeDark(settings) && !forbiddenClusters.Contains(e.Key))
                {
                    stats.DarknessRemaining += cluster.CostWeight;
                }
            }
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
                if (clusterDarkness[nname] == Darkness.Dark || forbiddenClusters.Contains(name))
                {
                    continue;
                }

                var ncluster = g.Clusters[nname];
                if (ncluster.MaximumDarkness >= Darkness.SemiDark)
                {
                    semiDarkCandidates.Add(nname);
                    if (!darkCandidates.Contains(nname) && ncluster.CanBeDark(settings)
                        && ncluster.AdjacentClusters.All(
                            cr => cr.Value != RelativeDarkness.Darker || clusterDarkness[cr.Key] == Darkness.Dark))
                    {
                        darkCandidates.Add(nname, ncluster.ProbabilityWeight);
                    }
                }
            }
        }
    }
}
