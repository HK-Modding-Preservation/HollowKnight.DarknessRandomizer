using DarknessRandomizer.Lib;
using DarknessRandomizer.Rando;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace DarknessRandomizer.Data
{
    public class SceneMetadata : BaseSceneMetadata<SceneName>
    {
        [JsonConverter(typeof(TypedIdDictionaryConverter<SceneName, SceneMetadata, SMDict>))]
        public class SMDict : SceneDictionary<SceneMetadata> { }

        private static readonly SMDict data = JsonUtil.DeserializeEmbedded<SMDict>(
                "DarknessRandomizer.Resources.Data.scene_metadata.json");

        public static SceneMetadata Get(SceneName sceneName) => data[sceneName];

        public static void Load() { DarknessRandomizer.Log("Loaded SceneMetadata");  }
    }

    public class SceneData : BaseSceneData<ClusterName>
    {
        [JsonConverter(typeof(TypedIdDictionaryConverter<SceneName, SceneData, SDDict>))]
        public class SDDict : SceneDictionary<SceneData> { }

        private static readonly SDDict data = JsonUtil.DeserializeEmbedded<SDDict>(
                "DarknessRandomizer.Resources.Data.scene_data.json");

        public static SceneData Get(SceneName sceneName) => data[sceneName];

        public static void Load() { DarknessRandomizer.Log("Loaded SceneData"); }
    }

    public class ClusterData : BaseClusterData<SceneName, ClusterName>
    {
        [JsonConverter(typeof(TypedIdDictionaryConverter<ClusterName, ClusterData, CDDict>))]
        public class CDDict : ClusterDictionary<ClusterData> { }

        [JsonConverter(typeof(TypedIdDictionaryConverter<ClusterName, RelativeDarkness, RDDict>))]
        public class RDDict : ClusterDictionary<RelativeDarkness> { }

        private static readonly CDDict data = JsonUtil.DeserializeEmbedded<CDDict>(
                "DarknessRandomizer.Resources.Data.cluster_data.json");

        public RDDict AdjacentClusters = new();

        public static ClusterData Get(ClusterName clusterName) => data[clusterName];

        public static ClusterData Get(SceneName sceneName) => data[SceneData.Get(sceneName).Cluster];

        protected override IEnumerable<KeyValuePair<ClusterName, RelativeDarkness>> AdjacentDarkness() => AdjacentClusters.Enumerate();

        protected override RelativeDarkness GetAdjacentDarkness(ClusterName cluster) => AdjacentClusters[cluster];

        public bool IsInWhitePalace => SceneNames.Any(s => SceneMetadata.Get(s).MapArea == "White Palace");

        public bool IsInPathOfPain => SceneNames.Any(s => SceneMetadata.Get(s).Alias.StartsWith("POP_"));

        public static void Load() { DarknessRandomizer.Log("Loaded ClusterData"); }

        public bool CanBeDarknessSource(DarknessRandomizationSettings settings) => base.CanBeDarknessSource(SceneData.Get, settings);

        public Darkness MaximumDarkness(DarknessRandomizationSettings settings) => base.MaximumDarkness(SceneData.Get, settings);

        public Darkness MinimumDarkness() => base.MinimumDarkness(SceneData.Get);
    }
}
