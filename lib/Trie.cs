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
            public List<int> Accept;
            public int Count;
            public TrieNode()
            {
                Next = new Dictionary<T, TrieNode>();
                Accept = new List<int>();
            }
        }
        TrieNode root;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_Trie()
        {
            root = new TrieNode();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(ReadOnlySpan<T> str)
        {
            var id = root.Count;
            var node = root;
            foreach (var item in str)
            {
                ++node.Count;
                TrieNode nextNode;
                if (!node.Next.TryGetValue(item, out nextNode)) nextNode = node.Next[item] = new TrieNode();
                node = nextNode;
            }
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
    }
    ////end
}