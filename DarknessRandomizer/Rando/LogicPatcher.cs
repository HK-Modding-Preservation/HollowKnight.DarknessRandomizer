using RandomizerCore.Logic;
using RandomizerMod.RC;
using RandomizerMod.Settings;

namespace DarknessRandomizer.Rando
{
    internal static class LogicPatcher
    {
        public static void Setup()
        {
            // FIXME: Set a proper weight here.
            RCData.RuntimeLogicOverride.Subscribe(1.5f, ModifyLMB);
        }

        public static void ModifyLMB(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!RandoInterop.IsEnabled()) return;

            RandoInterop.Initialize(gs.Seed);

            // FIXME: Modify Logic
        }
    }
}
