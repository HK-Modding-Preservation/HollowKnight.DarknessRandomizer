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

        private static IEnumerable<int> ToSceneIds(string csv)
        {
            List<int> ret = new();
            foreach (var scene in csv.Split(','))
            {
                if (SceneName.TryGetSceneName(scene, out SceneName sceneName))
                {
                    ret.Add(sceneName.Id);
                }
            }
            return ret;
        }

        public static bool TryGetDarkness(SceneName sceneName, out Darkness d)
        {
            if (RandoInterop.LS != null)
            {
                return RandoInterop.LS.DarknessOverrides.TryGetValue(sceneName, out d);
            }
            else
            {
                return ItemChangerMod.Modules.Get<DarknessRandomizerModule>().DarknessOverrides.TryGetValue(sceneName, out d);
            }
        }

        public override bool TryMatch(LogicManager lm, string term, out LogicInt variable)
        {
            if (Parent.TryMatch(lm, term, out variable)) return true;

            Match match = Regex.Match(term, @"^\$DarknessBrightened\[(.+)\]$");
            if (match.Success)
            {
                variable = new DarknessBrightenedInt(ToSceneIds(match.Groups[1].Value));
                return true;
            }

            match = Regex.Match(term, @"^\$DarknessNotDarkened\[(.+)\]$");
            if (match.Success)
            {
                variable = new DarknessNotDarkenedInt(ToSceneIds(match.Groups[1].Value));
                return true;
            }

            return false;
        }
    }

    internal class DarknessBrightenedInt : LogicInt
    {
        public List<int> sceneIds;

        public DarknessBrightenedInt(IEnumerable<int> sceneIds)
        {
            this.sceneIds = new(sceneIds);
            this.Name = $"$DarknessBrightened[{String.Join(",", sceneIds.Select(SceneName.FromId))}]";
        }

        public override string Name { get; }

        public override IEnumerable<Term> GetTerms() => Enumerable.Empty<Term>();

        public override int GetValue(object sender, ProgressionManager pm)
        {
            bool anyNewBrightness = false;
            foreach (var id in sceneIds)
            {
                var sceneName = SceneName.FromId(id);
                if (DarknessVariableResolver.TryGetDarkness(sceneName, out Darkness d))
                {
                    if (d == Darkness.Dark) {
                        return 0;
                    }
                    else
                    {
                        anyNewBrightness |= sceneName.IsVanillaDark();
                    }
                }
            }

            return anyNewBrightness ? 1 : 0;
        }
    }

    internal class DarknessNotDarkenedInt : LogicInt
    {
        public List<int> sceneIds;

        public DarknessNotDarkenedInt(IEnumerable<int> sceneIds)
        {
            this.sceneIds = new(sceneIds);
            this.Name = $"$DarknessNotDarkened[{String.Join(",", sceneIds.Select(SceneName.FromId))}]";
        }

        public override string Name { get; }

        public override IEnumerable<Term> GetTerms() => Enumerable.Empty<Term>();

        public override int GetValue(object sender, ProgressionManager pm)
        {
            foreach (var id in sceneIds)
            {
                var sceneName = SceneName.FromId(id);
                if (DarknessVariableResolver.TryGetDarkness(sceneName, out Darkness d))
                {
                    if (d == Darkness.Dark && !sceneName.IsVanillaDark())
                    {
                        return 0;
                    }
                }
            }

            return 1;
        }
    }
}
