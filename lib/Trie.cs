using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Math;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Library
{
    ////start
    class LIB_Trie<T>
    {
        class TrieNode
        {
            public Dictionary<T, TrieNode> Next;
            public TrieNode fail;
            public List<int> Accept;
            public HashSet<int> UnionAccept;
            public int Count;
            public TrieNode()
            {
                Next = new Dictionary<T, TrieNode>();
                Accept = new List<int>();
                UnionAccept = new HashSet<int>();
            }
        }
        bool builtAhoCorasick;
        List<int> patLens;
        TrieNode root;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_Trie()
        {
            root = new TrieNode();
            builtAhoCorasick = false;
            patLens = new List<int>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<(int patIdx, int pos)> AhoCorasick(ReadOnlySpan<T> str) => AhoCorasick((IEnumerable<T>)str.ToArray());
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<(int patIdx, int pos)> AhoCorasick(IEnumerable<T> str)
        {
            if (!builtAhoCorasick)
            {
                builtAhoCorasick = true;
                var q = new Queue<TrieNode>();
                foreach (var item in root.Next)
                {
                    item.Value.fail = root;
                    foreach (var item2 in item.Value.Accept) item.Value.UnionAccept.Add(item2);
                    q.Enqueue(item.Value);
                }
                root.fail = root;
                while (q.Count > 0)
                {
                    var t = q.Dequeue();
                    foreach (var item in t.Next)
                    {
                        foreach (var item2 in item.Value.Accept) item.Value.UnionAccept.Add(item2);
                        q.Enqueue(item.Value);
                        var r = t.fail;
                        while (r != root && !r.Next.ContainsKey(item.Key)) r = r.fail;
                        if (!r.Next.TryGetValue(item.Key, out item.Value.fail))
                        {
                            item.Value.fail = root;
                        }
                        foreach (var item2 in item.Value.fail.UnionAccept)
                        {
                            item.Value.UnionAccept.Add(item2);
                        }
                    }
                }
            }
            var v = root;
            var pos = 0;
            foreach (var item in str.ToArray())
            {
                ++pos;
                while (v != root && !v.Next.ContainsKey(item)) v = v.fail;
                if (!v.Next.TryGetValue(item, out v)) v = root;
                foreach (var item2 in v.UnionAccept) yield return (item2, pos - patLens[item2]);
            }
            yield break;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(ReadOnlySpan<T> str)
        {
            builtAhoCorasick = false;
            var id = root.Count;
            var node = root;
            var len = 0;
            foreach (var item in str)
            {
                ++len;
                ++node.Count;
                TrieNode nextNode;
                if (!node.Next.TryGetValue(item, out nextNode)) nextNode = node.Next[item] = new TrieNode();
                node = nextNode;
            }
            patLens.Add(len);
            ++node.Count;
            node.Accept.Add(id);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(IEnumerable<T> str)
        {
            builtAhoCorasick = false;
            var id = root.Count;
            var node = root;
            var len = 0;
            foreach (var item in str)
            {
                ++len;
                ++node.Count;
                TrieNode nextNode;
                if (!node.Next.TryGetValue(item, out nextNode)) nextNode = node.Next[item] = new TrieNode();
                node = nextNode;
            }
            patLens.Add(len);
            ++node.Count;
            node.Accept.Add(id);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<int> Query(ReadOnlySpan<T> str)
        {
            var ret = new List<int>();
            var node = root;
            foreach (var item in str)
            {
                foreach (var item2 in node.Accept) ret.Add(item2);
                TrieNode nextNode;
                if (!node.Next.TryGetValue(item, out nextNode)) return ret;
                node = nextNode;
            }
            foreach (var item in node.Accept) ret.Add(item);
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<int> Query(IEnumerable<T> str)
        {
            var ret = new List<int>();
            var node = root;
            foreach (var item in str)
            {
                foreach (var item2 in node.Accept) ret.Add(item2);
                TrieNode nextNode;
                if (!node.Next.TryGetValue(item, out nextNode)) return ret;
                node = nextNode;
            }
            foreach (var item in node.Accept) ret.Add(item);
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int[] Find(ReadOnlySpan<T> str)
        {
            var node = root;
            foreach (var item in str)
            {
                TrieNode nextNode;
                if (!node.Next.TryGetValue(item, out nextNode)) return new int[0];
                node = nextNode;
            }
            return node.Accept.ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int[] Find(IEnumerable<T> str)
        {
            var node = root;
            foreach (var item in str)
            {
                TrieNode nextNode;
                if (!node.Next.TryGetValue(item, out nextNode)) return new int[0];
                node = nextNode;
            }
            return node.Accept.ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count() => root.Count;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count(ReadOnlySpan<T> prefix)
        {
            var node = root;
            foreach (var item in prefix)
            {
                TrieNode nextNode;
                if (!node.Next.TryGetValue(item, out nextNode)) return 0;
                node = nextNode;
            }
            return node.Count;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count(IEnumerable<T> prefix)
        {
            var node = root;
            foreach (var item in prefix)
            {
                TrieNode nextNode;
                if (!node.Next.TryGetValue(item, out nextNode)) return 0;
                node = nextNode;
            }
            return node.Count;
        }
    }
    ////end
}