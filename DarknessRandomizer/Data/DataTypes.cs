using DarknessRandomizer.Lib;
using DarknessRandomizer.Rando;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Data
{
    public class SceneMetadata : BaseSceneMetadata<SceneName>
    {
        private static readonly SceneDictionary<SceneMetadata> data = JsonUtil.DeserializeEmbedded<SceneDictionary<SceneMetadata>>(
                "DarknessRandomizer.Resources.Data.scene_metadata.json");

        public static SceneMetadata Get(SceneName sceneName) => data[sceneName];

        public static void Load() { DarknessRandomizer.Log("Loaded SceneMetadata");  }
    }

    public class SceneData : BaseSceneData<ClusterName>
    {
        private static readonly SceneDictionary<SceneData> data = JsonUtil.DeserializeEmbedded<SceneDictionary<SceneData>>(
                "DarknessRandomizer.Resources.Data.scene_data.json");

        public static SceneData Get(SceneName sceneName) => data[sceneName];

        public static void Load() { DarknessRandomizer.Log("Loaded SceneData"); }
    }

    public class ClusterData : BaseClusterData<SceneName, ClusterName>
    {
        private static readonly ClusterDictionary<ClusterData> data = JsonUtil.DeserializeEmbedded<ClusterDictionary<ClusterData>>(
                "DarknessRandomizer.Resources.Data.cluster_data.json");

        public ClusterDictionary<RelativeDarkness> AdjacentClusters = new();

        public static ClusterData Get(ClusterName clusterName) => data[clusterName];

        protected override IEnumerable<KeyValuePair<ClusterName, RelativeDarkness>> AdjacentDarkness() => AdjacentClusters.Enumerate();

        protected override RelativeDarkness GetAdjacentDarkness(ClusterName cluster) => AdjacentClusters[cluster];

        public static void Load() { DarknessRandomizer.Log("Loaded ClusterData"); }

        public bool CanBeDarknessSource(DarknessRandomizationSettings settings) => base.CanBeDarknessSource(SceneData.Get, settings);

        public Darkness MaximumDarkness(DarknessRandomizationSettings settings) => base.MaximumDarkness(SceneData.Get, settings);

        public Darkness MinimumDarkness() => base.MinimumDarkness(SceneData.Get);
    }
}
