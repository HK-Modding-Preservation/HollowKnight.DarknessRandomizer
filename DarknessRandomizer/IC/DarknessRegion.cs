using DarknessRandomizer.Data;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace DarknessRandomizer.IC
{
    public class DarknessRegion
    {
        public record Parameters
        {
            public float X;
            public float Y;
            public float Width;
            public float Height;
        }

        public static void Spawn(Parameters parameters)
        {
            var obj = Preloader.Instance.NewDarknessRegion();
            obj.name = $"CustomDarknessRegion-{parameters.X}-{parameters.Y}";
            obj.transform.position = new(parameters.X, parameters.Y, 0);
            obj.transform.localScale = new(1, 1, 1);
            obj.GetComponent<BoxCollider2D>().size = new(parameters.Width, parameters.Height);
            obj.LocateMyFSM("Darkness Region").FsmVariables.FindFsmInt("Darkness").Value = 2;
        }

        public class Preloader : ItemChanger.Internal.Preloaders.Preloader
        {
            public static Preloader Instance { get; } = new();

            public override IEnumerable<(string, string)> GetPreloadNames()
            {
                yield return (SceneName.CliffsMain.Name(), "Darkness Region (3)");
            }

            private GameObject darknessRegionTemplate;
            public GameObject NewDarknessRegion() => UObject.Instantiate(darknessRegionTemplate);

            public override void SavePreloads(Dictionary<string, Dictionary<string, GameObject>> objectsByScene)
            {
                darknessRegionTemplate = objectsByScene[SceneName.CliffsMain.Name()]["Darkness Region (3)"];
            }
        }
    }
}
