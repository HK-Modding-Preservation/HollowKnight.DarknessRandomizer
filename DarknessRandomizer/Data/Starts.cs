using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Data
{
    public static class Starts
    {
        // FIXME: Add clusters
        // Some of these are transition-rando only, and so won't benefit from clusters outside the area
        private static readonly Dictionary<String, HashSet<Cluster>> ProtectedStartClusters = new()
        {
            { "Abyss", new() },
            { "Ancestral Mound", new() },
            { "City Storerooms", new() },
            { "City of Tears", new() },
            { "Crystallized Mound", new() },
            { "Distant Village", new() },
            { "East Blue Lake", new() },
            { "East Crossroads", new() },
            { "East Fog Canyon", new() },
            { "Far Greenpath", new() },
            { "Fungal Core", new() },
            { "Fungal Wastes", new() },
            { "Greenpath", new() {
                Cluster.GreenpathCliffsBridge,
                Cluster.GreenpathLower,
                Cluster.GreenpathOutsideNoEyes,
                Cluster.GreenpathUpper,
                Cluster.GreenpathWest } },
            { "Hallownest's Crown", new() },
            { "Hive", new() },
            { "King's Pass", new() },
            { "King's Station", new() },
            { "Kingdom's Edge", new() },
            { "Lower Greenpath", new() {
                Cluster.FogCanyonWest,
                Cluster.GreenpathLower,
                Cluster.GreenpathMMC,
                Cluster.GreenpathOutsideNoEyes,
                Cluster.GreenpathUpper } },
            { "Mantis Village", new() },
            { "Outside Colosseum", new() },
            { "Queen's Gardens", new() },
            { "Queen's Station", new() },
            { "Royal Waterways", new() },
            { "Stag Nest", new() },
            { "West Blue Lake", new() },
            { "West Crossroads", new() },
            { "West Fog Canyon", new() },
            { "West Waterways", new() }
        };

        public static IReadOnlyCollection<Cluster> GetStartClusters(string start) => ProtectedStartClusters[start];
    }
}
