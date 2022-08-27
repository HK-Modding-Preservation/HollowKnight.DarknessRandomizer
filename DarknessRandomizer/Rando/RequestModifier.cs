using RandomizerMod.RC;

namespace DarknessRandomizer.Rando
{
    internal static class RequestModifier
    {
        public static void Setup()
        {
            RequestBuilder.OnUpdate.Subscribe(90.0f, rb => RandoInterop.LS = new(rb.gs.Seed, rb.ctx.StartDef));
        }
    }
}
