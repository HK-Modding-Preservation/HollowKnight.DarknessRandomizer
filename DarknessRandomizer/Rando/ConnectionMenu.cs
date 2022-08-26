using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using RandomizerMod.Menu;
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
        public MenuElementFactory<DarknessRandomizationSettings> elementFactory;
        public VerticalItemPanel verticalPanel;

        private ConnectionMenu(MenuPage landingPage)
        {
            mainPage = new("DarknessRando Main Page", landingPage);
            entryButton = new(landingPage, Localize("Darkness Rando"));
            entryButton.AddHideAndShowEvent(mainPage);

            elementFactory = new(mainPage, DarknessRandomizer.GS.DarknessRandomizationSettings);
            Localize(elementFactory);
            verticalPanel = new(mainPage, SpaceParameters.TOP_CENTER_UNDER_TITLE, SpaceParameters.VSPACE_MEDIUM, true, elementFactory.Elements);
        }
    }
}
