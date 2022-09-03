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

        [JsonConstructor]
        DarknessVariableResolver() { }

        public static bool TryGetDarkness(SceneName sceneName, out Darkness darkness)
        {
            // FIXME: Figure out how to make this faster; it incurs a non-trivial performance penalty on randomization.
            if (RandoInterop.LS != null)
            {
                return RandoInterop.LS.DarknessOverrides.TryGetValue(sceneName, out darkness);
            }
            else
            {
                return ItemChangerMod.Modules.Get<DarknessRandomizerModule>().DarknessOverrides.TryGetValue(sceneName, out darkness);
            }
        }

        public override bool TryMatch(LogicManager lm, string term, out LogicInt variable)
        {
            if (Parent.TryMatch(lm, term, out variable)) return true;

            Match match = Regex.Match(term, @"^\$DarknessLevel\[(.+)\]$");
            if (match.Success && SceneName.TryGetValue(match.Groups[1].Value, out SceneName sceneName))
            {
                variable = new DarknessLevelInt(sceneName);
                return true;
            }

            return false;
        }
    }

    internal class DarknessLevelInt : LogicInt
    {
        public SceneName sceneName;

        public DarknessLevelInt(SceneName sceneName)
        {
            this.sceneName = sceneName;
            this.Name = $"$DarknessLevel[{sceneName}]";
        }

        public override string Name { get; }

        public override IEnumerable<Term> GetTerms() => Enumerable.Empty<Term>();

        public override int GetValue(object sender, ProgressionManager pm)
        {
            if (DarknessVariableResolver.TryGetDarkness(sceneName, out Darkness d))
            {
                return (int)d;
            } else
            {
                return (int)Darkness.Bright;
            }
        }
    }
}
