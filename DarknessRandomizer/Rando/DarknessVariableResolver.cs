using DarknessRandomizer.Data;
using DarknessRandomizer.Lib;
using ItemChanger;
using Newtonsoft.Json;
using RandomizerCore.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DarknessRandomizer.Rando
{
    public class DarknessVariableResolver : VariableResolver
    {
        public VariableResolver Parent;

        public DarknessVariableResolver(VariableResolver parent)
        {
            this.Parent = parent;
        }

        [JsonConstructorAttribute]
        DarknessVariableResolver() { }

        public static readonly HashSet<string> VanillaDarkScenes = new()
        {
            Scenes.CliffsJonisDark,
            Scenes.CrossroadsPeakDarkToll,
            Scenes.CrystalDarkRoom,
            Scenes.DeepnestFirstDevout,
            Scenes.DeepnestMidwife,
            Scenes.DeepnestOutsideGalien,
            Scenes.DeepnestOutsideMaskMaker,
            Scenes.DeepnestWhisperingRoot,
            Scenes.GreenpathStoneSanctuary
        };

        public static bool TryGetDarkness(string scene, out Darkness d)
        {
            if (RandoInterop.LS != null)
            {
                return RandoInterop.LS.DarknessOverrides.TryGetValue(scene, out d);
            }
            else
            {
                return ItemChangerMod.Modules.Get<DarknessRandomizerModule>().DarknessOverrides.TryGetValue(scene, out d);
            }
        }

        public override bool TryMatch(LogicManager lm, string term, out LogicInt variable)
        {
            if (Parent.TryMatch(lm, term, out variable)) return true;

            Match match = Regex.Match(term, @"^\$DarknessBrightened\[(.+)\]$");
            if (match.Success)
            {
                variable = new DarknessBrightenedInt(match.Groups[1].Value.Split(',').ToArray());
                return true;
            }

            match = Regex.Match(term, @"^\$DarknessNotDarkened\[(.+)\]$");
            if (match.Success)
            {
                variable = new DarknessNotDarkenedInt(match.Groups[1].Value.Split(',').ToArray());
                return true;
            }

            return false;
        }
    }

    internal class DarknessBrightenedInt : LogicInt
    {
        // TODO: Improve efficieny by converting scenes into ints, and doing an array lookup.
        public List<string> scenes;

        public DarknessBrightenedInt(IEnumerable<string> scenes)
        {
            this.scenes = new(scenes);
            this.Name = $"$DarknessBrightened[{String.Join(",", scenes)}]";
        }

        public override string Name { get; }

        public override IEnumerable<Term> GetTerms() => Enumerable.Empty<Term>();

        public override int GetValue(object sender, ProgressionManager pm)
        {
            bool anyNewBrightness = false;
            foreach (var scene in scenes)
            {
                if (DarknessVariableResolver.TryGetDarkness(scene, out Darkness d))
                {
                    if (d == Darkness.Dark) {
                        return 0;
                    }
                    else
                    {
                        anyNewBrightness |= DarknessVariableResolver.VanillaDarkScenes.Contains(scene);
                    }
                }
            }

            return anyNewBrightness ? 1 : 0;
        }
    }

    internal class DarknessNotDarkenedInt : LogicInt
    {
        public List<string> scenes;

        public DarknessNotDarkenedInt(IEnumerable<string> scenes)
        {
            this.scenes = new(scenes);
            this.Name = $"$DarknessNotDarkened[{String.Join(",", scenes)}]";
        }

        public override string Name { get; }

        public override IEnumerable<Term> GetTerms() => Enumerable.Empty<Term>();

        public override int GetValue(object sender, ProgressionManager pm)
        {
            foreach (var scene in scenes)
            {
                if (DarknessVariableResolver.TryGetDarkness(scene, out Darkness d))
                {
                    if (d == Darkness.Dark && !DarknessVariableResolver.VanillaDarkScenes.Contains(scene))
                    {
                        return 0;
                    }
                }
            }

            return 1;
        }
    }
}
