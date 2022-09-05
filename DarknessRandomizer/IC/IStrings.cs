using ItemChanger;

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
                        return "I hear that with two lanterns you can explore advanced darkness.";
                }
            }
        }

        public IString Clone() => new LanternShardShopString();
    }
}
