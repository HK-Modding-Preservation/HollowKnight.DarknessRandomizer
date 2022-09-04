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
                int count = LanternShards.GetPDShardCount();
                return count >= LanternShards.TotalNumShards ? "Lantern Shard" : $"Lantern Shard (#{count})";
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
                switch (LanternShards.GetPDShardCount())
                {
                    case 0:
                        return "I suppose this piece of trash is worth something?";
                    case 1:
                        return "What are you going to do with two pieces of trash?";
                    case 2:
                        return "Are you going to weld these together or something?  How?!";
                    default:
                        return "Pay no attention to the debug message behind the curtain.";
                }
            }
        }

        public IString Clone() => new LanternShardShopString();
    }
}
