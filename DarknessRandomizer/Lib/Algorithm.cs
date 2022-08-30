using DarknessRandomizer.Data;
using DarknessRandomizer.Rando;
using RandomizerMod.RandomizerData;
using RandomizerMod.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DarknessRandomizer.Lib
{
    public record AlgorithmStats
    {
        public ClusterDarknessDict ClusterDarkness;
        public int DarknessSpent;
        public int DarknessRemaining;
    }

    public class Algorithm
    {
        private readonly Random r;
        private readonly DarknessRandomizationSettings settings;
        private readonly GenerationSettings GS;

        private int darknessAvailable;
        private readonly ClusterDarknessDict clusterDarkness;
        private readonly WeightedHeap<ClusterName> darkCandidates;
        private readonly HashSet<ClusterName> semiDarkCandidates;
        private readonly HashSet<ClusterName> forbiddenClusters;

        public Algorithm(int seed, StartDef startDef, DarknessRandomizationSettings settings, GenerationSettings GS)
        {
            this.r = new(seed);
            this.settings = settings;

            this.darknessAvailable = settings.GetDarknessBudget(r);
            this.clusterDarkness = new();
            this.darkCandidates = new();
            this.semiDarkCandidates = new();
            this.forbiddenClusters = new();

            foreach (var c in Starts.GetStartClusters(startDef.Name))
            {
                this.forbiddenClusters.Add(c);
            }
            
            // Always include the local cluster, even in TRANDO.
            if (SceneName.TryGetSceneName(startDef.SceneName, out SceneName sceneName))
            {
                DarknessRandomizer.Log($"SceneName: {sceneName}");
                this.forbiddenClusters.Add(Data.SceneData.Get(sceneName).Cluster);
            }

            // If white palace rando is disabled, add all white palace rooms to the forbidden set.
            if (!GS.LongLocationSettings.WhitePalaceRando)
            {
                foreach (var cluster in ClusterName.All())
                {
                    if (ClusterData.Get(cluster).IsInWhitePalace)
                    {
                        this.forbiddenClusters.Add(cluster);
                    }
                }
            }
        }

        public void SelectDarknessLevels(out SceneDarknessDict darknessOverrides, out AlgorithmStats stats)
        {
            // Phase 0: Everything starts as bright.
            foreach (var c in ClusterName.All())
            {
                clusterDarkness[c] = Darkness.Bright;
            }

            // Phase 1: Select source nodes until we run out of darkness.
            foreach (var c in ClusterName.All())
            {
                var cData = ClusterData.Get(c);
                if (ClusterData.Get(c).CanBeDarknessSource(settings) && !forbiddenClusters.Contains(c))
                {
                    darkCandidates.Add(c, cData.ProbabilityWeight);
                }
            }
            while (!darkCandidates.IsEmpty() && darknessAvailable > 0)
            {
                SelectNewDarknessNode();
            }

            // Phase 2: Turn the remaining nodes into semi-darkness.
            foreach (var c in semiDarkCandidates)
            {
                clusterDarkness[c] = Darkness.SemiDark;
            }

            // Phase 3: Output the per-scene darkness levels.
            darknessOverrides = new();
            foreach (var e in clusterDarkness.Enumerate())
            {
                var cluster = e.Key;
                var darkness = e.Value;
                foreach (var scene in ClusterData.Get(cluster).SceneNames)
                {
                    darknessOverrides[scene] = Data.SceneData.Get(scene).ClampDarkness(darkness);
                }
            }

            stats = new();
            stats.ClusterDarkness = new(clusterDarkness);
            stats.DarknessSpent = 0;
            stats.DarknessRemaining = 0;
            foreach (var e in stats.ClusterDarkness.Enumerate())
            {
                var cData = ClusterData.Get(e.Key);
                if (e.Value == Darkness.Dark)
                {
                    stats.DarknessSpent += cData.CostWeight;
                }
                else if (cData.MaximumDarkness(settings) == Darkness.Dark && !forbiddenClusters.Contains(e.Key))
                {
                    stats.DarknessRemaining += cData.CostWeight;
                }
            }
        }

        private void SelectNewDarknessNode()
        {
            var name = darkCandidates.Remove(r);
            clusterDarkness[name] = Darkness.Dark;
            semiDarkCandidates.Remove(name);

            // This can go negative; fixing that would require pruning the heap of high cost clusters.
            var cData = ClusterData.Get(name);
            darknessAvailable -= cData.CostWeight;

            // Add adjacent clusters if constraints are satisfied.
            foreach (var e in cData.AdjacentClusters.Enumerate())
            {
                var nname = e.Key;

                if (clusterDarkness[nname] == Darkness.Dark || forbiddenClusters.Contains(name))
                {
                    continue;
                }

                var ncluster = ClusterData.Get(nname);
                Darkness maxDark = ncluster.MaximumDarkness(settings);
                if (maxDark >= Darkness.SemiDark)
                {
                    semiDarkCandidates.Add(nname);
                    if (!darkCandidates.Contains(nname) && maxDark == Darkness.Dark
                        && ncluster.AdjacentClusters.Enumerate().All(
                            e => e.Value != RelativeDarkness.Darker || clusterDarkness[e.Key] == Darkness.Dark))
                    {
                        darkCandidates.Add(nname, ncluster.ProbabilityWeight);
                    }
                }
            }
        }
    }
}
