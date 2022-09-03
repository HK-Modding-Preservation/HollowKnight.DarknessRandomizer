using DarknessRandomizer.IC;
using ItemChanger;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;

namespace DarknessRandomizer.Rando
{
    internal static class RequestModifier
    {
        public static void Setup()
        {
            RequestBuilder.OnUpdate.Subscribe(-500f, SetupRefs);
            RequestBuilder.OnUpdate.Subscribe(50.0f, RandomizeDarkness);
            RequestBuilder.OnUpdate.Subscribe(60.0f, AddItems);
        }

        private static void SetupRefs(RequestBuilder rb)
        {
            if (!RandoInterop.ShardedLantern) return;

            rb.EditItemRequest(LanternShardItem.Name, info =>
            {
                info.getItemDef = () => new()
                {
                    Name = LanternShardItem.Name,
                    Pool = PoolNames.Key,
                    MajorItem = false,
                    PriceCap = 750
                };
            });
        }

        private static void RandomizeDarkness(RequestBuilder rb)
        {
            if (!RandoInterop.IsEnabled) return;

            RandoInterop.LS = new(rb.gs, rb.ctx.StartDef);
        }

        private static void AddItems(RequestBuilder rb)
        {
            if (!RandoInterop.ShardedLantern || rb.StartItems.GetCount(ItemNames.Lumafly_Lantern) > 0)
            {
                // Nothing further to do.
                return;
            }

            rb.RemoveItemByName(ItemNames.Lumafly_Lantern);
            rb.RemoveItemByName($"{PlaceholderItem.Prefix}{ItemNames.Lumafly_Lantern}");

            if (rb.gs.PoolSettings.Keys)
            {
                int numShards = LanternShardItem.TotalNumShards;
                numShards += DarknessRandomizer.GS.DarknessRandomizationSettings.TwoDupeShards ? 2 : 0;
                numShards *= rb.gs.DuplicateItemSettings.DuplicateUniqueKeys ? 2 : 1;
                for (int i = 0; i < numShards; ++i)
                {
                    rb.AddItemByName(i < LanternShardItem.TotalNumShards ? LanternShardItem.Name : $"{PlaceholderItem.Prefix}{LanternShardItem.Name}");
                }
            }
            else
            {
                rb.RemoveFromVanilla(LocationNames.Sly, ItemNames.Lumafly_Lantern);

                for (int i = 0; i < LanternShardItem.TotalNumShards; ++i)
                {
                    VanillaDef def = new(LanternShardItem.Name, LocationNames.Sly, new CostDef[] { new("GEO", 300 + i * 100) });
                    rb.AddToVanilla(def);
                }
            }
        }
    }
}
