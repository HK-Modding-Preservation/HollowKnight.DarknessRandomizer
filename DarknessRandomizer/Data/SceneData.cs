using DarknessRandomizer.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Data
{
    public class SceneData
    {
        public string Cluster = "UNASSIGNED";
        public Darkness MinimumDarkness = Darkness.Bright;
        public Darkness MaximumDarkness = Darkness.Dark;
        public LocationSet DifficultSkipLocs = LocationSet.None();
        public LocationSet ProficientSkipLocs = LocationSet.None();

        public static SortedDictionary<string, SceneData> LoadAll() => JsonUtil.Deserialize<SortedDictionary<string, SceneData>>("DarknessRandomizer.Resources.Data.scene_data.json");
    }
}
