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

        private static bool VanillaShatteredLantern(RequestBuilder? rb = null) => RandoInterop.ShatteredLantern && !(rb?.gs ?? RandomizerMod.RandomizerMod.RS.GenerationSettings).PoolSettings.Keys;
        private static void RemoveLantern(RequestBuilder rb)
        {
            if (!VanillaShatteredLantern(rb)) return;

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
            if (!VanillaShatteredLantern())
            {
                orig(self, permaDeath, bossRush);
                return;
            }

            var placement = Finder.GetLocation(LocationNames.Sly).Wrap();
            for (int i = 0; i < 4; i++)
            {
                var item = Finder.GetItem(RandoInterop.LanternShardItemName);
                item.AddTag<CostTag>().Cost = Cost.NewGeoCost(300 + i * 100);
                placement.Add(item);
            }
            List<AbstractPlacement> placements = new() { placement };
            ItemChangerMod.AddPlacements(placements);

            orig(self, permaDeath, bossRush);
        }
    }
}
