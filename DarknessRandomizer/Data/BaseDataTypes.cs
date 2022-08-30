using DarknessRandomizer.Lib;
using DarknessRandomizer.Rando;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Data
{
    // Because we use compiled identifier types, and the code which updates those types also depends on them, we need two classes
    // of data types in order to bootstrap. One, DataTypes.cs, uses the strongly-typed identifiers, where as RawDataTypes.cs uses
    // only strings. The graph updated code uses raw data types, whereas the rest uses DataTypes.cs.
    //
    // Both sets of classes load from the same json files and therefore must have compatible serialization as well.

    public class SceneDictionary<V> : TypedIdDictionary<SceneName, V>
    {
        public SceneDictionary() : base(SceneName.Factory) { }
        public SceneDictionary(SceneDictionary<V> other) : base(other) { }
    }
    public class ClusterDictionary<V> : TypedIdDictionary<ClusterName, V>
    {
        public ClusterDictionary() : base(ClusterName.Factory) { }
        public ClusterDictionary(ClusterDictionary<V> other) : base(other) { }
    }

    public class BaseSceneMetadata<S>
    {
        public string Alias;
        public string MapArea;
        public List<S> AdjacentScenes;
    }

    public class BaseSceneData<C>
    {
        public string Alias;
        public C Cluster;
        public Darkness MinimumDarkness = Darkness.Bright;
        public Darkness MaximumDarkness = Darkness.Dark;
        public bool IsVanillaDark = false;

        public Darkness ClampDarkness(Darkness d)
        {
            if (d < MinimumDarkness) return MinimumDarkness;
            if (d > MaximumDarkness) return MaximumDarkness;
            return d;
        }
    }

    public enum RelativeDarkness
    {
        Unspecified,
        Brighter,
        Any,
        Darker,
        Disconnected
    }

    public class DarkSettings
    {
        public int ProbabilityWeight = 100;
        public int CostWeight = 100;
    }

    public class SemiDarkSettings
    {
        public int SemiDarkProbability = 100;
    }

    public abstract class BaseClusterData<S, C>
    {
        public List<string> SceneAliases = new();
        public List<S> SceneNames = new();
        public bool? OverrideCannotBeDarknessSource = null;
        public bool? CursedOnly = false;
        public DarkSettings DarkSettings = null;
        public SemiDarkSettings SemiDarkSettings = null;

        public delegate BaseSceneData<C> SceneLookup(S scene);

        protected abstract IEnumerable<KeyValuePair<C, RelativeDarkness>> AdjacentDarkness();

        protected abstract RelativeDarkness GetAdjacentDarkness(C cluster);

        [JsonIgnore]
        public int ProbabilityWeight => DarkSettings?.ProbabilityWeight ?? 100;

        [JsonIgnore]
        public int CostWeight => DarkSettings?.CostWeight ?? 100;

        [JsonIgnore]
        public int SemiDarkProbability => SemiDarkSettings?.SemiDarkProbability ?? 100;

        public bool CanBeDarknessSource(SceneLookup SL, DarknessRandomizationSettings settings = null)
        {
            if (MaximumDarkness(SL, settings) < Darkness.Dark) return false;
            if (OverrideCannotBeDarknessSource is bool b && b) return false;
            return AdjacentDarkness().All(e => e.Value != RelativeDarkness.Brighter);
        }

        public Darkness MaximumDarkness(SceneLookup SL, DarknessRandomizationSettings settings = null)
        {
            var d = Darkness.Bright;
            foreach (var sn in SceneNames)
            {
                Darkness d2 = SL.Invoke(sn).MaximumDarkness;
                if (d2 > d) d = d2;
            }

            if (d == Darkness.Dark && CursedOnly is bool b && b && settings != null && settings.DarknessLevel != DarknessLevel.Cursed)
            {
                return Darkness.SemiDark;
            }
            return d;
        }

        public Darkness MinimumDarkness(SceneLookup SL)
        {
            var d = Darkness.Dark;
            foreach (var sn in SceneNames)
            {
                Darkness d2 = SL.Invoke(sn).MinimumDarkness;
                if (d2 < d) d = d2;
            }
            return d;
        }
    }

    public static class DataExtensions
    {
        public static RelativeDarkness Opposite(this RelativeDarkness rd)
        {
            switch (rd)
            {
                case RelativeDarkness.Any:
                    return RelativeDarkness.Any;
                case RelativeDarkness.Brighter:
                    return RelativeDarkness.Darker;
                case RelativeDarkness.Darker:
                    return RelativeDarkness.Brighter;
                case RelativeDarkness.Unspecified:
                    return RelativeDarkness.Unspecified;
                case RelativeDarkness.Disconnected:
                    return RelativeDarkness.Disconnected;
                default:
                    throw new ArgumentException($"Unknown RelativeDarkness {rd}");
            }
        }
    }
}
