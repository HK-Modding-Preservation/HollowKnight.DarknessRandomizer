using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace DarknessRandomizer.Lib
{
    // The type of darkness assigned to a SceneCluster
    public enum ClusterDarkness : int
    {
        Bright = 0,  // All Bright
        HalfSemiDark = 1,  // All Bright/SemiDark
        SemiDark = 2,  // All SemiDark
        HalfDark = 3,  // All SemiDark/Dark
        Dark = 4  // All Dark
    }

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
        // The maximum assignable darkness level to this room.
        public Darkness MaximumDarkness;

        // Locations in this set require DIFFICULTSKIPS in addition to DARKROOMS without lantern.
        public LocationSet DifficultSkipLocs;

        // Locations in this set require PROFICIENTCOMBAT in addition to DARKROOMS without lantern.
        public LocationSet ProficientCombatLocs;
    }

    public class Weights
    {
        // Relative probability of a specific ClusterDarkness being chosen.
        public float Probability;

        // Relative cost of a specific ClusterDarkness being chosen.
        public float Cost;
    }

    // A cluster of adjacent scenes that should share their darkness level.
    //
    // SceneClusters are organized into a topology, where a scene may have zero to many 'lower' adjacent clusters,
    // and 'higher' adjacent clusters. Randomization enforces that as one wanders the scene cluster graph from
    // high nodes to low nodes, the cluster darkness level must be monotonically non-increasing.
    public class SceneCluster
    {
        // Scene data for the cluster.
        public Dictionary<String, Scene> Scenes;

        // If false, all scenes in the cluster must have the same darkness level.
        public Boolean AllowMixed;

        // Adjacent clusters, organized by relative darkness.
        public Dictionary<ClusterRelativity, HashSet<String>> AdjacentClusters;

        // Optional weights to apply to the probability that this cluster receives the given darkness level.
        public Dictionary<ClusterDarkness, Weights> Weights;
    }

    public class Graph
    {
        Dictionary<String, SceneCluster> Clusters;

        // Map of scene names to cluster names.
        [Newtonsoft.Json.JsonIgnore] private Dictionary<String, String> sceneLookup;

        public void Init()
        {
            sceneLookup = new();

            foreach (var item in Clusters) {
                foreach (var scene in item.Value.Scenes.Keys)
                {
                    sceneLookup.Add(scene, item.Key);
                }
            }
        }
    }
}