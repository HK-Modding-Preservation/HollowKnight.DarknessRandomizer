using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Lib
{
    public interface LocationSet
    {
        bool Contains(String location);
    }

    public class NoLocations : LocationSet
    {
        public static NoLocations Instance = new();
        public bool Contains(string location) => false;
    }

    public class AllLocations : LocationSet
    {
        public static AllLocations Instance = new();

        public bool Contains(string location) => true;
    }

    public class LocationsList : LocationSet
    {
        public HashSet<String> Locations = new();

        public bool Contains(string location) => Locations.Contains(location);
    }

}
