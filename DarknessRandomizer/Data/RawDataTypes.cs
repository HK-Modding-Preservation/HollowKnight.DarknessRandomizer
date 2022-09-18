﻿using System.Collections.Generic;

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

        public SortedDictionary<string, string> SceneNames = new();

        public SortedDictionary<string, RelativeDarkness> AdjacentClusters = new();

        public override int SceneCount => SceneNames.Count;

        protected override IEnumerable<string> EnumerateSceneNames() => SceneNames.Keys;

        protected override IEnumerable<KeyValuePair<string, RelativeDarkness>> EnumerateRelativeDarkness() {
            foreach (var e in AdjacentClusters)
            {
                yield return new(e.Key, e.Value);
            }
        }
    }
}
