using ItemChanger;
using Modding;
using Newtonsoft.Json;
using RandomizerMod.RC;
using System.Collections.Generic;
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

            if (ModHooks.GetMod("CondensedSpoilerLogger") is Mod)
            {
                CondensedSpoilerLogger.API.AddCategory("Lantern Shards", _ => ShatteredLantern, new() { LanternItemName });
            }

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

            ItemChangerMod.Modules.GetOrAdd<DarknessRandomizerModule>().DarknessOverrides = new(LS.DarknessOverrides);
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
