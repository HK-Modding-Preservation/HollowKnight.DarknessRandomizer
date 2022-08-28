using DarknessRandomizer.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Data
{
    public class SceneMetadata
    {
        public String Alias;
        public String MapArea;
        public List<String> AdjacentScenes;

        public static SortedDictionary<string, SceneMetadata> LoadAll() => JsonUtil.Deserialize<SortedDictionary<string, SceneMetadata>>("DarknessRandomizer.Resources.Data.scene_metadata.json");
    }

    public class SceneData
    {
        public string Alias;
        public string Cluster = "UNASSIGNED";
        public Darkness MinimumDarkness = Darkness.Bright;
        public Darkness MaximumDarkness = Darkness.Dark;
        public LocationSet DifficultSkipLocs = LocationSet.None();
        public LocationSet ProficientSkipLocs = LocationSet.None();

        public static SortedDictionary<string, SceneData> LoadAll() => JsonUtil.Deserialize<SortedDictionary<string, SceneData>>("DarknessRandomizer.Resources.Data.scene_data.json");
    }

    public class ClusterData
    {
        public List<string> SceneAliases = new();
        public List<string> SceneNames = new();
        public bool CanBeDarknessSource = true;
        public bool CursedOnly = false;
        public int ProbabilityWeight = 100;
        public int CostWeight = 100;
        public int SemiDarkProbability = 100;
        public SortedDictionary<string, RelativeDarkness> AdjacentClusters = new();

        public static SortedDictionary<string, ClusterData> LoadAll() => JsonUtil.Deserialize<SortedDictionary<string, ClusterData>>("DarknessRandomizer.Resources.Data.cluster_data.json");
    }
}
