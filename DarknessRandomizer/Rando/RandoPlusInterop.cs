using DarknessRandomizer.IC;
using ItemChanger;
using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Rando
{
    public static class RandoPlusInterop
    {
        public static bool ModInstalled => ModHooks.GetMod("RandoPlus") is Mod;

        public static bool NoLantern => ModInstalled && RandoPlus.RandoPlus.GS.NoLantern;

        public static void DefineICRefs()
        {
            if (ModInstalled)
            {
                NoLanternShardItem.DefineICRefs();
            }
        }
    }
}
