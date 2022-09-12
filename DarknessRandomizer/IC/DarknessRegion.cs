using DarknessRandomizer.Data;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace DarknessRandomizer.IC
{
    public record DarknessRegion
    {
        public Darkness Darkness;
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public void Deploy()
        {
            var obj = Preloader.Instance.NewDarknessRegion();
            obj.name = $"CustomDarknessRegion-{X}-{Y}";
            obj.transform.position = new(X, Y, 0);
            obj.transform.localScale = new(1, 1, 1);
            obj.GetComponent<BoxCollider2D>().size = new(Width, Height);
            obj.LocateMyFSM("Darkness Region").FsmVariables.FindFsmInt("Darkness").Value = (int)Darkness;

            obj.SetActive(true);
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
