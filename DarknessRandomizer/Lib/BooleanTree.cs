using RandomizerCore.StringLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Lib
{
    // An N-ary tree where each parent node has at least 2 children, and those children are all either leaf nodes, or
    // binary nodes of the opposite operator type.
    //
    // T is the node type, and stored only on leaves.
    // M is the metadata type, and stored on every node.
    public class BooleanTree<T, M>
    {
        private class OpNode
        {
            public OperatorType op;
            public List<BooleanTree<T, M>> nodes = new();

            public OpNode(OperatorType op)
            {
                this.op = op;
            }
        }

        private readonly Variant<T, OpNode> contents;
        public M Metadata;

        private BooleanTree(Variant<T, OpNode> contents)
        {
            this.contents = contents;
        }

        public static BooleanTree<T, M> CreateLeaf(T leaf) => new(new(leaf));

        public static BooleanTree<T, M> CreateTree(OperatorType op, IEnumerable<BooleanTree<T, M>> trees)
        {
            OpNode node = new(op);
            var list = trees.ToList();
            if (list.Count < 2)
            {
                throw new ArgumentException($"BooleanTree requires two or more arguments; receives {list.Count}");
            }

            if (list.All(t => t.IsLeaf || t.Op == op))
            {
                list.ForEach(t => t.ExpandOneLevel().ToList().ForEach(t2 => node.nodes.Add(t2)));
                return new(new(node));
            }
            else
            {
                list.ForEach(t => node.nodes.Add(t));
            }
            return new(new(node));
        }

        public bool IsLeaf => contents.HasFirst;

        public T Value => contents.First;

        public bool IsTree => contents.HasSecond;

        public OperatorType Op => contents.Second.op;

        public IReadOnlyList<BooleanTree<T, M>> Children => contents.Second.nodes;

        private IEnumerable<BooleanTree<T, M>> ExpandOneLevel()
        {
            if (IsLeaf)
            {
                yield return this;
            }
            else
            {
                foreach (var t in Children)
                {
                    yield return t;
                }
            }
        }

    }
}
