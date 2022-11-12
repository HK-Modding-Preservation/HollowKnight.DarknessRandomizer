using DarknessRandomizer.IC;
using PurenailCore.ModUtil;
using UnityEngine;

namespace DarknessRandomizer
{
    public class Preloader : PurenailCore.ModUtil.Preloader
    {
        public static readonly Preloader Instance = new();

        [Preload("Cliffs_01", "Darkness Region (3)")]
        private GameObject _darknessRegion;

        public GameObject NewDarknessRegion()
        {
            var obj = Object.Instantiate(_darknessRegion);
            obj.AddComponent<CustomDarknessRegion>();
            return obj;
        }
    }
}
