using DarknessRandomizer.Lib;
using DarknessRandomizer.Rando;
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
        public static readonly Graph Graph = Graph.Instance;

        public DarknessRandomizer() : base("DarknessRandomizer")
        {
            Instance = this;

            Version v = typeof(DarknessRandomizer).Assembly.GetName().Version;
            Version = $"{v.Major}.{v.Minor}.{v.Build}";
        }

        public override void Initialize()
        {
            if (ModHooks.GetMod("Randomizer 4") is Mod)
            {
                RandoInterop.Setup();
            }
        }

        public void OnLoadGlobal(GlobalSettings s) => GS = s ?? new();

        public GlobalSettings OnSaveGlobal() => GS ?? new();

        private string Version { get; }
        public override string GetVersion() => Version;
    }
}