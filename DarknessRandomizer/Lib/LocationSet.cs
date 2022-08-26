using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Lib
{
    public abstract class LocationSet
    {
        public static readonly LocationSet NONE = new NoLocations();
        public static readonly LocationSet ALL = new AllLocations();

        public abstract bool Contains(string location);
    }

    public class NoLocations : LocationSet
    {
        public override bool Contains(string location) => false;
    }

    public class AllLocations : LocationSet
    {

        public override bool Contains(string location) => true;
    }

    public class LocationsList : LocationSet
    {
        public HashSet<string> Locations = new();

        public LocationsList() { }
        public LocationsList(string loc1, params string[] locs)
        {
            Locations.Add(loc1);
            foreach (var loc in locs)
            {
                Locations.Add(loc);
            }
        }

        public override bool Contains(string location) => Locations.Contains(location);
    }

}
