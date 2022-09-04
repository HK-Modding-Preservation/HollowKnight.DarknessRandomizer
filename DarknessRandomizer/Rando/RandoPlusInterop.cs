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

        public static bool NoLantern => ModInstalled && InternalRandoPlusInterop.NoLantern;

        public static string LanternTermName => ModInstalled ? InternalRandoPlusInterop.LanternTermName : "LANTERN";

        public static string LanternItemName => ModInstalled ? InternalRandoPlusInterop.LanternItemName : ItemNames.Lumafly_Lantern;

        public static string LanternShardItemName => ModInstalled ? InternalRandoPlusInterop.LanternShardItemName : LanternShardItem.ItemName;

        public static void DefineICRefs()
        {
            if (ModInstalled)
            {
                InternalRandoPlusInterop.DefineICRefs();
            }
        }
    }
}
