using DarknessRandomizer.Data;
using DarknessRandomizer.Lib;
using RandomizerCore.Logic;
using RandomizerCore.StringLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Rando
{
    using LogicTree = BooleanTree<ParsedToken, LogicTreeMetadata>;
    using SceneTree = BooleanTree<ParsedToken, SceneTreeMetadata>;

    internal class LogicTreeMetadata
    {
        public LogicTree Parent;
        public SceneTree SceneTree;
    }

    internal class SceneTreeMetadata
    {
        public bool GuaranteedLantern = false;
    }

    public static class LanternSubstitution
    {
        public static void Apply(LogicManagerBuilder lmb, string name)
        {
            var lc = lmb.LogicLookup[name];

            // Parse to tree of +|
            Stack<LogicTree> stack = new();
            for (int i = 0; i < lc.Count; i++)
            {
                var lt = lc[i];
                if (lt is OperatorToken ot)
                {
                    stack.Push(LogicTree.CreateTree(ot.OperatorType, new List<LogicTree> { stack.Pop(), stack.Pop() }));
                }
                else
                {
                    stack.Push(LogicTree.CreateLeaf(ParsedToken.Parse(lt)));
                }
            }

            // Get the top node.
            var ln = stack.Pop();
            if (stack.Count != 0)
            {
                throw new ArgumentException($"Bad logic expression: {lc.ToInfix()}");
            }

            CalculateParentsAndSceneTrees(ln, null);

            // At this point we can create a new LogicClause.
            List<LogicToken> tokens = new();
            ListNewTokens(ln, null, tokens);
            LogicClauseBuilder lcb = new(tokens);
            lmb.LogicLookup[name] = new(lcb);
        }

        private static void CalculateParentsAndSceneTrees(LogicTree tree, LogicTree parent)
        {
            tree.Metadata.Parent = parent;
            if (tree.IsTree)
            {
                foreach (var t in tree.Children)
                {
                    CalculateParentsAndSceneTrees(t, tree);
                }

                // Child SceneTrees are now calculated, so we can calculate ours.
                tree.Metadata.SceneTree = Combine(tree.Op, tree.Children.Select(t => t.Metadata.SceneTree));
            }
            else
            {
                tree.Metadata.SceneTree = SceneTree.CreateLeaf(tree.Value);
            }
        }

        private static SceneTree LanternSceneTree()
        {
            var tree = SceneTree.CreateLeaf(ParsedToken.Lantern);
            tree.Metadata.GuaranteedLantern = true;
            return tree;
        }

        private static SceneTree Combine(OperatorType op, IEnumerable<SceneTree> trees)
        {
            var treeList = trees.Where(t => t != null && (t.IsTree || t.Value.IsRelevant)).ToList();
            if (treeList.Count == 0)
            {
                return null;
            }

            bool guaranteedLantern;
            if (op == OperatorType.AND)
            {
                if (treeList.All(t => t.IsLeaf && t.Value.IsLantern))
                {
                    return LanternSceneTree();
                }
                guaranteedLantern = treeList.All(t => t.Metadata.GuaranteedLantern);
            }
            else
            {
                if (treeList.Any(t => t.IsLeaf && t.Value.IsLantern))
                {
                    return LanternSceneTree();
                }
                guaranteedLantern = treeList.Any(t => t.Metadata.GuaranteedLantern);
            }

            // Deduplicate identical SceneNames at the same level.
            List<SceneTree> subtrees = new();
            Dictionary<SceneName, SceneTree> uniqueAtoms = new();
            foreach (var t in treeList)
            {
                if (t.IsTree)
                {
                    subtrees.Add(t);
                }
                else if (t.Value.IsScene)
                {
                    uniqueAtoms[t.Value.SceneName] = t;
                }
            }

            List<SceneTree> list = new();
            subtrees.ForEach(t => list.Add(t));
            uniqueAtoms.Values.ToList().ForEach(t => list.Add(t));

            if (list.Count == 0)
            {
                return guaranteedLantern ? LanternSceneTree() : null;
            }
            else if (list.Count == 1)
            {
                return list[0];
            }
            else
            {
                var tree = SceneTree.CreateTree(op, list);
                tree.Metadata.GuaranteedLantern = guaranteedLantern;
                return tree;
            }
        }

        private static OperatorToken GetOperatorToken(OperatorType type)
        {
            return type == OperatorType.OR ? OperatorToken.OR : OperatorToken.AND;
        }

        private static void ListNewTokens(LogicTree tree, SceneTree sceneTree, List<LogicToken> sink)
        {
            // The authoritative SceneTree for this branch is the one from the top-most conjunction.
            // These comprise the scenes necessary for this Lantern to be unnecessary.
            if (tree.IsLeaf || tree.Op == OperatorType.AND)
            {
                sceneTree ??= tree.Metadata.SceneTree;
            }

            if (tree.IsLeaf)
            {
                if (tree.Value.IsLantern)
                {
                    if (ListLanternTreeTokens(sceneTree, sink))
                    {
                        sink.Add(ParsedToken.LanternToken);
                        sink.Add(OperatorToken.OR);
                    }
                    else
                    {
                        sink.Add(ParsedToken.LanternToken);
                    }
                }
                else
                {
                    sink.Add(tree.Value.Token);
                }
            }
            else
            {
                for (int i = 0; i < tree.Children.Count; i++)
                {
                    ListNewTokens(tree.Children[i], sceneTree, sink);
                    if (i > 0)
                    {
                        sink.Add(GetOperatorToken(tree.Op));
                    }
                }
            }
        }

        private static bool ListLanternTreeTokens(SceneTree tree, List<LogicToken> sink)
        {
            if (tree == null || (tree.IsLeaf && tree.Value.IsLantern))
            {
                return false;
            }

            if (tree.IsLeaf)
            {
                sink.Add(new ComparisonToken(ComparisonType.LT, $"$DarknessLevel[{tree.Value.SceneName}]", "2"));
            }
            else
            {
                int stacked = 0;
                foreach (var t in tree.Children)
                {
                    if (ListLanternTreeTokens(t, sink) && ++stacked > 1)
                    {
                        sink.Add(GetOperatorToken(tree.Op));
                    }
                }
            }
            return true;
        }
    }

    class ParsedToken
    {
        enum TokenType
        {
            Scene,
            Lantern,
            Irrelevant
        }

        public LogicToken Token { get; private set; }
        private readonly TokenType type;
        private readonly SceneName sceneName;

        private ParsedToken(LogicToken token, TokenType type, SceneName sceneName)
        {
            this.Token = token;
            this.type = type;
            this.sceneName = sceneName;
        }

        public static readonly SimpleToken LanternToken = new("LANTERN");
        public static readonly ParsedToken Lantern = new(LanternToken, TokenType.Lantern, null);

        public static ParsedToken Parse(LogicToken token)
        {
            if (token is SimpleToken st)
            {
                string name = st.Write();
                if (name == "LANTERN")
                {
                    return Lantern;
                }

                if (SceneName.IsTransition(name, out SceneName sceneName))
                {
                    return new(token, TokenType.Scene, sceneName);
                }
            }

            return new(token, TokenType.Irrelevant, null);
        }

        public bool IsScene => type == TokenType.Scene;

        public SceneName SceneName
        {
            get
            {
                if (!IsScene)
                {
                    throw new ArgumentException("Illegal access");
                }
                return sceneName;
            }
        }

        public bool IsLantern => type == TokenType.Lantern;

        public bool IsRelevant => type != TokenType.Irrelevant;
    }
}
