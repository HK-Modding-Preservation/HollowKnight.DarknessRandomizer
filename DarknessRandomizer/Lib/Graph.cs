using DarknessRandomizer.Data;
using DarknessRandomizer.Rando;
using System;
using System.Collections.Generic;

namespace DarknessRandomizer.Lib
{
    // The relative darkness of an adjacent SceneCluster.
    // This enforces, for certain areas, that it can only get darker/brighter deeper in.
    //
    // This helps places like dark Path of Pain feel rewarding, and can allow players to confidently avoid them if they choose,
    // by ensuring that if the entrance is dark... the rest of it will be, too.
    public enum RelativeDarkness : int
    {
        Brighter = 0,
        Any = 1,
        Darker = 2
    }

    // Data specific to a scene.
    public class Scene
    {
        public Darkness MinimumDarkness = Darkness.Bright;
        public Darkness MaximumDarkness = Darkness.Dark;
        public LocationSet DifficultSkipLocs = LocationSet.NONE;
        public LocationSet ProficientCombatLocs = LocationSet.NONE;

        public Darkness Clamp(Darkness d)
        {
            if (d < MinimumDarkness) return MinimumDarkness;
            if (d > MaximumDarkness) return MaximumDarkness;
            return d;
        }
    }

    // A cluster of adjacent scenes that should share their darkness level.
    //
    // SceneClusters are organized into a topology, where a scene may have zero to many 'lower' adjacent clusters,
    // and 'higher' adjacent clusters. Randomization enforces that as one wanders the scene cluster graph from
    // high nodes to low nodes, the cluster darkness level must be monotonically non-increasing.
    public class SceneCluster
    {
        // Scene data for the cluster.
        public Dictionary<string, Scene> Scenes = new();

        public Darkness MaximumDarkness
        {
            get
            {
                var m = Darkness.Bright;
                foreach (var e in Scenes)
                {
                    if (e.Value.MaximumDarkness > m)
                    {
                        m = e.Value.MaximumDarkness;
                    }
                }
                return m;
            }
        }

        // Adjacent clusters, defined by relative darkness.
        public Dictionary<Cluster, RelativeDarkness> AdjacentClusters = new();

        // If false, darkness cannot start here even if it otherwise could.
        // It must start somewhere adjacent and spread here instead.
        public bool OverrideIsDarknessSource = true;

        // If true, this can only be made dark in Cursed mode.
        public bool CursedOnly = false;

        public bool IsDarknessSource
        {
            get
            {
                if (!OverrideIsDarknessSource || !CanBeDark)
                {
                    return false;
                }

                foreach (var cr in AdjacentClusters.Values)
                {
                    if (cr == RelativeDarkness.Darker) return false;
                }
                return true;
            }
        }

        public bool CanBeDark
        {
            get
            {
                return MaximumDarkness >= Darkness.Dark && (!CursedOnly || RandoInterop.LS.Settings.DarknessLevel == DarknessLevel.Cursed);
            }
        }

        // The relative probability of this cluster being selected for darkness.
        public int ProbabilityWeight = 100;

        // The cost of this cluster being selected for darkness.
        public int CostWeight = 100;

        // The probability that this cluster is made semi-dark if it's adjacent to a dark cluster.
        public int SemiDarkProbabilty = 100;
    }

    public class Graph
    {
        public static Graph Instance = GraphData.LoadGraph();

        public Dictionary<Cluster, SceneCluster> Clusters = new();

        // Map of scene names to cluster names.
        private Dictionary<string, Cluster> sceneLookup;

        public bool TryGetCluster(string scene, out Cluster cluster) => sceneLookup.TryGetValue(scene, out cluster);

        public bool TryGetSceneData(string scene, out Scene sceneData)
        {
            if (TryGetCluster(scene, out Cluster c))
            {
                sceneData = Clusters[c].Scenes[scene];
                return true;
            }

            sceneData = default;
            return false;
        }

        private static RelativeDarkness Oppose(RelativeDarkness cr)
        {
            switch (cr)
            {
                case RelativeDarkness.Brighter:
                    return RelativeDarkness.Darker;
                case RelativeDarkness.Darker:
                    return RelativeDarkness.Darker;
                case RelativeDarkness.Any:
                    return RelativeDarkness.Any;
            }
            throw new ArgumentException($"Unknown ClusterRelativity {cr}");
        }

        public void Init()
        {
            sceneLookup = new();

            foreach (Cluster c in Enum.GetValues(typeof(Cluster)))
            {
                if (!Clusters.ContainsKey(c))
                {
                    throw new ArgumentException($"Missing cluster: {c}");
                }
            }

            foreach (var item in Clusters) {
                var name = item.Key;
                var cluster = item.Value;

                foreach (var scene in cluster.Scenes.Keys)
                {
                    if (sceneLookup.ContainsKey(scene))
                    {
                        throw new ArgumentException($"Duplicate scene registration: {scene}");
                    }

                    sceneLookup.Add(scene, name);
                }

                // Mirror connections
                foreach (var e in item.Value.AdjacentClusters)
                {
                    var nname = e.Key;
                    var cr = e.Value;
                    var ncluster = Clusters[nname];

                    if (cluster.AdjacentClusters.TryGetValue(name, out RelativeDarkness ncr))
                    {
                        if (ncr != Oppose(cr))
                        {
                            throw new ArgumentException($"Clusters {name} and {nname} have mismatched ClusterRelativity");
                        }
                    }
                    else
                    {
                        ncluster.AdjacentClusters[name] = Oppose(cr);
                    }
                }
            }
        }
    }
}