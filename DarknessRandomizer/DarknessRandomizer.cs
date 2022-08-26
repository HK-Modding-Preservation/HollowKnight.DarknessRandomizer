using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace DarknessRandomizer
{
    public class DarknessRandomizer : Mod, IGlobalSettings<GlobalSettings>
    {
        public static DarknessRandomizer Instance { get; private set; }
        public static GlobalSettings GS { get; private set; } = new();

        public DarknessRandomizer() : base("DarknessRandomizer") { }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Instance = this;
        }

        public void OnLoadGlobal(GlobalSettings s)
        {
            GS = s ?? new();
        }

        public GlobalSettings OnSaveGlobal()
        {
            return GS ?? new();
        }
    }
}