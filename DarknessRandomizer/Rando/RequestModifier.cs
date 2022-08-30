using RandomizerMod.RC;

namespace DarknessRandomizer.Rando
{
    internal static class RequestModifier
    {
        public static void Setup()
        {
            RequestBuilder.OnUpdate.Subscribe(90.0f, InitLocalSettings);
        }

        private static void InitLocalSettings(RequestBuilder rb)
        {
            if (!RandoInterop.IsEnabled()) return;
            RandoInterop.LS = new(rb.gs, rb.ctx.StartDef);
        }
    }
}
