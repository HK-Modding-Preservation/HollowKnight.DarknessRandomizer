using ItemChanger;
using RandomizerMod.RC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Rando
{
    public static class Vanilla
    {
        public static void Setup()
        {
            RequestBuilder.OnUpdate.Subscribe(-99f, RemoveLantern);
            On.UIManager.StartNewGame += PlaceVanillaItems;
        }

        private static bool VanillaShatteredLantern => RandoInterop.ShatteredLantern && !RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.Keys;

        private static void RemoveLantern(RequestBuilder rb)
        {
            if (!VanillaShatteredLantern) return;

            rb.EditLocationRequest(LocationNames.Sly, info =>
            {
                var origFetch = info.customPlacementFetch;
                info.customPlacementFetch = (factory, placement) =>
                {
                    var origPlacement = origFetch(factory, placement);
                    if (origPlacement is ItemChanger.Placements.ShopPlacement sp)
                    {
                        sp.defaultShopItems &= ~DefaultShopItems.SlyLantern;
                        return sp;
                    }
                    return origPlacement;
                };
            });
        }

        private static void PlaceVanillaItems(On.UIManager.orig_StartNewGame orig, UIManager self, bool permaDeath, bool bossRush)
        {
            if (!VanillaShatteredLantern)
            {
                orig(self, permaDeath, bossRush);
                return;
            }

            List<AbstractPlacement> placements = new();
            var sly = Finder.GetLocation(LocationNames.Sly);
            var item = Finder.GetItem(RandoInterop.LanternShardItemName);
            for (int i = 0; i < 4; i++)
            {
                var placement = sly.Wrap().Add(item);
                placement.AddTag<CostTag>().Cost = Cost.NewGeoCost(300 + i * 100);
            }

            orig(self, permaDeath, bossRush);
        }
    }
}
