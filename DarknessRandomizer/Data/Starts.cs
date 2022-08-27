using System;
using System.Collections.Generic;

namespace DarknessRandomizer.Data
{
    public static class Starts
    {
        // FIXME: Add clusters
        // Some of these are transition-rando only, and so won't benefit from clusters outside the area
        private static readonly Dictionary<String, HashSet<Cluster>> ProtectedStartClusters = new()
        {
            { "Abyss", new() },  // TRANDO only
            {
                "Ancestral Mound",
                new()
                {
                    Cluster.CrossroadsAncestralMound,
                    Cluster.CrossroadsEntrance,
                    Cluster.CrossroadsWest,
                    Cluster.CrossroadsLower,
                    Cluster.CrossroadsMawlek
                }
            },
            { "City Storerooms", new() },
            { "City of Tears", new() },  // TRANDO only
            {
                "Crystallized Mound",
                new()
                {
                    Cluster.CrystalPeakCrystallizedMound,
                    Cluster.RestingGroundsMain
                }
            },
            { "Distant Village", new() },
            {
                "East Blue Lake",
                new()
                {
                    Cluster.BlueLake,
                    Cluster.RestingGroundsMain
                }
            },
            {
                "East Crossroads",
                new()
                {
                    Cluster.CrossroadsUpper,
                    Cluster.CrossroadsEntrance,
                    Cluster.CrossroadsWest,
                    Cluster.CrossroadsPeaksBridge
                }
            },
            {
                "East Fog Canyon",
                new()
                {
                    Cluster.FogCanyonEast,
                    Cluster.GreenpathOutsideNoEyes,
                    Cluster.GreenpathLower
                }
            },
            { "Far Greenpath", new() },  // TRANDO only
            { "Fungal Core", new() },  // TRANDO only
            { "Fungal Wastes", new() },
            {
                "Greenpath",
                new()
                {
                    Cluster.GreenpathCliffsBridge,
                    Cluster.GreenpathLower,
                    Cluster.GreenpathOutsideNoEyes,
                    Cluster.GreenpathUpper,
                    Cluster.GreenpathWest
                }
            },
            {
                "Hallownest's Crown",
                new()
                {
                    Cluster.CrystalPeakCrown,
                    Cluster.CrystalPeakUpper,
                    Cluster.CrystalPeakWest,
                    Cluster.CrystalPeakLower
                }
            },
            { "Hive", new() },  // TRANDO only
            {
                "King's Pass",
                new()
                {
                    Cluster.KingsPass,
                    Cluster.CrossroadsEntrance,
                    Cluster.CrossroadsWest,
                    Cluster.CrossroadsUpper
                }
            },
            { "King's Station", new() },
            { "Kingdom's Edge", new() },  // TRANDO only
            {
                "Lower Greenpath",
                new()
                {
                    Cluster.FogCanyonWest,
                    Cluster.GreenpathLower,
                    Cluster.GreenpathMMC,
                    Cluster.GreenpathOutsideNoEyes,
                    Cluster.GreenpathUpper
                }
            },
            { "Mantis Village", new() },
            { "Outside Colosseum", new() },
            { "Queen's Gardens", new() },
            { "Queen's Station", new() },
            { "Royal Waterways", new() },
            { "Stag Nest", new()
            {
                Cluster.CliffsMain,
                Cluster.KingsPass,
                Cluster.CliffsBaldur,
                Cluster.GreenpathUpper,
                Cluster.CrossroadsEntrance
            } },
            {
                "West Blue Lake",
                new()
                {

                    Cluster.BlueLake,
                    Cluster.CrossroadsLower,
                    Cluster.CrossroadsUpper,
                    Cluster.CrossroadsEntrance
                }
            },
            {
                "West Crossroads",
                new()
                {
                    Cluster.CrossroadsMawlek,
                    Cluster.CrossroadsWest,
                    Cluster.CrossroadsEntrance,
                    Cluster.CrossroadsLower
                }
            },
            { "West Fog Canyon", new() },
            { "West Waterways", new() }
        };

        public static IReadOnlyCollection<Cluster> GetStartClusters(string start) => ProtectedStartClusters[start];
    }
}
