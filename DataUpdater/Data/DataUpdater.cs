using DarknessRandomizer.Lib;
using DarknessRandomizer.Rando;
using RandomizerCore.Logic;
using RandomizerMod.RC;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DarknessRandomizer.Data
{
    public class DataStats
    {
        public int NumClusters;
        public int NumSemiDarkOnlyClusters;
        public int NumCursedOnlyClusters;
        public int NumClusterAdjacencies;
        public int NumScenes;
        public int NumSemiDarkOnlyScenes;
        public int NumCursedOnlyScenes;
        public int NumSceneAdjacencies;
        public int NumDarknessSources;
        public int TotalSourceBudget;
        public int TotalCursedSourceBudget;
        public int TotalDarknessBudget;
        public int TotalCursedDarknessBudget;
    }

    class DataUpdater
    {
        private static void SyncDicts<K,V1,V2>(IDictionary<K, V1> src, IDictionary<K, V2> dst, Func<K, V2> creator)
        {
            // Add missing values.
            foreach (var key in src.Keys)
            {
                dst.AddIfEmpty(key, () => creator.Invoke(key));
            }

            // Remove any broken values.
            List<K> toRemove = new();
            foreach (var key in dst.Keys)
            {
                if (!src.ContainsKey(key))
                {
                    toRemove.Add(key);
                }
            }
            toRemove.ForEach(k => dst.Remove(k));
        }

        private static string InferGitRoot(string path)
        {
            var info = Directory.GetParent(path);
            while (info != null)
            {
                if (Directory.Exists(Path.Combine(info.FullName, ".git")))
                {
                    return info.FullName;
                }
                info = Directory.GetParent(info.FullName);
            }

            return path;
        }

        public static void UpdateGraphData()
        {
            string root = InferGitRoot(System.IO.Directory.GetCurrentDirectory());
            Console.WriteLine("Updating graph data");

            // Load all the data.
            var SM = RawSceneMetadata.LoadFromPath($"{root}/DarknessRandomizer/Resources/Data/scene_metadata.json");
            var SD = RawSceneData.LoadFromPath($"{root}/DarknessRandomizer/Resources/Data/scene_data.json");
            var CD = RawClusterData.LoadFromPath($"{root}/DarknessRandomizer/Resources/Data/cluster_data.json");

            // We delete scenes from metadata to not track them; make sure adjacencies are also updated.
            foreach (var e in SM)
            {
                var scene = e.Key;
                var sData = e.Value;
                sData.AdjacentScenes.RemoveWhere(s => !SM.ContainsKey(s));

                // Ensure adjacencies are reflected.
                foreach (var aScene in sData.AdjacentScenes)
                {
                    SM[aScene].AdjacentScenes.Add(scene);
                }
            }

            // Sync SceneData to SceneMetadata.
            SyncDicts(SM, SD, k => new() { Alias = SM[k].Alias });

            // Validate SceneData
            List<string> exceptions = new();
            foreach (var e in SD)
            {
                var scene = e.Key;
                var sData = e.Value;
                if (sData.MaximumDarkness < sData.MinimumDarkness)
                {
                    exceptions.Add($"Invalid darkness settings for scene {scene}");
                }
                if (sData.Cluster == null || sData.Alias == null)
                {
                    exceptions.Add($"Scene {scene} is missing fields");
                }

                sData.DisplayDarknessOverrides = CleanDDO(sData);
            }
            MaybeThrowException(exceptions);

            // Build up metadata maps.
            Dictionary<string, HashSet<string>> clusterToScenes = new();
            Dictionary<string, string> sceneToCluster = new();
            foreach (var e in SD)
            {
                var scene = e.Key;
                var sData = e.Value;
                if (sData.Cluster == "UNASSIGNED") continue;

                sceneToCluster[scene] = sData.Cluster;
                clusterToScenes.GetOrAddNew(sData.Cluster).Add(scene);
            }

            // Determine cluster adjacency.
            Dictionary<string, HashSet<string>> clusterAdjacency = new();
            foreach (var e in clusterToScenes)
            {
                var cluster = e.Key;
                var scenes = e.Value;

                foreach (var scene in scenes)
                {
                    foreach (var aScene in SM[scene].AdjacentScenes)
                    {
                        if (sceneToCluster.TryGetValue(aScene, out string aCluster) && aCluster != cluster)
                        { 
                            clusterAdjacency.GetOrAddNew(cluster).Add(aCluster);
                        }
                    }
                }
            }

            SyncDicts(clusterToScenes, CD, k => new());

            // Update and validate each cluster.
            foreach (var e in CD)
            {
                var cluster = e.Key;
                var cData = e.Value;

                // Update scene names.
                cData.SceneNames = new();
                foreach (var scene in clusterToScenes[cluster])
                {
                    cData.SceneNames[scene] = SM[scene].Alias;
                }
            }

            foreach (var e in CD)
            {
                var cluster = e.Key;
                var cData = e.Value;

                if (!clusterAdjacency.TryGetValue(cluster, out HashSet<string> aClusters))
                {
                    aClusters = new();
                }

                // Sync adjacencies.
                foreach (var aCluster in aClusters)
                {
                    cData.AdjacentClusters.AddIfEmpty(aCluster, () => RelativeDarkness.Unspecified);
                }

                // Remove obsolete adjacencies.
                List<string> toRemove = new();
                foreach (var aCluster in cData.AdjacentClusters.Keys)
                {
                    if (!aClusters.Contains(aCluster))
                    {
                        toRemove.Add(aCluster);
                    }
                }
                toRemove.ForEach(c => cData.AdjacentClusters.Remove(c));

                // Validate the remaining adjacencies.
                List<Action> deferred = new();
                foreach (var e2 in cData.AdjacentClusters)
                {
                    var aCluster = e2.Key;
                    var aData = CD[aCluster];
                    var rd = e2.Value;
                    if (rd == RelativeDarkness.Disconnected
                        || (aData.AdjacentClusters.TryGetValue(cluster, out RelativeDarkness ard) && ard == RelativeDarkness.Disconnected))
                    {
                        deferred.Add(() => cData.AdjacentClusters[aCluster] = RelativeDarkness.Disconnected);
                        aData.AdjacentClusters[cluster] = RelativeDarkness.Disconnected;
                        continue;
                    }

                    // If a certain relationship is forced, apply that.
                    bool canBeDark = cData.MaximumDarkness(s => SD[s]) == Darkness.Dark;
                    bool aCanBeDark = aData.MaximumDarkness(s => SD[s]) == Darkness.Dark;
                    if (!canBeDark && !aCanBeDark)
                    {
                        deferred.Add(() => cData.AdjacentClusters[aCluster] = RelativeDarkness.Any);
                        aData.AdjacentClusters[cluster] = RelativeDarkness.Any;
                        continue;
                    }
                    else if (canBeDark && !aCanBeDark)
                    {
                        deferred.Add(() => cData.AdjacentClusters[aCluster] = RelativeDarkness.Brighter);
                        aData.AdjacentClusters[cluster] = RelativeDarkness.Darker;
                        continue;
                    }
                    else if (!canBeDark && aCanBeDark)
                    {
                        deferred.Add(() => cData.AdjacentClusters[aCluster] = RelativeDarkness.Darker);
                        aData.AdjacentClusters[cluster] = RelativeDarkness.Brighter;
                        continue;
                    }

                    // If we're unspecified, let the other side force our hand.
                    if (rd == RelativeDarkness.Unspecified) continue;

                    // Inspect and validate te opposing cluster.
                    if (!aData.AdjacentClusters.TryGetValue(cluster, out ard) || ard == RelativeDarkness.Unspecified)
                    {
                        aData.AdjacentClusters[cluster] = rd.Opposite();
                    }
                    else if (ard != rd.Opposite())
                    {
                        exceptions.Add($"RelativeDarkness mismatch between {cluster} and {aCluster}");
                    }
                }
                deferred.ForEach(a => a.Invoke());

                // Don't set optional bools if they have no effect.
                bool? origOverride = cData.OverrideCannotBeDarknessSource;
                bool? origCursed = cData.CursedOnly;
                cData.OverrideCannotBeDarknessSource = null;
                cData.CursedOnly = null;
                if (!cData.CanBeDarknessSource(s => SD[s]))
                {
                    origOverride = null;
                }
                else
                {
                    origOverride ??= false;
                }
                if (cData.MaximumDarkness(s => SD[s]) < Darkness.Dark)
                {
                    origCursed = null;
                }
                else
                {
                    origCursed ??= false;
                }
                cData.OverrideCannotBeDarknessSource = origOverride;
                cData.CursedOnly = origCursed;

                // Handle darkness settings.
                if (cData.MaximumDarkness(s => SD[s]) < Darkness.Dark)
                {
                    cData.DarkSettings = null;
                }
                else
                {
                    cData.DarkSettings ??= new();
                }
            }
            MaybeThrowException(exceptions);

            var DS = ComputeDataStats(SM, SD, CD);
            RewriteJsonFile(DS, $"{root}/DarknessRandomizer/Resources/Data/data_stats.json");

            // Only update data at the end, if we have no exceptions.
            RewriteJsonFile(SM, $"{root}/DarknessRandomizer/Resources/Data/scene_metadata.json");
            RewriteJsonFile(SD, $"{root}/DarknessRandomizer/Resources/Data/scene_data.json");
            RewriteJsonFile(CD, $"{root}/DarknessRandomizer/Resources/Data/cluster_data.json");

            UpdateCSFile($"{root}/DarknessRandomizer/Data/SceneName.cs", "INSERT_SCENE_NAMES", SM,
                (n, sm) => $"public static readonly SceneName {CSharpClean(sm.Alias)} = new(\"{n}\")");
            UpdateCSFile($"{root}/DarknessRandomizer/Data/ClusterName.cs", "INSERT_CLUSTER_NAMES", CD,
                (n, cd) => $"public static readonly ClusterName {CSharpClean(n)} = new(\"{n}\")");
            UpdateCSFile($"{root}/DarknessRandomizer/Data/WaypointNames.cs", "INSERT_WAYPOINTS", GetWaypointsDict(),
                (k, v) => $"public const string {k} = \"{v}\"");
            Console.WriteLine("Update Graph Data!");
        }

        private static bool DDOHasDiff(DisplayDarknessOverrides ddo, Darkness d)
        {
            // FIXME: Denormalize regions
            return ddo.SceneDarkness != d || (ddo.RegionDarkness != d && ddo.DarknessRegions.Count > 0);
        }

        private static DisplayDarknessOverrides CleanDDO(RawSceneData sceneData)
        {
            var ddo = sceneData.DisplayDarknessOverrides;
            if (ddo == null) return null;

            ddo.Conditions.RemoveWhere(d => d < sceneData.MinimumDarkness || d > sceneData.MaximumDarkness);
            ddo.Conditions.RemoveWhere(d => !DDOHasDiff(ddo, d));
            if (ddo.Conditions.Count == 0) return null;

            ddo.DarknessRegions.Sort((a, b) => Math.Sign((a.X != b.X) ? a.X - b.X : a.Y - b.Y));
            return ddo;
        }

        private static void MaybeThrowException(List<string> exceptions)
        {
            exceptions.ForEach(Console.WriteLine);
            if (exceptions.Count > 0)
            {
                throw new ArgumentException($"{exceptions.Count} data errors encountered");
            }
        }

        private static DataStats ComputeDataStats(SortedDictionary<string, RawSceneMetadata> SM,
            SortedDictionary<string, RawSceneData> SD, SortedDictionary<string, RawClusterData> CD)
        {
            DataStats DS = new();
            foreach (var e in CD)
            {
                var cluster = e.Key;
                var cData = e.Value;

                bool darkSource = cData.CanBeDarknessSource(s => SD[s]);
                bool semiDark = cData.MaximumDarkness(s => SD[s]) == Darkness.SemiDark;
                bool cursed = cData.CursedOnly ?? false;
                int darkCost = cData.CostWeight ?? 0;

                ++DS.NumClusters;
                DS.NumDarknessSources += darkSource ? 1 : 0;
                DS.NumClusterAdjacencies += cData.AdjacentClusters.Count;
                DS.NumCursedOnlyClusters += cursed ? 1 : 0;
                DS.NumSemiDarkOnlyClusters += semiDark ? 1 : 0;
                DS.TotalDarknessBudget += cursed ? 0 : darkCost;
                DS.TotalCursedDarknessBudget += darkCost;
                DS.TotalSourceBudget += darkSource ? (cursed ? 0 : darkCost) : 0;
                DS.TotalCursedSourceBudget += darkSource ? darkCost : 0;
            }
            foreach (var e in SD)
            {
                var scene = e.Key;
                var sData = e.Value;
                var cData = CD[sData.Cluster];

                bool semiDark = sData.MaximumDarkness == Darkness.SemiDark;

                ++DS.NumScenes;
                DS.NumSceneAdjacencies += SM[scene].AdjacentScenes.Count;
                DS.NumSemiDarkOnlyScenes += semiDark ? 1 : 0;
                DS.NumCursedOnlyScenes += cData.CursedOnly ?? false ? 1 : 0;
            }
            DS.NumSceneAdjacencies /= 2;
            DS.NumClusterAdjacencies /= 2;
            return DS;
        }

        private static SortedDictionary<string, string> GetWaypointsDict()
        {
            SortedDictionary<string, string> dict = new();
            LogicManager lm = RCData.GetNewLogicManager(new());
            foreach (var lw in lm.Waypoints)
            {
                dict[CSharpClean(lw.term.Name)] = lw.term.Name;
            }
            return dict;
        }

        private static void RewriteJsonFile<T>(T data, string path)
        {
            File.Delete(path);
            JsonUtil.Serialize(data, path);
        }

        private static string CSharpClean(string name)
        {
            return name.Replace("_", "").Replace("'", "").Replace("-", "");
        }

        private delegate string CSAssignment<K, V>(K key, V value);

        private static void UpdateCSFile<K, V>(string path, string marker, IDictionary<K, V> dict, CSAssignment<K, V> assigner)
        {
            using StreamReader sr = new(path);
            List<string> outLines = new();
            string line, indent;
            int state = 0;
            while (true)
            {
                line = sr.ReadLine();
                if (line == null) break;

                if (state == 0)
                {
                    outLines.Add(line);
                    Match match = Regex.Match(line, $"^(.+)// @@@ {Regex.Escape(marker)} START @@@$");
                    if (match.Success)
                    {
                        state = 1;
                        indent = match.Groups[1].Value;
                        foreach (var e in dict)
                        {
                            outLines.Add($"{indent}{assigner.Invoke(e.Key, e.Value)};");
                        }
                    }
                }
                else if (state == 1)
                {
                    Match match = Regex.Match(line, $"^.+// @@@ {Regex.Escape(marker)} END @@@$");
                    if (match.Success)
                    {
                        state = 2;
                        outLines.Add(line);
                    }
                }
                else
                {
                    outLines.Add(line);
                }
            }

            sr.Close();
            File.Delete(path);

            using StreamWriter sw = new(path);
            outLines.ForEach(l => sw.WriteLine(l));
        }
    }
}
