using ItemChanger;
using ItemChanger.Tags;
using ItemChanger.UIDefs;

namespace DarknessRandomizer.IC
{
    public static class LanternShards
    {
        public const string PDName = "numLanternShards";

        public const int TotalNumShards = 4;

        public static int GetPDShardCount() => PlayerData.instance.GetInt(PDName);
    }

    public class AbstractLanternShardItem : AbstractItem
    {
        public string FinalItemName;

        protected AbstractLanternShardItem(string name, string finalName)
        {
            this.name = name;
            this.FinalItemName = finalName;
            this.UIDef = new MsgUIDef()
            {
                name = new LanternShardNameString(),
                shopDesc = new LanternShardShopString(),
                sprite = new EmbeddedSprite("LanternShard")
            };

            var interop = AddTag<InteropTag>();
            interop.Message = ConnectionMetadataInjector.SupplementalMetadata.InteropTagMessage;
            interop.Properties[ConnectionMetadataInjector.InjectedProps.ItemPoolGroup.Name] = ConnectionMetadataInjector.Util.PoolGroup.Keys.ToString();
            interop.Properties[ConnectionMetadataInjector.InjectedProps.ModSource.Name] = DarknessRandomizer.Instance.GetName();

            ModifyItem += MaybeCompleteLantern;
        }

        public override void GiveImmediate(GiveInfo info) => PlayerData.instance.SetInt(LanternShards.PDName, LanternShards.GetPDShardCount() + 1);

        public override bool Redundant() => PlayerData.instance.GetBool(nameof(PlayerData.instance.hasLantern));

        private void MaybeCompleteLantern(GiveEventArgs args)
        {
            if (LanternShards.GetPDShardCount() == LanternShards.TotalNumShards - 1
                || PlayerData.instance.GetBool(nameof(PlayerData.instance.hasLantern)))
            {
                args.Item = Finder.GetItem(FinalItemName);
            }
        }
    }

    public class LanternShardItem : AbstractLanternShardItem
    {
        public const string ItemName = "Lantern_Shard";

        public LanternShardItem() : base(ItemName, ItemNames.Lumafly_Lantern) { }

        public override AbstractItem Clone() => new LanternShardItem();

        public static void DefineICRefs() => Finder.DefineCustomItem(new LanternShardItem());
    }
}
