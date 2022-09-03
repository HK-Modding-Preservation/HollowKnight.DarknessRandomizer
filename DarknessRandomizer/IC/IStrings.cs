using ItemChanger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.IC
{

    public class LanternShardNameString : IString
    {
        public string Value
        {
            get
            {
                int count = LanternShardItem.GetPDShardCount();
                return count >= LanternShardItem.TotalNumShards ? "Lantern Shard" : $"Lantern Shard (#{count})";
            }
        }

        public IString Clone() => new LanternShardNameString();
    }

    public class LanternShardShopString : IString
    {
        public string Value
        {
            get
            {
                switch (LanternShardItem.GetPDShardCount())
                {
                    case 1:
                        return "I suppose this piece of trash is worth something?";
                    case 2:
                        return "What are you going to do with two pieces of trash?";
                    case 3:
                        return "Are you going to weld these together or something?  How?!";
                    default:
                        return "Shouldn't get here";
                }
            }
        }

        public IString Clone() => new LanternShardShopString();
    }
}
