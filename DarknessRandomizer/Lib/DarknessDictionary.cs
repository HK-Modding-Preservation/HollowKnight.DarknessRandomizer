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
        // SceneName.Id-indexed list of darkness.
        [JsonIgnore]
        private readonly List<(bool, Darkness)> darknessList;

        public List<KeyValuePair<SceneName, Darkness>> SerializableEntries
        {
            get { return Enumerate().ToList(); }
            set { Clear(); value.ForEach(e => darknessList[e.Key.Id] = (true, e.Value)); }
        }

        public DarknessDictionary() {
            darknessList = new();
            for (int i = 0; i < SceneName.NumSceneNames(); i++)
            {
                darknessList.Add((false, Darkness.Bright));
            }
        }

        public DarknessDictionary(DarknessDictionary other) => darknessList = new(other.darknessList);

        public bool TryGetValue(SceneName sceneName, out Darkness darkness) {
            bool present;
            (present, darkness) = darknessList[sceneName.Id];
            return present;
        }

        public Darkness Get(SceneName sceneName)
        {
            var (present, darkness) = darknessList[sceneName.Id];
            if (!present)
            {
                throw new KeyNotFoundException($"{sceneName}");
            }

            return darkness;
        }

        public void Set(SceneName sceneName, Darkness darkness) => darknessList[sceneName.Id] = (true, darkness);

        public void Clear()
        {
            for (int i = 0; i < darknessList.Count; i++)
            {
                darknessList[i] = (false, Darkness.Bright);
            }
        }

        public IEnumerable<KeyValuePair<SceneName, Darkness>> Enumerate()
        {
            for (int i = 0; i < darknessList.Count; i++)
            {
                var (present, darkness) = darknessList[i];
                if (present)
                {
                    yield return new(SceneName.FromId(i), darkness);
                }
            }
        }

        public Darkness this[SceneName sceneName]
        {
            get { return Get(sceneName); }
            set { Set(sceneName, value); }
        }
    }
}
