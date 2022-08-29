using Newtonsoft.Json;
using System.Collections.Generic;

namespace DarknessRandomizer.Lib
{
    public class LogicNameSet
    {
        public bool Exclusive;
        public HashSet<string> Locations;

        private LogicNameSet(bool exclusive, HashSet<string> locations)
        {
            this.Exclusive = exclusive;
            this.Locations = locations;
        }

        [JsonConstructorAttribute]
        LogicNameSet() { }


        public bool Empty => !Exclusive && Locations.Count == 0;

        public static LogicNameSet All() => new(false, new());
        public static LogicNameSet None() => new(true, new());
        public static LogicNameSet AllOf(params string[] locations) => new(false, new(locations));
        public static LogicNameSet NoneOf(params string[] locations) => new(true, new(locations));

        public bool Contains(string location) => Exclusive != Locations.Contains(location);
    }
}
