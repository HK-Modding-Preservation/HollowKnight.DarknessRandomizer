namespace DarknessRandomizer.Rando
{
    public enum DarknessLevel
    {
        Dim,
        Dark,
        Cursed
    }

    public class DarknessRandomizationSettings
    {
        public bool RandomizeDarkness = false;
        public DarknessLevel DarknessLevel = DarknessLevel.Dim;

        public DarknessRandomizationSettings Clone()
        {
            return (DarknessRandomizationSettings)MemberwiseClone();
        }
    }
}
