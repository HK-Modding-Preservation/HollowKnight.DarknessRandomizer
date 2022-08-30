using DarknessRandomizer.Data;
using DarknessRandomizer.Lib;
using DarknessRandomizer.Rando;
using ItemChanger.Internal.Menu;
using Modding;
using RandomizerMod;
using System;

namespace DarknessRandomizer
{
    public class DarknessRandomizer : Mod, IGlobalSettings<GlobalSettings>, ICustomMenuMod
    {
        public static DarknessRandomizer Instance { get; private set; }
        public static GlobalSettings GS { get; private set; } = new();

        public bool ToggleButtonInsideMenu => throw new NotImplementedException();

        public static new void Log(string msg) { ((Loggable)Instance).Log(msg); }

        public DarknessRandomizer() : base("DarknessRandomizer")
        {
            Instance = this;
        }

        public override void Initialize()
        {
            if (ModHooks.GetMod("Randomizer 4") is Mod)
            {
                RandoInterop.Setup();
            }

            SceneMetadata.Load();
            Data.SceneData.Load();
            ClusterData.Load();
        }

        public void OnLoadGlobal(GlobalSettings s) => GS = s ?? new();

        public GlobalSettings OnSaveGlobal() => GS ?? new();

        public override string GetVersion() => Version.Instance;

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {
            ModMenuScreenBuilder builder = new(Localization.Localize("Darkness Randomizer Viewer"), modListMenu);
            builder.AddButton(Localization.Localize("(DEV) Run Data Updater"), null, DataUpdater.UpdateGraphData);
            return builder.CreateMenuScreen();
        }
    }
}