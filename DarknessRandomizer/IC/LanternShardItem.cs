using ItemChanger;
using ItemChanger.Items;
using ItemChanger.Tags;
using ItemChanger.UIDefs;
using Modding;
using RandomizerMod.RandomizerData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.IC
{
    public class LanternShardItem : AbstractItem
    {
        public const string Name = "Lantern_Shard";

        public const int TotalNumShards = 4;

        public const string PDName = "numLanternShards";

        public static int GetPDShardCount() => PlayerData.instance.GetInt(PDName);

        LanternShardItem()
        {
            this.name = Name;
            this.UIDef = new MsgUIDef()
            {
                name = new LanternShardNameString(),
                shopDesc = new LanternShardShopString(),
                sprite = new EmbeddedSprite("LanternShard")
            };

            var interop = AddTag<InteropTag>();
            interop.Message = "RandoSupplementalData";
            interop.Properties["PoolGroup"] = "Keys";
            interop.Properties["ModSource"] = DarknessRandomizer.Instance.GetName();

            ModifyItem += MaybeCompleteLantern;
        }

        public override void GiveImmediate(GiveInfo info) => PlayerData.instance.SetInt(PDName, GetPDShardCount() + 1);

        public override bool Redundant() => PlayerData.instance.GetBool(nameof(PlayerData.instance.hasLantern));

        private void MaybeCompleteLantern(GiveEventArgs args)
        {
            if (LanternShardItem.GetPDShardCount() == LanternShardItem.TotalNumShards - 1
                || PlayerData.instance.GetBool(nameof(PlayerData.instance.hasLantern)))
            {
                args.Item = Finder.GetItem(ItemNames.Lumafly_Lantern);
            }
        }

        public static void DefineICRefs()
        {
            LanternShardItem lanternShard = new();
            Finder.DefineCustomItem(lanternShard);
        }
    }
}
