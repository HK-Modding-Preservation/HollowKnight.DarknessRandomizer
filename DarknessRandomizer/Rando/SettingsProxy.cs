using RandoSettingsManager.SettingsManagement;
using RandoSettingsManager.SettingsManagement.Versioning;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Rando
{
    public class SettingsProxy : RandoSettingsProxy<RandomizationSettings, string>
    {
        public static readonly SettingsProxy Instance = new SettingsProxy();

        internal class Policy : VersioningPolicy<string>
        {
            public static readonly Policy Instance = new();

            public override string Version => RandomizationSettings.Version;

            public override bool Allow(string version) => version == Version;
        }

        public override string ModKey => nameof(DarknessRandomizer);

        public override VersioningPolicy<string> VersioningPolicy => Policy.Instance;

        public override bool TryProvideSettings(out RandomizationSettings? settings)
        {
            settings = DarknessRandomizer.GS.DarknessRandomizationSettings;
            return settings.IsEnabled;
        }

        public override void ReceiveSettings(RandomizationSettings? settings) => ConnectionMenu.Instance.ApplySettings(settings ?? new());
    }
}
