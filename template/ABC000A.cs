using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC000A
    {
        static public void Main(string[] args)
        {

            Console.WriteLine();
        }

        static class Console_
        {
            private static Queue<string> param = new Queue<string>();
            public static string NextString()
            {
                if (param.Count == 0) foreach (var item in Console.ReadLine().Split(' ')) param.Enqueue(item);
                return param.Dequeue();
            }
        }
        static int NextInt => int.Parse(Console_.NextString());
        static List<int> NextIntList(int N)
        {
            var ret = new List<int>(N);
            for (int i = 0; i < N; ++i) ret.Add(NextInt);
            return ret;
        }
        static long NextLong => long.Parse(Console_.NextString());
        static List<long> NextLongList(int N)
        {
            var ret = new List<long>(N);
            for (int i = 0; i < N; ++i) ret.Add(NextLong);
            return ret;
        }
        static string NextString => Console_.NextString();
        static void Sort<T>(List<T> l) where T : IComparable => l.Sort();
        static void RevSort<T>(List<T> l) where T : IComparable => l.Sort((x, y) => y.CompareTo(x));
        static IEnumerable<long> Prime(long x)
        {
            if (x < 2) yield break;
            yield return 2;
            var halfx = x / 2;
            var table = new bool[halfx + 1];
            var max = (long)(Math.Sqrt(x) / 2);
            for (long i = 1; i <= max; ++i)
            {
                if (table[i]) continue;
                var add = 2 * i + 1;
                yield return add;
                for (long j = 2 * i * (i + 1); j <= halfx; j += add) table[j] = true;
            }
            for (long i = max + 1; i <= halfx; ++i) if (!table[i]) yield return 2 * i + 1;
        }
        static IEnumerable<long> Divisor(long x)
        {
            if (x < 1) yield break;
            var max = (long)Math.Sqrt(x);
            for (long i = 1; i < max; ++i)
            {
                if (x % i != 0) continue;
                yield return i;
                if (i != x / i) yield return x / i;
            }
        }
        static long GCD(long a, long b)
        {
            long tmpa = a, tmpb = b;
            while (tmpb > 0) { var tmp = tmpb; tmpb = tmpa % tmpb; tmpa = tmp; }
            return tmpa;
        }
        class PriorityQueue<T> where T : IComparable
        {
            private List<T> heap;
            private Comparison<T> comp;
            public PriorityQueue(int cap, Comparison<T> comp, bool asc = true) { heap = new List<T>(cap); this.comp = asc ? comp : (x, y) => comp(y, x); }
            public PriorityQueue(Comparison<T> comp, bool asc = true) { heap = new List<T>(); this.comp = asc ? comp : (x, y) => comp(y, x); }
            public PriorityQueue(int cap, bool asc = true) : this(cap, (x, y) => x.CompareTo(y), asc) { }
            public PriorityQueue(bool asc = true) : this((x, y) => x.CompareTo(y), asc) { }
            public void Push(T val)
            {
                var idx = heap.Count;
                heap.Add(val);
                while (idx > 0)
                {
                    var nidx = (idx - 1) / 2;
                    if (comp(val, heap[nidx]) >= 0) break;
                    heap[idx] = heap[nidx];
                    idx = nidx;
                }
                heap[idx] = val;
            }
            public T Peek => heap[0];
            public int Count => heap.Count;
            public T Pop
            {
                get
                {
                    var ret = heap[0];
                    var val = heap[heap.Count - 1];
                    heap.RemoveAt(heap.Count - 1);
                    if (heap.Count == 0) return ret;
                    var idx = 0;
                    while (idx * 2 + 1 < heap.Count)
                    {
                        var childIdx1 = idx * 2 + 1;
                        var childIdx2 = idx * 2 + 2;
                        if (childIdx2 < heap.Count && comp(heap[childIdx1], heap[childIdx2]) > 0) childIdx1 = childIdx2;
                        if (comp(val, heap[childIdx1]) <= 0) break;
                        heap[idx] = heap[childIdx1];
                        idx = childIdx1;
                    }
                    heap[idx] = val;
                    return ret;
                }
                private set { }
            }
        }
        class PriorityQueue<TKey, TValue> where TKey : IComparable
        {
            private PriorityQueue<Tuple<TKey, TValue>> queue;
            public PriorityQueue(int cap, Comparison<TKey> comp, bool asc = true) { queue = new PriorityQueue<Tuple<TKey, TValue>>(cap, (x, y) => comp(x.Item1, y.Item1), asc); }
            public PriorityQueue(Comparison<TKey> comp, bool asc = true) { queue = new PriorityQueue<Tuple<TKey, TValue>>((x, y) => comp(x.Item1, y.Item1), asc); }
            public PriorityQueue(int cap, bool asc = true) : this(cap, (x, y) => x.CompareTo(y), asc) { }
            public PriorityQueue(bool asc = true) : this((x, y) => x.CompareTo(y), asc) { }
            public void Push(TKey key, TValue val) => queue.Push(Tuple.Create(key, val));
            public Tuple<TKey, TValue> Peek => queue.Peek;
            public int Count => queue.Count;
            public Tuple<TKey, TValue> Pop => queue.Pop;
        }
    }
}
