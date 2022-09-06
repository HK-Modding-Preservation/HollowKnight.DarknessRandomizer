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

        private int darknessAvailable;
        private readonly ClusterDarknessDict clusterDarkness;
        private readonly WeightedHeap<ClusterName> darkCandidates;
        private readonly HashSet<ClusterName> forcedBrightClusters;

        public Algorithm(GenerationSettings GS, StartDef startDef, DarknessRandomizationSettings settings)
        {
            this.r = new(GS.Seed + 7);
            this.settings = settings;

            this.darknessAvailable = settings.GetDarknessBudget(r);
            this.clusterDarkness = new();
            this.darkCandidates = new();
            this.forcedBrightClusters = new();

            foreach (var c in Starts.GetStartClusters(startDef.Name))
            {
                this.forcedBrightClusters.Add(c);
            }

            // Always include the local cluster, even in TRANDO.
            if (SceneName.TryGetValue(startDef.SceneName, out SceneName sceneName))
            {
                this.forcedBrightClusters.Add(Data.SceneData.Get(sceneName).Cluster);
            }

            if (!GS.PoolSettings.Keys)
            {
                // If lantern isn't randomized, we need to ensure the vanilla lantern location (Sly) is accessible.
                // This won't work if they also didn't randomize stags but I'm not trying to fix that.
                this.forcedBrightClusters.Add(ClusterName.CrossroadsStag);
                this.forcedBrightClusters.Add(ClusterName.CrossroadsStagHub);
                this.forcedBrightClusters.Add(ClusterName.CrossroadsShops);
            }
            
            if (GS.CursedSettings.Deranged)
            {
                // Disallow vanilla dark rooms.
                this.forcedBrightClusters.Add(ClusterName.CliffsJonis);
                this.forcedBrightClusters.Add(ClusterName.CrystalPeaksDarkRoom);
                this.forcedBrightClusters.Add(ClusterName.CrystalPeaksToll);
                this.forcedBrightClusters.Add(ClusterName.DeepnestDark);
                this.forcedBrightClusters.Add(ClusterName.GreenpathStoneSanctuary);
            }

            // If white palace rando is disabled, add all white palace rooms to the forbidden set.
            if (GS.LongLocationSettings.WhitePalaceRando != LongLocationSettings.WPSetting.Allowed)
            {
                foreach (var cluster in ClusterName.All())
                {
                    var cData = ClusterData.Get(cluster);
                    if (cData.IsInPathOfPain || (GS.LongLocationSettings.WhitePalaceRando == LongLocationSettings.WPSetting.ExcludeWhitePalace && cData.IsInWhitePalace))
                    {
                        this.forcedBrightClusters.Add(cluster);
                    }
                }
            }
        }

        private static void Shuffle<T>(Random r, List<T> list)
        {
            for (int i = 0; i < list.Count - 1; ++i)
            {
                int j = i + r.Next(0, list.Count - i);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        public void SelectDarknessLevels(out SceneDarknessDict darknessOverrides, out AlgorithmStats stats)
        {
            // Phase 0: Everything starts as bright.
            foreach (var c in ClusterName.All())
            {
                clusterDarkness[c] = Darkness.Bright;
            }

            if (settings.Chaos)
            {
                // Randomly select individual clusters to be dark or semi-dark, ignoring all usual constraints.
                List<ClusterName> clusters = new(ClusterName.All());
                clusters.RemoveAll(c => ClusterData.Get(c).MaximumDarkness(settings) != Darkness.Dark);
                Shuffle(r, clusters);

                for (int i = 0; i < clusters.Count && darknessAvailable > 0; ++i)
                {
                    var c = clusters[i];
                    clusterDarkness[c] = Darkness.Dark;
                    darknessAvailable -= ClusterData.Get(c).CostWeight;
                }
            }
            else
            {
                // Phase 1: Select source nodes until we run out of darkness.
                foreach (var c in ClusterName.All())
                {
                    var cData = ClusterData.Get(c);
                    if (cData.CanBeDarknessSource(settings) && !forcedBrightClusters.Contains(c))
                    {
                        darkCandidates.Add(c, cData.ProbabilityWeight);
                    }
                }
                while (!darkCandidates.IsEmpty() && darknessAvailable > 0)
                {
                    SelectNewDarknessNode();
                }
            }

            // Inject custom darkness here.
            // The code below does a full inversion - all traditionally lit rooms are dark, and vice versa.
            // Mainly for debugging/testing purposes. You'll need to turn on DARKROOMS for this to get past Logic.
            //
            // foreach (var c in ClusterName.All())
            // {
            //     var cData = ClusterData.Get(c);
            //     clusterDarkness[c] = cData.MaximumDarkness(settings);
            // }
            // clusterDarkness[ClusterName.GreenpathStoneSanctuary] = Darkness.Bright;
            // clusterDarkness[ClusterName.DeepnestDark] = Darkness.Bright;
            // clusterDarkness[ClusterName.CrystalPeaksToll] = Darkness.Bright;
            // clusterDarkness[ClusterName.CrystalPeaksDarkRoom] = Darkness.Bright;
            // clusterDarkness[ClusterName.CliffsJonis] = Darkness.Bright;

            // Phase 2: Calculate semi-dark clusters.
            foreach (var name in ClusterName.All())
            {
                var darkness = clusterDarkness[name];
                
                if (darkness == Darkness.Dark)
                {
                    var cData = ClusterData.Get(name);
                    foreach (var e in cData.AdjacentClusters.Enumerate())
                    {
                        var aName = e.Key;
                        var rd = e.Value;
                        if (clusterDarkness[aName] == Darkness.Bright && rd != RelativeDarkness.Disconnected)
                        {
                            clusterDarkness[aName] = Darkness.SemiDark;
                        }
                    }
                }
            }

            // Phase 3: Output the per-scene darkness levels.
            GetPerSceneDarknessLevels(out darknessOverrides, out stats);
        }

        private void SelectNewDarknessNode()
        {
            var name = darkCandidates.Remove(r);
            clusterDarkness[name] = Darkness.Dark;

            // This can go negative; fixing that would require pruning the heap of high cost clusters.
            var cData = ClusterData.Get(name);
            darknessAvailable -= cData.CostWeight;

            // Add adjacent clusters if constraints are satisfied.
            foreach (var e in cData.AdjacentClusters.Enumerate())
            {
                var aName = e.Key;
                var rd = e.Value;

                if (clusterDarkness[aName] == Darkness.Dark || rd == RelativeDarkness.Disconnected || forcedBrightClusters.Contains(aName))
                {
                    continue;
                }

                var aData = ClusterData.Get(aName);
                if (!darkCandidates.Contains(aName) && aData.MaximumDarkness(settings) == Darkness.Dark
                    && aData.AdjacentClusters.Enumerate().All(
                        e => e.Value != RelativeDarkness.Darker || clusterDarkness[e.Key] == Darkness.Dark))
                {
                    darkCandidates.Add(aName, aData.ProbabilityWeight);
                }
            }
        }

        private void GetPerSceneDarknessLevels(out SceneDarknessDict darknessOverrides, out AlgorithmStats stats)
        {
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

            stats = new()
            {
                ClusterDarkness = new(clusterDarkness),
                DarknessSpent = 0,
                DarknessRemaining = 0
            };
            foreach (var e in stats.ClusterDarkness.Enumerate())
            {
                var cData = ClusterData.Get(e.Key);
                if (e.Value == Darkness.Dark)
                {
                    stats.DarknessSpent += cData.CostWeight;
                }
                else if (cData.MaximumDarkness(settings) == Darkness.Dark && !forcedBrightClusters.Contains(e.Key))
                {
                    stats.DarknessRemaining += cData.CostWeight;
                }
            }
        }
    }
}
