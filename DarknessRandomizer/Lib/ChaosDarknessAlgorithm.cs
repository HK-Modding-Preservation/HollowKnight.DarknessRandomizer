using DarknessRandomizer.Data;
using DarknessRandomizer.Rando;
using RandomizerMod.RandomizerData;
using RandomizerMod.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DarknessRandomizer.Lib
{
    public class ChaosDarknessAlgoritm : DarknessAlgorithm
    {

        public ChaosDarknessAlgoritm(GenerationSettings GS, StartDef start, DarknessRandomizationSettings DRS) : base(GS, start, DRS) { }

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

        // TODO: Switch chaos mode to a scene-based darkness algorithm.
        public override void SpreadDarkness(out SceneDarknessDict darknessOverrides, out AlgorithmStats stats)
        {
            // Phase 0: Everything starts as bright.
            ClusterDarknessDict clusterDarkness = new();
            foreach (var c in ClusterName.All())
            {
                clusterDarkness[c] = Darkness.Bright;
            }

            // Randomly select individual clusters to be dark or semi-dark, ignoring all usual constraints.
            List<ClusterName> clusters = new(ClusterName.All());
            Shuffle(r, clusters);

            // Phase 1: assign darkness and semi-darkness randomly.
            int semiDarknessAvailable = darknessAvailable;
            for (int i = 0; i < clusters.Count && (darknessAvailable > 0 || semiDarknessAvailable > 0); ++i)
            {
                var c = clusters[i];
                var cData = ClusterData.Get(c);
                var maxDarkness = cData.MaximumDarkness(DRS);

                if (darknessAvailable > 0 && maxDarkness >= Darkness.Dark)
                {
                    clusterDarkness[c] = Darkness.Dark;
                    darknessAvailable -= cData.CostWeight;
                }
                else if (semiDarknessAvailable > 0 && maxDarkness >= Darkness.SemiDark)
                {
                    clusterDarkness[c] = Darkness.SemiDark;
                    semiDarknessAvailable -= cData.CostWeight > 0 ? cData.CostWeight : 25 * cData.SceneNames.Count;
                }
            }

            // Phase 2: Output the per-scene darkness levels.
            GetPerSceneDarknessLevels(clusterDarkness, out darknessOverrides, out stats);
        }

        private void GetPerSceneDarknessLevels(ClusterDarknessDict clusterDarkness, out SceneDarknessDict darknessOverrides, out AlgorithmStats stats)
        {
            darknessOverrides = new();
            foreach (var e in clusterDarkness.Enumerate())
            {
                var cluster = e.Key;
                var darkness = e.Value;
                foreach (var scene in ClusterData.Get(cluster).SceneNames.Keys)
                {
                    darknessOverrides[scene] = Data.SceneData.Get(scene).ClampDarkness(darkness);
                }
            }

            stats = new()
            {
                DarknessSpent = 0,
                DarknessRemaining = 0
            };
            foreach (var e in clusterDarkness.Enumerate())
            {
                var cData = ClusterData.Get(e.Key);
                if (e.Value == Darkness.Dark)
                {
                    stats.DarknessSpent += cData.CostWeight;
                }
                else if (cData.MaximumDarkness(DRS) == Darkness.Dark && !forcedBrightClusters.Contains(e.Key))
                {
                    stats.DarknessRemaining += cData.CostWeight;
                }
            }
        }
    }
}
