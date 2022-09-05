using ItemChanger;

namespace DarknessRandomizer.IC
{
    public class NoLanternShardItem : AbstractLanternShardItem
    {
        public const string ItemName = "Not_Lantern_Shard";

        public NoLanternShardItem() : base(ItemName, RandoPlus.Consts.NoLantern) { }

        public override AbstractItem Clone() => new NoLanternShardItem();

        public static void DefineICRefs() => Finder.DefineCustomItem(new NoLanternShardItem());
    }
}
