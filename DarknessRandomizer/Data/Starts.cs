using System;
using System.Collections.Generic;

namespace DarknessRandomizer.Data
{
    public static class Starts
    {
        // FIXME: Add clusters
        // Some of these are transition-rando only, and so won't benefit from clusters outside the area
        private static readonly Dictionary<String, HashSet<LegacyCluster>> ProtectedStartClusters = new()
        {
            { "Abyss", new() },  // TRANDO only
            {
                "Ancestral Mound",
                new()
                {
                    LegacyCluster.CrossroadsAncestralMound,
                    LegacyCluster.CrossroadsEntrance,
                    LegacyCluster.CrossroadsWest,
                    LegacyCluster.CrossroadsLower,
                    LegacyCluster.CrossroadsMawlek
                }
            },
            { "City Storerooms", new() },
            { "City of Tears", new() },  // TRANDO only
            {
                "Crystallized Mound",
                new()
                {
                    LegacyCluster.CrystalPeakCrystallizedMound,
                    LegacyCluster.RestingGroundsMain
                }
            },
            { "Distant Village", new() },
            {
                "East Blue Lake",
                new()
                {
                    LegacyCluster.BlueLake,
                    LegacyCluster.RestingGroundsMain
                }
            },
            {
                "East Crossroads",
                new()
                {
                    LegacyCluster.CrossroadsUpper,
                    LegacyCluster.CrossroadsEntrance,
                    LegacyCluster.CrossroadsWest,
                    LegacyCluster.CrossroadsPeaksBridge
                }
            },
            {
                "East Fog Canyon",
                new()
                {
                    LegacyCluster.FogCanyonEast,
                    LegacyCluster.GreenpathOutsideNoEyes,
                    LegacyCluster.GreenpathLower
                }
            },
            { "Far Greenpath", new() },  // TRANDO only
            { "Fungal Core", new() },  // TRANDO only
            { "Fungal Wastes", new() },
            {
                "Greenpath",
                new()
                {
                    LegacyCluster.GreenpathCliffsBridge,
                    LegacyCluster.GreenpathLower,
                    LegacyCluster.GreenpathOutsideNoEyes,
                    LegacyCluster.GreenpathUpper,
                    LegacyCluster.GreenpathWest
                }
            },
            {
                "Hallownest's Crown",
                new()
                {
                    LegacyCluster.CrystalPeakCrown,
                    LegacyCluster.CrystalPeakUpper,
                    LegacyCluster.CrystalPeakWest,
                    LegacyCluster.CrystalPeakLower
                }
            },
            { "Hive", new() },  // TRANDO only
            {
                "King's Pass",
                new()
                {
                    LegacyCluster.KingsPass,
                    LegacyCluster.CrossroadsEntrance,
                    LegacyCluster.CrossroadsWest,
                    LegacyCluster.CrossroadsUpper
                }
            },
            { "King's Station", new() },
            { "Kingdom's Edge", new() },  // TRANDO only
            {
                "Lower Greenpath",
                new()
                {
                    LegacyCluster.FogCanyonWest,
                    LegacyCluster.GreenpathLower,
                    LegacyCluster.GreenpathMMC,
                    LegacyCluster.GreenpathOutsideNoEyes,
                    LegacyCluster.GreenpathUpper
                }
            },
            { "Mantis Village", new() },
            { "Outside Colosseum", new() },
            { "Queen's Gardens", new() },
            { "Queen's Station", new() },
            { "Royal Waterways", new() },
            { "Stag Nest", new()
            {
                LegacyCluster.CliffsMain,
                LegacyCluster.KingsPass,
                LegacyCluster.CliffsBaldur,
                LegacyCluster.GreenpathUpper,
                LegacyCluster.CrossroadsEntrance
            } },
            {
                "West Blue Lake",
                new()
                {

                    LegacyCluster.BlueLake,
                    LegacyCluster.CrossroadsLower,
                    LegacyCluster.CrossroadsUpper,
                    LegacyCluster.CrossroadsEntrance
                }
            },
            {
                "West Crossroads",
                new()
                {
                    LegacyCluster.CrossroadsMawlek,
                    LegacyCluster.CrossroadsWest,
                    LegacyCluster.CrossroadsEntrance,
                    LegacyCluster.CrossroadsLower
                }
            },
            { "West Fog Canyon", new() },
            { "West Waterways", new() }
        };

        public static IReadOnlyCollection<LegacyCluster> GetStartClusters(string start) => ProtectedStartClusters[start];
    }
}
