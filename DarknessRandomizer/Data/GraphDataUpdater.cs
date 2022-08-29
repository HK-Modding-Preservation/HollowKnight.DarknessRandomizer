using DarknessRandomizer.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DarknessRandomizer.Data
{
    // TODO(You): Make a copy of /Resources/Data/graph_update_example.json as graph_update.json, and fill in your own params.
    public class GraphUpdate
    {
        public bool UpdateGraphData;
        public string RepoRoot;
    }

    class GraphDataUpdater
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

        public static void MaybeUpdateGraphData()
        {
            GraphUpdate gu;
            try
            {
                gu = JsonUtil.Deserialize<GraphUpdate>("DarknessRandomizer.Resources.Data.graph_update.json");
            }
            catch (Exception)
            {
                // Ignore.
                return;
            }

            if (!gu.UpdateGraphData) return;
            DarknessRandomizer.Log("Updating graph data...");

            // Load all the data.
            var SM = SceneMetadata.Instance;
            var SD = SceneData.Instance;
            var CD = ClusterData.Instance;

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
                if (sData.DifficultSkips != null && sData.DifficultSkips.Empty)
                {
                    sData.DifficultSkips = null;
                }
                if (sData.ProficientSkips != null && sData.ProficientSkips.Empty)
                {
                    sData.ProficientSkips = null;
                }
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

                clusterToScenes.GetOrCreate(sData.Cluster).Add(scene);
                sceneToCluster[scene] = sData.Cluster;
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
                            clusterAdjacency.GetOrCreate(cluster).Add(aCluster);
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
                List<string> sceneNames = new(clusterToScenes[cluster]);
                sceneNames.Sort();
                cData.SceneNames = sceneNames;
                List<string> sceneAliases = new();
                sceneNames.ForEach(n => sceneAliases.Add(SM[n].Alias));
                sceneAliases.Sort();
                cData.SceneAliases = sceneAliases;

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
                foreach (var e2 in cData.AdjacentClusters)
                {
                    var aCluster = e2.Key;
                    var aData = CD[aCluster];
                    var rd = e2.Value;
                    if (rd == RelativeDarkness.Unspecified) continue;

                    // Inspect the opposing cluster.
                    if (!aData.AdjacentClusters.TryGetValue(cluster, out RelativeDarkness ord) || ord == RelativeDarkness.Unspecified)
                    {
                        aData.AdjacentClusters[cluster] = rd.Opposite();
                    }
                    else if (ord != rd.Opposite())
                    {
                        exceptions.Add($"RelativeDarkness mismatch between {cluster} and {aCluster}");
                    }

                    // Can't set a Darker edge if the other cluster can't be dark.
                    if ((rd == RelativeDarkness.Darker && aData.MaximumDarkness(SD) < Darkness.Dark)
                        || (rd == RelativeDarkness.Brighter && cData.MaximumDarkness(SD) < Darkness.Dark))
                    {
                        exceptions.Add($"Darkness edge between {cluster} and {aCluster} cannot be satisfied");
                    }
                }

                // Don't set optional bools if they have no effect.
                bool? origOverride = cData.OverrideCannotBeDarknessSource;
                bool? origCursed = cData.CursedOnly;
                cData.OverrideCannotBeDarknessSource = null;
                cData.CursedOnly = null;
                if (!cData.CanBeDarknessSource(SD))
                {
                    origOverride = null;
                }
                else
                {
                    origOverride ??= false;
                }
                if (cData.MaximumDarkness(SD) < Darkness.Dark)
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
                if (cData.MaximumDarkness(SD) < Darkness.Dark)
                {
                    cData.DarkSettings = null;
                }
                else
                {
                    cData.DarkSettings ??= new();
                }
                if (cData.MaximumDarkness(SD) < Darkness.SemiDark || cData.MinimumDarkness(SD) > Darkness.SemiDark)
                {
                    cData.SemiDarkSettings = null;
                }
                else
                {
                    cData.SemiDarkSettings ??= new();
                }
            }
            MaybeThrowException(exceptions);

            // Only update data at the end, if we have no exceptions.
            JsonUtil.Serialize(SM, $"{gu.RepoRoot}/DarknessRandomizer/Resources/Data/scene_metadata.json");
            JsonUtil.Serialize(SD, $"{gu.RepoRoot}/DarknessRandomizer/Resources/Data/scene_data.json");
            JsonUtil.Serialize(CD, $"{gu.RepoRoot}/DarknessRandomizer/Resources/Data/cluster_data.json");

            UpdateCSFile($"{gu.RepoRoot}/DarknessRandomizer/Data/SceneName.cs", "INSERT_SCENE_NAMES", SM,
                (n, sm) => $"SceneName {CSharpClean(sm.Alias)} = new(\"{n}\")");
            UpdateCSFile($"{gu.RepoRoot}/DarknessRandomizer/Data/ClusterName.cs", "INSERT_CLUSTER_NAMES", CD,
                (n, cd) => $"ClusterName {CSharpClean(n)} = new(\"{n}\")");
        }

        private static void MaybeThrowException(List<string> exceptions)
        {
            exceptions.ForEach(DarknessRandomizer.Log);
            if (exceptions.Count > 0)
            {
                throw new ArgumentException($"{exceptions.Count} data errors encountered");
            }
        }

        private static string CSharpClean(string name)
        {
            return name.Replace("_", "").Replace("'", "").Replace("-", "");
        }

        private delegate string CSAssignment<K, V>(K key, V value);

        private static void UpdateCSFile<K, V>(string path, string marker, IDictionary<K, V> dict, CSAssignment<K, V> assigner)
        {
            StreamReader sr = new(path);
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
                    Match match = Regex.Match(line, $"^(.+)// @@@ {marker} START @@@$");
                    if (match.Success)
                    {
                        state = 1;
                        indent = match.Groups[1].Value;
                        foreach (var e in dict)
                        {
                            outLines.Add($"{indent}public static readonly {assigner.Invoke(e.Key, e.Value)};");
                        }
                    }
                }
                else if (state == 1)
                {
                    Match match = Regex.Match(line, $"^.+// @@@ {marker} END @@@$");
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

            StreamWriter sw = new(path);
            outLines.ForEach(l => sw.WriteLine(l));
            sw.Close();
        }
    }
}
