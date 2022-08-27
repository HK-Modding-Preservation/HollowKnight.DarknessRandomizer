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
}
