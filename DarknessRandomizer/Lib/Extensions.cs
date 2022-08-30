using DarknessRandomizer.Data;
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

        public static V GetOrAddNew<K,V>(this IDictionary<K, V> dict, K key) where V : new()
        {
            if (dict.TryGetValue(key, out V value))
            {
                return value;
            }

            return (dict[key] = new());
        }

        public static V GetOrDefault<K,V>(this IDictionary<K, V> dict, K key, Supplier<V> creator)
        {
            if (dict.TryGetValue(key, out V value))
            {
                return value;
            }

            return creator.Invoke();
        }
    }
}
