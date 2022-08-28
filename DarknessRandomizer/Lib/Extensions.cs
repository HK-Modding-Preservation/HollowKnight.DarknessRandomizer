using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Lib
{
    public static class Extensions
    {
        public delegate V Supplier<V>();

        public static void AddIfEmpty<K,V>(this IDictionary<K, V> dict, K key, Supplier<V> creator)
        {
            if(!dict.ContainsKey(key))
            {
                dict[key] = creator.Invoke();
            }
        }

        public static V GetOrCreate<K,V>(this IDictionary<K, V> dict, K key) where V : new()
        {
            if (dict.TryGetValue(key, out V value))
            {
                return value;
            }

            return (dict[key] = new());
        }

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
                default:
                    throw new ArgumentException($"Unknown RelativeDarkness {rd}");
            }
        }
    }
}
