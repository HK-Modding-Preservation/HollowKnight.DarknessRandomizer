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
        public static readonly SortedDictionary<string, SceneMetadata> Instance =
            JsonUtil.Deserialize<SortedDictionary<string, SceneMetadata>>("DarknessRandomizer.Resources.Data.scene_metadata.json");

        public String Alias;
        public String MapArea;
        public List<String> AdjacentScenes;
    }

    public class SceneData
    {
        public static readonly SortedDictionary<string, SceneData> Instance =
            JsonUtil.Deserialize<SortedDictionary<string, SceneData>>("DarknessRandomizer.Resources.Data.scene_data.json");

        public string Alias;
        public string Cluster = "UNASSIGNED";
        public Darkness MinimumDarkness = Darkness.Bright;
        public Darkness MaximumDarkness = Darkness.Dark;
        public bool IsVanillaDark = false;
        public LocationSet DifficultSkipLocs = LocationSet.None();
        public LocationSet ProficientSkipLocs = LocationSet.None();
    }

    public class ClusterData
    {
        public static readonly SortedDictionary<string, ClusterData> Instance =
            JsonUtil.Deserialize<SortedDictionary<string, ClusterData>>("DarknessRandomizer.Resources.Data.cluster_data.json");

        public List<string> SceneAliases = new();
        public List<string> SceneNames = new();
        public bool CanBeDarknessSource = true;
        public bool CursedOnly = false;
        public int ProbabilityWeight = 100;
        public int CostWeight = 100;
        public int SemiDarkProbability = 100;
        public SortedDictionary<string, RelativeDarkness> AdjacentClusters = new();
    }
}
