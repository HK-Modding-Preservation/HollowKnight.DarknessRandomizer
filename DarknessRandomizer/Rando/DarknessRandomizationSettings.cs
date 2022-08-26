using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Rando
{
    public enum DarknessLevel
    {
        Vanilla,
        Dim,
        Dark,
        Cursed
    }

    public class DarknessRandomizationSettings
    {
        public DarknessLevel DarknessLevel = DarknessLevel.Vanilla;

        public DarknessRandomizationSettings Clone()
        {
            return (DarknessRandomizationSettings)MemberwiseClone();
        }
    }
}
