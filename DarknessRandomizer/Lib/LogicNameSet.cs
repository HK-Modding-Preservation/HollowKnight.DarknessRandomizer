using System.Collections.Generic;

namespace DarknessRandomizer.Lib
{
    public class LogicNameSet
    {
        public bool Inclusive;
        public HashSet<string> Locations;

        private LogicNameSet(bool inclusive, HashSet<string> locations)
        {
            this.Inclusive = inclusive;
            this.Locations = locations;
        }

        public static LogicNameSet All() => new(true, new());
        public static LogicNameSet None() => new(false, new());
        public static LogicNameSet AllOf(params string[] locations) => new(true, new(locations));
        public static LogicNameSet NoneOf(params string[] locations) => new(false, new(locations));

        public bool Contains(string location) => Inclusive == Locations.Contains(location);
    }
}
