using DarknessRandomizer.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Data
{
    public static class GraphData
    {
        public static Graph LoadGraph()
        {
            Graph g = new();

            g.Clusters[Clusters.Unn] = new() {
                Scenes = new() {
                    { Scenes.GreenpathLakeOfUnn, new() },
                    { Scenes.GreenpathUnn, new() },
                    { Scenes.GreenpathUnnBench, new() { MaximumDarkness = Darkness.SemiDark} } },
                CostWeight = 50 };

            g.Init();
            return g;
        }
    }
}
