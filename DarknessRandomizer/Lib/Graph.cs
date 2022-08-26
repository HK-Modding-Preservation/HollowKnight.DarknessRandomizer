using DarknessRandomizer.Data;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace DarknessRandomizer.Lib
{
    // The relative darkness of an adjacent SceneCluster.
    // This enforces, for certain areas, that it can only get darker/brighter deeper in.
    //
    // This helps places like dark Path of Pain feel rewarding, and can allow players to confidently avoid them if they choose,
    // by ensuring that if the entrance is dark... the rest of it will be, too.
    public enum ClusterRelativity : int
    {
        LevelOrBrighter = 0,
        DarkerOrBrighter = 1,
        LevelOrDarker = 2
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
        public Dictionary<String, Scene> Scenes = new();

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
        public Dictionary<String, ClusterRelativity> AdjacentClusters = new();

        public bool IsDarknessSource
        {
            get
            {
                foreach (var cr in AdjacentClusters.Values)
                {
                    if (cr == ClusterRelativity.LevelOrDarker) return false;
                }
                return true;
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

        public Dictionary<String, SceneCluster> Clusters = new();

        // Map of scene names to cluster names.
        private Dictionary<String, String> sceneLookup;

        public String ClusterFor(String scene)
        {
            if (sceneLookup.TryGetValue(scene, out String cluster))
            {
                return cluster;
            }
            return "";
        }

        // Clusters which have no 'darker' edges, and thus can be a source of darkness.
        private HashSet<String> sourceNodes;
        public IEnumerable<String> SourceNodes
        {
            get { return sourceNodes; }
        }

        private static ClusterRelativity Oppose(ClusterRelativity cr)
        {
            switch (cr)
            {
                case ClusterRelativity.LevelOrBrighter:
                    return ClusterRelativity.LevelOrDarker;
                case ClusterRelativity.LevelOrDarker:
                    return ClusterRelativity.LevelOrDarker;
                case ClusterRelativity.DarkerOrBrighter:
                    return ClusterRelativity.DarkerOrBrighter;
            }
            throw new ArgumentException($"Unknown ClusterRelativity {cr}");
        }

        public void Init()
        {
            sceneLookup = new();
            sourceNodes = new();

            foreach (var item in Clusters) {
                var name = item.Key;
                var cluster = item.Value;

                if (cluster.IsDarknessSource)
                {
                    sourceNodes.Add(name);
                }
                foreach (var scene in cluster.Scenes.Keys)
                {
                    sceneLookup.Add(scene, name);
                }

                // Mirror connections
                foreach (var e in item.Value.AdjacentClusters)
                {
                    var nname = e.Key;
                    var cr = e.Value;
                    var ncluster = Clusters[nname];

                    if (cluster.AdjacentClusters.TryGetValue(name, out ClusterRelativity ncr))
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