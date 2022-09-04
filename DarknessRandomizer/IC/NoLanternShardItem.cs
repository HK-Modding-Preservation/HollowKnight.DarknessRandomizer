using ItemChanger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
