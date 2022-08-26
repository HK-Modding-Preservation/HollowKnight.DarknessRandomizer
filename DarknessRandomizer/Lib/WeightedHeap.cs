using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarknessRandomizer.Lib
{
    // A sorted, weighted heap which supports O(log(N)) random removal.
    //
    // Sorted for determinism. This class may be changed in the future for performance or other reasons, but it should always
    // produce the same removal for the same-seeded Random input.
    public class WeightedHeap<T>
    {
        private int size = 0;
        private int weight = 0;

        private T pivot = default;
        private int pivotWeight = 0;
        private WeightedHeap<T> left;
        private WeightedHeap<T> right;

        public WeightedHeap() { }

        private WeightedHeap(IEnumerable<(T, int)> sorted)
        {
            var list = sorted.ToList();
            if (list.Count > 0)
            {
                int mid = list.Count / 2;
                int lw = 0;
                if (mid > 0)
                {
                    left = new(list.GetRange(0, mid));
                    lw = left.Weight();
                }
                (pivot, pivotWeight) = list[mid];
                int rw = 0;
                if (mid < list.Count - 1)
                {
                    right = new(list.GetRange(mid + 1, list.Count - mid - 1));
                    rw = right.Weight();
                }

                size = list.Count;
                weight = lw + pivotWeight + rw;
            }
        }

        public int Size()
        {
            return size;
        }

        public int Weight()
        {
            return weight;
        }

        public bool IsEmpty()
        {
            return size == 0;
        }

        public bool Contains(T t)
        {
            if (size == 0)
            {
                return false;
            }

            int cmp = Comparer<T>.Default.Compare(t, pivot);
            if (cmp < 0)
            {
                return left.Contains(t);
            }
            else if (cmp > 0)
            {
                return right.Contains(t);
            }
            else
            {
                return true;
            }
        }

        public void Clear()
        {
            size = 0;
            weight = 0;
            pivot = default;
            pivotWeight = 0;
            left = null;
            right = null;
        }

        public void Add(T t, int w)
        {
            if (w <= 0)
            {
                throw new ArgumentException(string.Format("Weight (%d) must be a positive integer", w));
            }

            if (size == 0)
            {
                pivot = t;
                pivotWeight = w;
            }
            else
            {
                int cmp = Comparer<T>.Default.Compare(t, pivot);
                if (cmp < 0)
                {
                    (left ??= new()).Add(t, w);
                }
                else if (cmp > 0)
                {
                    (right ??= new()).Add(t, w);
                }
                else
                {
                    throw new ArgumentException(string.Format("Element %s already in heap", t));
                }
            }

            ++size;
            weight += w;
            MaybeRebalance();
        }

        public T Remove(Random r)
        {
            return Remove(r.Next(0, weight)).Item1;
        }

        private (T, int) Remove(int i)
        {
            if (size == 0)
            {
                throw new InvalidOperationException("WeightedHeap is empty");
            }

            int lw = left != null ? left.Weight() : 0;
            (T, int) ret;
            if (i < lw)
            {
                ret = left.Remove(i);
            } else if (i < lw + pivotWeight)
            {
                ret = RemovePivotNoAccounting();
            } else
            {
                ret = right.Remove(i - lw - pivotWeight);
            }

            --size;
            weight -= ret.Item2;
            MaybeRebalance();
            return ret;
        }

        private (T, int) RemovePivotNoAccounting()
        {
            var ret = (pivot, pivotWeight);
            pivot = default;
            pivotWeight = 0;
            return ret;
        }

        private (T, int) RemoveFirst()
        {
            if (size == 0)
            {
                throw new InvalidOperationException("Cannot remove from an empty heap");
            }

            (T, int) ret;
            if (left != null)
            {
                ret = left.RemoveFirst();
            } else
            {
                ret = RemovePivotNoAccounting();
            }

            --size;
            weight -= ret.Item2;
            MaybeRebalance();
            return ret;
        }

        private (T, int) RemoveLast()
        {
            if (size == 0)
            {
                throw new InvalidOperationException("Cannot remove from an empty heap");
            }

            (T, int) ret;
            if (right != null)
            {
                ret = right.RemoveLast();
            } else
            {
                ret = RemovePivotNoAccounting();
            }

            --size;
            weight -= ret.Item2;
            MaybeRebalance();
            return ret;
        }
        public IEnumerable<(T, int)> EnumerateSorted()
        {
            if (left != null)
            {
                foreach (var e in left.EnumerateSorted())
                {
                    yield return e;
                }
            }

            if (pivotWeight > 0)
            {
                yield return (pivot, pivotWeight);
            }

            if (right != null)
            {
                foreach (var e in right.EnumerateSorted())
                {
                    yield return e;
                }
            }
        }

        private void Copy(WeightedHeap<T> that)
        {
            this.size = that.size;
            this.weight = that.weight;
            this.pivot = that.pivot;
            this.pivotWeight = that.pivotWeight;
            this.left = that.left;
            this.right = that.right;
        }

        private void MaybeRebalance()
        {
            if (size == 0)
            {
                return;
            }

            if (pivotWeight == 0)
            {
                if (left != null && (right == null || right.Size() > left.Size()))
                {
                    (pivot, pivotWeight) = left.RemoveLast();
                } else if (right != null)
                {
                    (pivot, pivotWeight) = right.RemoveFirst();
                }
            }

            if (left != null && left.Size() == 0)
            {
                left = null;
            }
            if (right != null && right.Size() == 0)
            {
                right = null;
            }

            int ls = left != null ? left.Size() : 0;
            int rs = right != null ? right.Size() : 0;
            if (size > 10 && 3*Math.Abs(ls - rs) > size)
            {
                Copy(new WeightedHeap<T>(EnumerateSorted().ToList()));
            }
        }
    }
}
