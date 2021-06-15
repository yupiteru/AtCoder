
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
    class LIB_BinaryTrie
    {
        int bitlen;
        const int CHILD_SHIFT = 32;
        const ulong CHILD_MASK = 4294967295;
        struct Node
        {
            public ulong child;
            public int shortcut;
            public uint cnt;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int SetChild(int idx, int tgt)
            {
                child = (child & (CHILD_MASK << (idx * CHILD_SHIFT))) | ((ulong)tgt << ((1 - idx) * CHILD_SHIFT));
                return tgt;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int GetChild(int idx)
            {
                return (int)((child >> ((1 - idx) * CHILD_SHIFT)) & CHILD_MASK);
            }
        }
        int root;
        static int datcnt;
        static int[] pool;
        static int poolcnt;
        static Node[] dat;
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }
        static LIB_BinaryTrie()
        {
            dat = new Node[100000000];
            pool = new int[10000000];
            datcnt = 1;
            poolcnt = 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int NewNode()
        {
            if (poolcnt != 0)
            {
                var ret = pool[--poolcnt];
                dat[ret].child = 0;
                return ret;
            }
            return datcnt++;
        }
        static readonly int NODE_SIZE = Unsafe.SizeOf<Node>();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_BinaryTrie(int bitlen)
        {
            this.bitlen = bitlen;
            root = NewNode();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Add(long val)
        {
            var node = root;
            ref var datref = ref dat[0];
            var list = new int[bitlen];
            bool lastadd = false;
            for (var i = bitlen - 1; i >= 0; --i)
            {
                var bit = (int)((val >> i) & 1);
                ref var refnode = ref Unsafe.Add(ref datref, node);
                if (lastadd || (node = refnode.GetChild(bit)) == 0)
                {
                    node = refnode.SetChild(bit, NewNode());
                    lastadd = true;
                }
                list[i] = node;
            }
            if (lastadd)
            {
                var last = list[0];
                Unsafe.Add(ref datref, last).child = (ulong)val;
                foreach (var item in list)
                {
                    ++Unsafe.Add(ref datref, item).cnt;
                    Unsafe.Add(ref datref, item).shortcut = last;
                }
                ++Count;
                return true;
            }
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(long val)
        {
            var node = root;
            ref var datref = ref dat[0];
            var list = new (int node, int parent, int bit)[bitlen];
            var i = bitlen - 1;
            for (; i >= 0; --i)
            {
                var bit = (int)((val >> i) & 1);
                var p = node;
                if ((node = Unsafe.Add(ref datref, node).GetChild(bit)) != 0) list[i] = (node, p, bit);
                else break;
            }
            if (i == -1)
            {
                foreach (var item in list)
                {
                    ref var parent = ref Unsafe.Add(ref datref, item.parent);
                    parent.shortcut = Unsafe.Add(ref datref, item.node).shortcut;
                    if (--Unsafe.Add(ref datref, item.node).cnt == 0)
                    {
                        parent.SetChild(item.bit, 0);
                        parent.shortcut = Unsafe.Add(ref datref, parent.GetChild(1 - item.bit)).shortcut;
                        pool[poolcnt++] = item.node;
                    }
                }
                --Count;
                return true;
            }
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveAt(long val)
        {
            return false;
            // TODO
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long At(int idx)
        {
            // TODO
            return 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Min()
        {
            // TODO
            return 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Max()
        {
            // TODO
            return 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long XorMin(long val)
        {
            var node = root;
            ref var datref = ref dat[0];
            for (var i = bitlen - 1; i >= 0; --i)
            {
                if (Unsafe.Add(ref datref, node).cnt == 1)
                {
                    node = Unsafe.Add(ref datref, node).shortcut;
                    break;
                }
                var bit = (int)((val >> i) & 1);
                ref var refnode = ref Unsafe.Add(ref datref, node);
                if ((node = refnode.GetChild(bit)) == 0)
                {
                    node = refnode.GetChild(1 - bit);
                }
            }
            return (long)Unsafe.Add(ref datref, node).child ^ val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long XorMax(long val)
        {
            // TODO
            return 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long AndMin(long val)
        {
            // TODO
            return 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long AndMax(long val)
        {
            // TODO
            return 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long OrMin(long val)
        {
            // TODO
            return 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long OrMax(long val)
        {
            // TODO
            return 0;
        }
    }
    ////end
}