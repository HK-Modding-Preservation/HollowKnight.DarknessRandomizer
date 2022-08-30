﻿using DarknessRandomizer.Data;
using DarknessRandomizer.Lib;
using RandomizerMod.RandomizerData;
using System.Collections.Generic;

// Node-based logic for determining Hallownest darkness during randomization.
// This does not deal with logic-overrides of any kind.
namespace DarknessRandomizer.Rando
{
    public class LocalSettings
    {
        public DarknessRandomizationSettings Settings;
        public SceneDictionary<Darkness> DarknessOverrides;
        public AlgorithmStats Stats;

        public LocalSettings(int seed, StartDef startDef)
        {
            Settings = DarknessRandomizer.GS.DarknessRandomizationSettings.Clone();

            Algorithm algorithm = new(seed, startDef, Settings);
            algorithm.SelectDarknessLevels(out DarknessOverrides, out Stats);
        }
    }
}