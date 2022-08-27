using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Data
{
    class GraphDataUpdater
    {
        public static void Main(string[] args)
        {
            // Step one: Merge SceneMetadata into SceneData
            var SM = SceneMetadata.LoadAll();
            var SD = SceneData.LoadAll();

            // Add any missing scenes.
            foreach (var scene in SM.Keys)
            {
                if (!SD.ContainsKey(scene))
                {
                    SD[scene] = new();
                }
            }

            // Remove any broken scenes.
            List<string> toRemove = new();
            foreach (var scene in SD.Keys)
            {
                if (!SM.ContainsKey(scene))
                {
                    toRemove.Add(scene);
                }
            }
            toRemove.ForEach(s => SD.Remove(s));

            // Write out the updates SceneData.
            JsonUtil.Serialize(SD, "/DarknessRandomizer/Resources/Data/scene_data.json");
        }
    }
}
