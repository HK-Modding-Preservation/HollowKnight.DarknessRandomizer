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
    }

    public interface ITypedId
    {
        int Id();
    }

    // A custom wrapper around `Dictionary<Id, Value>` which doesn't serialize correctly when Id is a special type.
    public class TypedIdDictionary<K, V> where K : ITypedId
    {
        private readonly ITypedIdFactory<K> factory;
        private Dictionary<K, V> dict;

        public List<KeyValuePair<K, V>> SerializableEntries
        {
            get { return Enumerate().ToList(); }
            set
            {
                Clear();
                value.ForEach(e => dict[e.Key] = e.Value);
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
}
