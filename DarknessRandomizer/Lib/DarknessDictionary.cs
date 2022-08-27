using DarknessRandomizer.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Lib
{
    // A custom wrapper around `Dictionary<SceneName, Darkness>` which doesn't serialize correctly.
    public class DarknessDictionary
    {
        [JsonIgnore]
        public Dictionary<SceneName, Darkness> Dict = new();

        public DarknessDictionary() { }

        public DarknessDictionary(DarknessDictionary other) => Dict = new(other.Dict);

        List<KeyValuePair<SceneName, Darkness>> SerializedDict
        {
            get { return Dict.ToList();  }
            set { Dict = value.ToDictionary(e => e.Key, e => e.Value);  }
        }

        public bool TryGetValue(SceneName sceneName, out Darkness darkness) => Dict.TryGetValue(sceneName, out darkness);

        public Darkness this[SceneName sceneName]
        {
            get { return Dict[sceneName]; }
            set { Dict[sceneName] = value; }
        }
    }
}
