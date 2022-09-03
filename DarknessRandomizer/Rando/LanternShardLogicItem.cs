using DarknessRandomizer.IC;
using RandomizerCore;
using RandomizerCore.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Rando
{
    public record LanternShardLogicItem(Term ShardTerm, Term LanternTerm) : LogicItem(LanternShardItem.Name)
    {
        public override void AddTo(ProgressionManager pm)
        {
            var shards = pm.Get(ShardTerm);
            if (shards < LanternShardItem.TotalNumShards)
            {
                pm.Set(ShardTerm, shards + 1);
                if (shards == LanternShardItem.TotalNumShards - 1)
                {
                    pm.Set(LanternTerm, 1);
                }
            }
        }

        public override IEnumerable<Term> GetAffectedTerms()
        {
            yield return ShardTerm;
            yield return LanternTerm;
        }
    }
}
