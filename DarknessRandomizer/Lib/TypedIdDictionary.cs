using DarknessRandomizer.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Lib
{
    public interface ITypedIdFactory<T> where T : ITypedId
    {
        int Count();

        T FromId(int id);

        T FromName(string name);
    }

    public interface ITypedId
    {
        int Id();

        string Name();
    }

    // A custom wrapper around `Dictionary<Id, Value>` which doesn't serialize correctly when Id is a special type.
    public class TypedIdDictionary<K, V> where K : ITypedId
    {
        private readonly ITypedIdFactory<K> factory;
        private Dictionary<K, V> dict;

        [JsonIgnore]
        public SortedDictionary<string, V> AsSortedDict
        {
            get
            {
                SortedDictionary<string, V> ret = new();
                foreach (var e in dict)
                {
                    ret[e.Key.Name()] = e.Value;
                }
                return ret;
            }
            set
            {
                Clear();
                foreach (var e in value)
                {
                    dict[factory.FromName(e.Key)] = e.Value;
                }
            }
        }

        public TypedIdDictionary(ITypedIdFactory<K> factory) {
            this.factory = factory;
            this.dict = new();
        }

        public TypedIdDictionary(TypedIdDictionary<K, V> other)
        {
            this.factory = other.factory;
            this.dict = new(other.dict);
        }

        public bool TryGetValue(K id, out V value) => dict.TryGetValue(id, out value);

        public void Clear() => dict.Clear();

        public IEnumerable<KeyValuePair<K, V>> Enumerate()
        {
            foreach (var e in dict)
            {
                yield return new(e.Key, e.Value);
            }
        }

        public V this[K k]
        {
            get { return dict[k]; }
            set { dict[k] = value; }
        }
    }

    public class TypedIdDictionaryConverter<K, V, T> : JsonConverter<T> where K : ITypedId where T : TypedIdDictionary<K, V>, new()
    {
        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            T ret = new();
            ret.AsSortedDict = serializer.Deserialize<SortedDictionary<string, V>>(reader);
            return ret;
        }

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.AsSortedDict);
        }
    }
}
