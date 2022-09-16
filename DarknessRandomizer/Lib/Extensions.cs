using System;
using System.Collections.Generic;

namespace DarknessRandomizer.Lib
{
    public static class Extensions
    {
        public delegate V Supplier<V>();

        public static bool HasLantern(this PlayerData self) => self.GetBool(nameof(self.hasLantern));

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

        public static void Shuffle<T>(this List<T> list, Random r)
        {
            for (int i = 0; i < list.Count - 1; ++i)
            {
                int j = i + r.Next(0, list.Count - i);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}
