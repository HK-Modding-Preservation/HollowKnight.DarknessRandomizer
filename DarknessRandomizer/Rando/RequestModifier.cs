using RandomizerMod.RC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
