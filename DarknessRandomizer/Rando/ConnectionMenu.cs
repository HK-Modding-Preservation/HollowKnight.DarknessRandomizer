using ItemChanger;
using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using RandomizerMod.Menu;
using System;
using static RandomizerMod.Localization;

namespace DarknessRandomizer.Rando
{
    internal class ConnectionMenu
    {
        public static ConnectionMenu Instance { get; private set; }

        public static void Setup()
        {
            RandomizerMenuAPI.AddMenuPage(OnRandomizerMenuConstruction, TryGetMenuButton);
            MenuChangerMod.OnExitMainMenu += () => Instance = null;
        }

        public static void OnRandomizerMenuConstruction(MenuPage page) => Instance = new(page);

        public static bool TryGetMenuButton(MenuPage page, out SmallButton button)
        {
            button = Instance.entryButton;
            return true;
        }

        public SmallButton entryButton;
        public MenuPage mainPage;
        public GridItemPanel gridPanel;

        private static T Lookup<T>(MenuElementFactory<DarknessRandomizationSettings> factory, string name) where T : MenuItem => factory.ElementLookup[name] as T ?? throw new ArgumentException("Menu error");

        private static void LockIfFalse(MenuItem<bool> src, ILockable dest)
        {
            void onChange(bool value)
            {
                if (value) dest.Unlock();
                else dest.Lock();
            }

            src.ValueChanged += onChange;
            onChange(src.Value);
        }

        private ConnectionMenu(MenuPage landingPage)
        {
            mainPage = new("DarknessRando Main Page", landingPage);
            entryButton = new(landingPage, Localize("Darkness Rando"));
            entryButton.AddHideAndShowEvent(mainPage);

            var settings = DarknessRandomizer.GS.DarknessRandomizationSettings;
            MenuElementFactory<DarknessRandomizationSettings> factory = new(mainPage, settings);
            var randomizeDarkness = Lookup<MenuItem<bool>>(factory, nameof(settings.RandomizeDarkness));
            var darknessLevel = Lookup<MenuItem>(factory, nameof(settings.DarknessLevel));
            var shatteredLantern = Lookup<MenuItem<bool>>(factory, nameof(settings.ShatteredLantern));
            var twoDupeShards = Lookup<MenuItem>(factory, nameof(settings.TwoDupeShards));

            LockIfFalse(randomizeDarkness, darknessLevel);
            LockIfFalse(shatteredLantern, twoDupeShards);

            gridPanel = new(mainPage, SpaceParameters.TOP_CENTER_UNDER_TITLE, 2, SpaceParameters.VSPACE_MEDIUM, SpaceParameters.HSPACE_LARGE, true);
            gridPanel.Insert(0, 0, randomizeDarkness);
            gridPanel.Insert(0, 1, shatteredLantern);
            gridPanel.Insert(1, 0, darknessLevel);
            gridPanel.Insert(1, 1, twoDupeShards);
        }
    }
}
