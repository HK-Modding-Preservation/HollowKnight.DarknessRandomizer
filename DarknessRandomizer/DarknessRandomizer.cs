using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace DarknessRandomizer
{
    public class DarknessRandomizer : Mod
    {
        internal static DarknessRandomizer Instance;

        public DarknessRandomizer() : base("DarknessRandomizer") { }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Instance = this;
        }
    }
}