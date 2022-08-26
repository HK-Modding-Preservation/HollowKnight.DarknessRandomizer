using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemChanger;
using RandomizerMod.RC;

namespace DarknessRandomizer.Rando
{
    public static class RandoInterop
    {
        public static LocalSettings LS { get; private set; }

        public static void Setup()
        {
            ConnectionMenu.Setup();
            LogicPatcher.Setup();

            RandoController.OnExportCompleted += Finish;
        }

        public static void Clear()
        {
            LS = null;
        }

        public static bool IsEnabled()
        {
            return DarknessRandomizer.GS.DarknessRandomizationSettings.DarknessLevel != DarknessLevel.Vanilla;
        }

        public static void Initialize(int seed)
        {
            Clear();
            LS = new(seed);
        }

        public static void Finish(RandoController rc)
        {
            if (!IsEnabled()) return;

            // TODO: Define a module for FSM edits.

            var dlem = ItemChangerMod.Modules.GetOrAdd<ItemChanger.Modules.DarknessLevelEditModule>();
            foreach (var entry in LS.DarknessOverrides) {
                dlem.darknessLevelsByScene[entry.Key] = (int)entry.Value;
            }
        }
    }
}
