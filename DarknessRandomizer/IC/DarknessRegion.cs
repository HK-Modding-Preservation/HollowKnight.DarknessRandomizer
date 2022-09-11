using DarknessRandomizer.Data;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace DarknessRandomizer.IC
{
    public class DarknessRegion
    {
        public class Preloader : ItemChanger.Internal.Preloaders.Preloader
        {
            public static Preloader Instance { get; } = new();

            public override IEnumerable<(string, string)> GetPreloadNames()
            {
                yield return (SceneName.GroundsBlueLake.Name(), "Darkness Region (3)");
            }

            private GameObject _darknessRegion;
            public GameObject DarknessRegion => UObject.Instantiate(_darknessRegion);

            public override void SavePreloads(Dictionary<string, Dictionary<string, GameObject>> objectsByScene)
            {
                _darknessRegion = objectsByScene[SceneName.GroundsBlueLake.Name()]["Darkness Region (3)"];
            }
        }
    }
}
