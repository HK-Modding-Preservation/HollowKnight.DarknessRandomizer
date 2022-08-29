using DarknessRandomizer.Lib;
using DarknessRandomizer.Rando;
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

        public static SceneMetadata Get(SceneName sceneName) => Instance[sceneName.Name];

        public string Alias;
        public string MapArea;
        public List<string> AdjacentScenes;
    }

    public class SceneData
    {
        public static readonly SortedDictionary<string, SceneData> Instance =
            JsonUtil.Deserialize<SortedDictionary<string, SceneData>>("DarknessRandomizer.Resources.Data.scene_data.json");

        public static SceneData Get(SceneName sceneName) => Instance[sceneName.Name];

        public string Alias;
        public string Cluster = "UNASSIGNED";
        public Darkness MinimumDarkness = Darkness.Bright;
        public Darkness MaximumDarkness = Darkness.Dark;
        public bool IsVanillaDark = false;
        public LogicNameSet DifficultSkips = LogicNameSet.None();
        public LogicNameSet ProficientSkips = LogicNameSet.None();
    }

    public class DarkSettings
    {
        public int ProbabilityWeight = 100;
        public int CostWeight = 100;
    }

    public class SemiDarkSettings
    {
        public int SemiDarkProbability = 100;
    }

    public class ClusterData
    {
        public static readonly SortedDictionary<string, ClusterData> Instance =
            JsonUtil.Deserialize<SortedDictionary<string, ClusterData>>("DarknessRandomizer.Resources.Data.cluster_data.json");

        public static ClusterData Get(ClusterName clusterName) => Instance[clusterName.Name];

        public List<string> SceneAliases = new();
        public List<string> SceneNames = new();
        public bool? OverrideCannotBeDarknessSource = null;
        public bool? CursedOnly = false;
        public DarkSettings DarkSettings = null;
        public SemiDarkSettings SemiDarkSettings = null;
        public SortedDictionary<string, RelativeDarkness> AdjacentClusters = new();

        public bool CanBeDarknessSource(SortedDictionary<string, SceneData> SD) => CanBeDarknessSource(null, SD);

        public bool CanBeDarknessSource(DarknessRandomizationSettings settings = null, SortedDictionary<string, SceneData> SD = null)
        {
            SD ??= SceneData.Instance;
            if (MaximumDarkness(settings, SD) < Darkness.Dark) return false;
            if (OverrideCannotBeDarknessSource is bool b && b) return false;
            return AdjacentClusters.Values.All(rd => rd != RelativeDarkness.Brighter);
        }

        public Darkness MaximumDarkness(SortedDictionary<string, SceneData> SD) => MaximumDarkness(null, SD);

        public Darkness MaximumDarkness(DarknessRandomizationSettings settings = null, SortedDictionary<string, SceneData> SD = null)
        {
            SD ??= SceneData.Instance;
            var d = Darkness.Bright;
            foreach (var sn in SceneNames)
            {
                Darkness d2 = SD[sn].MaximumDarkness;
                if (d2 > d) d = d2;
            }

            if (d == Darkness.Dark && CursedOnly is bool b && b && settings != null && settings.DarknessLevel != DarknessLevel.Cursed)
            {
                return Darkness.SemiDark;
            }
            return d;
        }

        public Darkness MinimumDarkness(SortedDictionary<string, SceneData> SD = null)
        {
            SD ??= SceneData.Instance;
            var d = Darkness.Dark;
            foreach (var sn in SceneNames)
            {
                Darkness d2 = SD[sn].MinimumDarkness;
                if (d2 < d) d = d2;
            }
            return d;
        }
    }
}
