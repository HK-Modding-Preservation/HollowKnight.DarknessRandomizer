using DarknessRandomizer.Lib;
using DarknessRandomizer.Rando;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Data
{
    public class RawSceneMetadata : BaseSceneMetadata<string>
    {
        public static SortedDictionary<string, RawSceneMetadata> LoadFromPath(string path) =>
            JsonUtil.DeserializeFromPath<SortedDictionary<string, RawSceneMetadata>>(path);
    }

    public class RawSceneData : BaseSceneData<string>
    {
        public static SortedDictionary<string, RawSceneData> LoadFromPath(string path) =>
            JsonUtil.DeserializeFromPath<SortedDictionary<string, RawSceneData>>(path);
    }

    public class RawClusterData : BaseClusterData<string, string>
    {
        public static SortedDictionary<string, RawClusterData> LoadFromPath(string path) =>
            JsonUtil.DeserializeFromPath<SortedDictionary<string, RawClusterData>>(path);

        public Dictionary<string, RelativeDarkness> AdjacentClusters = new();

        protected override IEnumerable<KeyValuePair<string, RelativeDarkness>> AdjacentDarkness() {
            foreach (var e in AdjacentClusters)
            {
                yield return new(e.Key, e.Value);
            }
        }

        protected override RelativeDarkness GetAdjacentDarkness(string cluster) => AdjacentClusters[cluster];
    }
}
