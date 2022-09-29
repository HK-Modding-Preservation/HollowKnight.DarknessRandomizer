using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using RandomizerMod.Menu;
using System;
using System.Collections.Generic;
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

        private static T Lookup<T>(MenuElementFactory<DarknessRandomizationSettings> factory, string name) where T : MenuItem => factory.ElementLookup[name] as T ?? throw new ArgumentException("Menu error");

        private void LockIfFalse(MenuItem<bool> src, List<ILockable> dest)
        {
            void onChange(bool value)
            {
                foreach (var lockable in dest)
                {
                    if (value) lockable.Unlock();
                    else lockable.Lock();
                }
                SetEnabledColor();
            }

            src.ValueChanged += onChange;
            onChange(src.Value);
        }

        private void SetEnabledColor() => entryButton.Text.color = DarknessRandomizer.GS.DarknessRandomizationSettings.IsEnabled ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR; 

        private ConnectionMenu(MenuPage landingPage)
        {
            MenuPage mainPage = new("DarknessRando Main Page", landingPage);
            entryButton = new(landingPage, Localize("Darkness Rando"));
            entryButton.AddHideAndShowEvent(mainPage);

            var settings = DarknessRandomizer.GS.DarknessRandomizationSettings;
            MenuElementFactory<DarknessRandomizationSettings> factory = new(mainPage, settings);
            var randomizeDarkness = Lookup<MenuItem<bool>>(factory, nameof(settings.RandomizeDarkness));
            var darknessLevel = Lookup<MenuItem>(factory, nameof(settings.DarknessLevel));
            var chaos = Lookup<MenuItem>(factory, nameof(settings.Chaos));
            var shatteredLantern = Lookup<MenuItem<bool>>(factory, nameof(settings.ShatteredLantern));
            var twoDupeShards = Lookup<MenuItem>(factory, nameof(settings.TwoDupeShards));

            LockIfFalse(randomizeDarkness, new() { darknessLevel, chaos });
            LockIfFalse(shatteredLantern, new() { twoDupeShards });
            SetEnabledColor();

            GridItemPanel gridItemPanel = new(mainPage, SpaceParameters.TOP_CENTER_UNDER_TITLE, 2, SpaceParameters.VSPACE_MEDIUM, SpaceParameters.HSPACE_LARGE, true);
            gridItemPanel.Insert(0, 0, randomizeDarkness);
            gridItemPanel.Insert(0, 1, shatteredLantern);
            gridItemPanel.Insert(1, 0, darknessLevel);
            gridItemPanel.Insert(1, 1, twoDupeShards);
            gridItemPanel.Insert(2, 0, chaos);
        }
    }
}
