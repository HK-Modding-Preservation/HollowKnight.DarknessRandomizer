using System.Collections.Generic;

namespace DarknessRandomizer.Lib
{
    public class LocationSet
    {
        public bool IsAll;
        public HashSet<string> Locations;

        public LocationSet(bool isAll)
        {
            this.IsAll = isAll;
            this.Locations = new();
        }

        public LocationSet(params string[] locations)
        {
            this.IsAll = false;
            this.Locations = new();

            foreach (var loc in locations)
            {
                this.Locations.Add(loc);
            }
        }

        public static LocationSet None() => new(false);
        public static LocationSet All() => new(true);

        public bool Contains(string location) => IsAll || Locations.Contains(location);
    }
}
