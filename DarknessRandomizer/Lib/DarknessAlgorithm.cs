using DarknessRandomizer.Data;
using DarknessRandomizer.Rando;
using RandomizerMod.RandomizerData;
using RandomizerMod.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Lib
{
    public class AlgorithmStats
    {
        public int DarknessSpent;
        public int DarknessRemaining;
        public ICustomDarknessAlgorithmStats? CustomStats = null;
    }

    public interface ICustomDarknessAlgorithmStats { }

    public abstract class DarknessAlgorithm
    {
        public static DarknessAlgorithm Select(GenerationSettings GS, StartDef start, DarknessRandomizationSettings DRS)
        {
            if (DRS.Chaos)
            {
                return new ChaosDarknessAlgoritm(GS, start, DRS);
            }
            else
            {
                return new DefaultDarknessAlgorithm(GS, start, DRS);
            }
        }

        protected readonly GenerationSettings GS;
        protected readonly StartDef start;
        protected readonly DarknessRandomizationSettings DRS;

        protected readonly Random r;
        protected int darknessAvailable;
        protected readonly HashSet<ClusterName> forcedBrightClusters;

        protected DarknessAlgorithm(GenerationSettings GS, StartDef start, DarknessRandomizationSettings DRS)
        {
            this.GS = GS;
            this.start = start;
            this.DRS = DRS;
            this.r = new(GS.Seed + 7);
            this.darknessAvailable = DRS.GetDarknessBudget(r);

            this.forcedBrightClusters = new();

            foreach (var c in Starts.GetStartClusters(start.Name))
            {
                this.forcedBrightClusters.Add(c);
            }

            // Always include the local cluster, even in TRANDO.
            if (SceneName.TryGetValue(start.SceneName, out SceneName sceneName))
            {
                this.forcedBrightClusters.Add(Data.SceneData.Get(sceneName).Cluster);
            }

            if (!GS.PoolSettings.Keys)
            {
                // If lantern isn't randomized, we need to ensure the vanilla lantern location (Sly) is accessible.
                // This won't work if they also didn't randomize stags but I'm not trying to fix that.
                this.forcedBrightClusters.Add(ClusterName.CrossroadsStag);
                this.forcedBrightClusters.Add(ClusterName.CrossroadsStagHub);
                this.forcedBrightClusters.Add(ClusterName.CrossroadsShops);
            }

            if (GS.CursedSettings.Deranged)
            {
                // Disallow vanilla dark rooms.
                this.forcedBrightClusters.Add(ClusterName.CliffsJonis);
                this.forcedBrightClusters.Add(ClusterName.CrystalPeaksDarkRoom);
                this.forcedBrightClusters.Add(ClusterName.CrystalPeaksToll);
                this.forcedBrightClusters.Add(ClusterName.DeepnestDark);
                this.forcedBrightClusters.Add(ClusterName.GreenpathStoneSanctuary);
            }

            // If white palace rando is disabled, add all white palace rooms to the forbidden set.
            if (GS.LongLocationSettings.WhitePalaceRando != LongLocationSettings.WPSetting.Allowed)
            {
                foreach (var cluster in ClusterName.All())
                {
                    var cData = ClusterData.Get(cluster);
                    if (cData.IsInPathOfPain || (GS.LongLocationSettings.WhitePalaceRando == LongLocationSettings.WPSetting.ExcludeWhitePalace && cData.IsInWhitePalace))
                    {
                        this.forcedBrightClusters.Add(cluster);
                    }
                }
            }
        }

        public abstract void SpreadDarkness(out SceneDarknessDict sceneDarkness, out AlgorithmStats stats);
    }
}
