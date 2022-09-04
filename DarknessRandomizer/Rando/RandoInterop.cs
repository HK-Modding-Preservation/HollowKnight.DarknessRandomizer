using DarknessRandomizer.IC;
using ItemChanger;
using RandomizerMod.RC;

namespace DarknessRandomizer.Rando
{
    public static class RandoInterop
    {
        public static LocalSettings LS { get; set; }

        public static void Setup()
        {
            ConnectionMenu.Setup();
            LogicPatcher.Setup();
            RequestModifier.Setup();

            RandoController.OnExportCompleted += Finish;
            RandomizerMod.Logging.LogManager.AddLogger(new DarknessLogger());
        }

        public static bool IsEnabled => RandomizeDarkness || ShatteredLantern;

        public static bool RandomizeDarkness => DarknessRandomizer.GS.DarknessRandomizationSettings.RandomizeDarkness;

        public static bool ShatteredLantern => DarknessRandomizer.GS.DarknessRandomizationSettings.ShatteredLantern;

        public static string LanternTermName => RandoPlusInterop.NoLantern ? "NOLANTERN" : "LANTERN";

        public static string LanternItemName => RandoPlusInterop.NoLantern ? RandoPlus.Consts.NoLantern : ItemNames.Lumafly_Lantern;

        public static string LanternShardItemName => RandoPlusInterop.NoLantern ? NoLanternShardItem.ItemName : LanternShardItem.ItemName;

        public static void Finish(RandoController rc)
        {
            if (!IsEnabled) return;

            var drm = ItemChangerMod.Modules.GetOrAdd<DarknessRandomizerModule>();
            drm.DarknessOverrides = new(LS.DarknessOverrides);

            if (RandomizeDarkness)
            {
                var dlem = ItemChangerMod.Modules.GetOrAdd<ItemChanger.Modules.DarknessLevelEditModule>();
                foreach (var entry in LS.DarknessOverrides.Enumerate())
                {
                    dlem.darknessLevelsByScene[entry.Key.Name()] = (int)entry.Value;
                }
            }
        }
    }
}
