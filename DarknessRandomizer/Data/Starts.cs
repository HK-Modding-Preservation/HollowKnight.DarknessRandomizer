using DarknessRandomizer.Lib;
using System;
using System.Collections.Generic;

namespace DarknessRandomizer.Data
{
    public static class Starts
    {
        // FIXME: Add clusters
        // Some of these are transition-rando only, and so won't benefit from clusters outside the area
        private static readonly Dictionary<string, HashSet<ClusterName>> ProtectedStartClusters = new()
        {
            { "Abyss", new() },  // TRANDO only
            {
                "Ancestral Mound",
                new()
                {
                }
            },
            { "City Storerooms", new() },
            { "City of Tears", new() },  // TRANDO only
            {
                "Crystallized Mound",
                new()
                {
                }
            },
            { "Distant Village", new() },
            {
                "East Blue Lake",
                new()
                {
                }
            },
            {
                "East Crossroads",
                new()
                {
                }
            },
            {
                "East Fog Canyon",
                new()
                {
                }
            },
            { "Far Greenpath", new() },  // TRANDO only
            { "Fungal Core", new() },  // TRANDO only
            { "Fungal Wastes", new() },
            {
                "Greenpath",
                new()
                {
                }
            },
            {
                "Hallownest's Crown",
                new()
                {
                }
            },
            { "Hive", new() },  // TRANDO only
            {
                "King's Pass",
                new()
                {
                }
            },
            { "King's Station", new() },
            { "Kingdom's Edge", new() },  // TRANDO only
            {
                "Lower Greenpath",
                new()
                {
                }
            },
            { "Mantis Village", new() },
            { "Outside Colosseum", new() },
            { "Queen's Gardens", new() },
            { "Queen's Station", new() },
            { "Royal Waterways", new() },
            {
                "Stag Nest",
                new()
                {
                }
            },
            {
                "West Blue Lake",
                new()
                {

                }
            },
            {
                "West Crossroads",
                new()
                {
                }
            },
            { "West Fog Canyon", new() },
            { "West Waterways", new() }
        };

        public static IReadOnlyCollection<ClusterName> GetStartClusters(string start) => ProtectedStartClusters.GetOrDefault(start, () => new());
    }
}
