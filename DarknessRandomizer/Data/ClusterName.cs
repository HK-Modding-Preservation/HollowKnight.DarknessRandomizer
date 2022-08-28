using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DarknessRandomizer.Data
{
    [JsonConverter(typeof(ClusterNameConverter))]
    public class ClusterName
    {
        private static readonly Dictionary<string, ClusterName> clustersByName = new();
        private static readonly List<ClusterName> clustersById = new();
        private static int NextId = 0;

        public readonly string Name;
        public readonly int Id;

        private ClusterName(string name)
        {
            if (clustersByName.ContainsKey(name))
            {
                throw new ArgumentException($"Duplicate cluster: {name}");
            }

            this.Name = name;
            this.Id = NextId++;
            clustersByName.Add(name, this);
            clustersById.Add(this);
        }

        public static bool TryGetClusterName(string s, out ClusterName cluster) => clustersByName.TryGetValue(s, out cluster);

        public static ClusterName FromId(int id) => clustersById[id];

        public static bool IsScene(string s) => clustersByName.ContainsKey(s);

        public override string ToString() => Name;

        public static int NumSceneNames() => clustersById.Count;

        public override bool Equals(object obj)
        {
            return obj is ClusterName name &&
                   Id == name.Id;
        }

        public override int GetHashCode() => Id;

        // @@@ INSERT_CLUSTER_NAMES START @@@
        public static readonly ClusterName AbyssBirthplace = new("AbyssBirthplace");
        public static readonly ClusterName AbyssCore = new("AbyssCore");
        public static readonly ClusterName AbyssLifebloodCore = new("AbyssLifebloodCore");
        public static readonly ClusterName AbyssShadeCloakWing = new("AbyssShadeCloakWing");
        public static readonly ClusterName AbyssShriekWing = new("AbyssShriekWing");
        public static readonly ClusterName BasinBrokenVesselWing = new("BasinBrokenVesselWing");
        public static readonly ClusterName BasinCloth = new("BasinCloth");
        public static readonly ClusterName BasinFountain = new("BasinFountain");
        public static readonly ClusterName BasinPalaceWing = new("BasinPalaceWing");
        public static readonly ClusterName CityBridgeToBasin = new("CityBridgeToBasin");
        public static readonly ClusterName CliffsBaldurShell = new("CliffsBaldurShell");
        public static readonly ClusterName DeepnestTramWing = new("DeepnestTramWing");
        public static readonly ClusterName EdgeTramEntrance = new("EdgeTramEntrance");
        public static readonly ClusterName FungalClothCorridor = new("FungalClothCorridor");
        public static readonly ClusterName FungalElderHuWing = new("FungalElderHuWing");
        public static readonly ClusterName FungalEntrance = new("FungalEntrance");
        public static readonly ClusterName FungalOgres = new("FungalOgres");
        public static readonly ClusterName FungalQueensStation = new("FungalQueensStation");
        public static readonly ClusterName FungalUpper = new("FungalUpper");
        public static readonly ClusterName GreenpathMMC = new("GreenpathMMC");
        public static readonly ClusterName GreenpathStoneSanctuary = new("GreenpathStoneSanctuary");
        public static readonly ClusterName GreenpathStoneSanctuaryApproach = new("GreenpathStoneSanctuaryApproach");
        public static readonly ClusterName GreenpathStoneSanctuaryBench = new("GreenpathStoneSanctuaryBench");
        public static readonly ClusterName GreenpathUnn = new("GreenpathUnn");
        public static readonly ClusterName GreenpathUnnBridge = new("GreenpathUnnBridge");
        public static readonly ClusterName GreenpathUpper = new("GreenpathUpper");
        public static readonly ClusterName GreenpathWest = new("GreenpathWest");
        // @@@ INSERT_CLUSTER_NAMES END @@@
    }

    class ClusterNameConverter : JsonConverter<ClusterName>
    {
        public override ClusterName ReadJson(JsonReader reader, Type objectType, ClusterName existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (serializer.Deserialize(reader, typeof(string)) is string name && ClusterName.TryGetClusterName(name, out ClusterName clusterName))
            {
                return clusterName;
            }
            throw new JsonReaderException("Error decoding ClusterName");
        }

        public override void WriteJson(JsonWriter writer, ClusterName value, JsonSerializer serializer) => serializer.Serialize(writer, value.Name);
    }
}
