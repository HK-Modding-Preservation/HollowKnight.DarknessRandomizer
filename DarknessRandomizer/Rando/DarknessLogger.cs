using RandomizerMod.Logging;
using System.IO;

namespace DarknessRandomizer.Rando
{
    public class DarknessLogger : RandoLogger
    {
        public override void Log(LogArguments args)
        {
            LogManager.Write(DoLog, "DarknessSpoiler.json");
        }

        public void DoLog(TextWriter tw)
        {
            JsonUtil.Serialize(RandoInterop.LS, tw);
        }
    }
}
