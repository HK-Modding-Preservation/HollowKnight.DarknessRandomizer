namespace DarknessRandomizer.Lib
{
    public enum Darkness : int
    {
        Bright = 0,
        SemiDark = 1,
        Dark = 2
    }

    public static class DarknessUtil
    {
        public static Darkness Min(Darkness a, Darkness b) => a < b ? a : b;

        public static Darkness Max(Darkness a, Darkness b) => a > b ? a : b;

        public static Darkness Clamp(this Darkness self, Darkness min, Darkness max) => Min(Max(self, min), max);
    }
}
