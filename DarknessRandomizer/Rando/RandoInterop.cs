using ItemChanger;
using Newtonsoft.Json;
using RandomizerMod.RC;
using System.IO;

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
            RandomizerMod.Logging.SettingsLog.AfterLogSettings += LogSettings;
            RandomizerMod.Logging.LogManager.AddLogger(new DarknessLogger());
        }

        public static bool IsEnabled => RandomizeDarkness || ShatteredLantern;

        public static bool RandomizeDarkness => DarknessRandomizer.GS.DarknessRandomizationSettings.RandomizeDarkness;

        public static bool ShatteredLantern => DarknessRandomizer.GS.DarknessRandomizationSettings.ShatteredLantern;

        public static string LanternTermName => RandoPlusInterop.LanternTermName;

        public static string LanternItemName => RandoPlusInterop.LanternItemName;

        public static string LanternShardItemName => RandoPlusInterop.LanternShardItemName;

        private static void Finish(RandoController rc)
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

        private static void LogSettings(RandomizerMod.Logging.LogArguments args, TextWriter tw)
        {
            if (!IsEnabled) return;
            tw.WriteLine("Logging DarknessRando DarknessRandomizationSettings:");
            using JsonTextWriter jtw = new(tw) { CloseOutput = false };
            JsonUtil._js.Serialize(jtw, LS.Settings);
            tw.WriteLine();
        }
    }
}
