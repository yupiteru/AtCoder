
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
            public void SetChild(int idx, int tgt)
            {
                child = (child & (CHILD_MASK << (idx * CHILD_SHIFT))) | ((ulong)tgt << ((1 - idx) * CHILD_SHIFT));
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
        static LIB_BinaryTrie()
        {
            dat = new Node[50000000];
            pool = new int[50000000];
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
                dat[ret].shortcut = 0;
                dat[ret].cnt = 0;
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
        public void Add(long val, bool allowDup = true)
        {
            var node = root;
            ref var datref = ref dat[0];
            var list = new int[bitlen];
            ref var listref = ref list[0];
            var i = bitlen - 1;
            for (; i >= 0; --i)
            {
                var bit = (int)((val >> i) & 1);
                ref var refnode = ref Unsafe.Add(ref datref, node);
                if (refnode.shortcut == node)
                {
                    var tmp = NewNode();
                    ref var tmpnode = ref Unsafe.Add(ref datref, tmp);
                    tmpnode = refnode;
                    tmpnode.shortcut = tmp;
                    refnode.child = 0;
                    refnode.SetChild((int)((tmpnode.child >> i) & 1), tmp);
                }
                if ((node = refnode.GetChild(bit)) == 0)
                {
                    refnode.SetChild(bit, node = NewNode());
                    Unsafe.Add(ref listref, i--) = node;
                    break;
                }
                Unsafe.Add(ref listref, i) = node;
            }
            var last = Unsafe.Add(ref listref, ++i);
            var add = 1U;
            if (Unsafe.Add(ref datref, last).child == (ulong)val && !allowDup) --add;
            Unsafe.Add(ref datref, last).child = (ulong)val;
            for (; i < bitlen; ++i)
            {
                var item = Unsafe.Add(ref listref, i);
                Unsafe.Add(ref datref, item).cnt += add;
                Unsafe.Add(ref datref, item).shortcut = last;
            }
            Unsafe.Add(ref datref, root).cnt += add;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(long val)
        {
            var node = root;
            ref var datref = ref dat[0];
            var list = new (int node, int parent, int bit)[bitlen];
            ref var listref = ref list[0];
            var i = bitlen - 1;
            for (; i >= 0; --i)
            {
                var bit = (int)((val >> i) & 1);
                var p = node;
                ref var refnode = ref Unsafe.Add(ref datref, node);
                if (refnode.shortcut == node)
                {
                    if (refnode.child == (ulong)val) break;
                    return false;
                }
                if ((node = refnode.GetChild(bit)) == 0) return false;
                Unsafe.Add(ref listref, i) = (node, p, bit);
            }
            for (++i; i < bitlen; ++i)
            {
                var item = Unsafe.Add(ref listref, i);
                ref var parent = ref Unsafe.Add(ref datref, item.parent);
                parent.shortcut = Unsafe.Add(ref datref, item.node).shortcut;
                if (--Unsafe.Add(ref datref, item.node).cnt == 0)
                {
                    parent.SetChild(item.bit, 0);
                    parent.shortcut = Unsafe.Add(ref datref, parent.GetChild(1 - item.bit)).shortcut;
                    pool[poolcnt++] = item.node;
                }
            }
            --Unsafe.Add(ref datref, root).cnt;
            Unsafe.Add(ref datref, root).shortcut = 0;
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveAt(long idx)
        {
            if (idx < 0 || Count() <= idx) return false;
            var node = root;
            ref var datref = ref dat[0];
            var list = new (int node, int parent, int bit)[bitlen];
            ref var listref = ref list[0];
            var i = bitlen - 1;
            for (; i >= 0; --i)
            {
                ref var refnode = ref Unsafe.Add(ref datref, node);
                if (refnode.shortcut == node) break;
                var bit = 0;
                var p = node;
                var zero = refnode.GetChild(0);
                if (Unsafe.Add(ref datref, node = zero).cnt <= idx)
                {
                    bit = 1;
                    node = refnode.GetChild(1);
                    idx -= (int)Unsafe.Add(ref datref, zero).cnt;
                }
                Unsafe.Add(ref listref, i) = (node, p, bit);
            }
            for (++i; i < bitlen; ++i)
            {
                var item = Unsafe.Add(ref listref, i);
                ref var parent = ref Unsafe.Add(ref datref, item.parent);
                parent.shortcut = Unsafe.Add(ref datref, item.node).shortcut;
                if (--Unsafe.Add(ref datref, item.node).cnt == 0)
                {
                    parent.SetChild(item.bit, 0);
                    parent.shortcut = Unsafe.Add(ref datref, parent.GetChild(1 - item.bit)).shortcut;
                    pool[poolcnt++] = item.node;
                }
            }
            --Unsafe.Add(ref datref, root).cnt;
            Unsafe.Add(ref datref, root).shortcut = 0;
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long At(long idx)
        {
            if (idx < 0 || Count() <= idx) return 0;
            var node = root;
            ref var datref = ref dat[0];
            for (var i = bitlen - 1; i >= 0; --i)
            {
                ref var refnode = ref Unsafe.Add(ref datref, node);
                ref var refshortcut = ref Unsafe.Add(ref datref, refnode.shortcut);
                if (refnode.cnt == refshortcut.cnt)
                {
                    return (long)refshortcut.child;
                }
                var zero = refnode.GetChild(0);
                if (Unsafe.Add(ref datref, node = zero).cnt <= idx)
                {
                    node = refnode.GetChild(1);
                    idx -= (int)Unsafe.Add(ref datref, zero).cnt;
                }
            }
            return (long)Unsafe.Add(ref datref, node).child;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Count(long val)
        {
            var node = root;
            ref var datref = ref dat[0];
            var i = bitlen - 1;
            for (; i >= 0; --i)
            {
                ref var refnode = ref Unsafe.Add(ref datref, node);
                ref var refshortcut = ref Unsafe.Add(ref datref, refnode.shortcut);
                if (refnode.cnt == refshortcut.cnt)
                {
                    if (refshortcut.child == (ulong)val) return refshortcut.cnt;
                    return 0;
                }
                if ((node = refnode.GetChild((int)((val >> i) & 1))) == 0) break;
            }
            if (i == -1) return Unsafe.Add(ref datref, node).cnt;
            return 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Count() => dat[root].cnt;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint CountAt(long idx)
        {
            if (idx < 0 || Count() <= idx) return 0;
            var node = root;
            ref var datref = ref dat[0];
            for (var i = bitlen - 1; i >= 0; --i)
            {
                ref var refnode = ref Unsafe.Add(ref datref, node);
                ref var refshortcut = ref Unsafe.Add(ref datref, refnode.shortcut);
                if (refnode.cnt == refshortcut.cnt)
                {
                    return refshortcut.cnt;
                }
                var zero = refnode.GetChild(0);
                if (Unsafe.Add(ref datref, node = zero).cnt <= idx)
                {
                    node = refnode.GetChild(1);
                    idx -= (int)Unsafe.Add(ref datref, zero).cnt;
                }
            }
            return Unsafe.Add(ref datref, node).cnt;
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
                ref var refnode = ref Unsafe.Add(ref datref, node);
                ref var refshortcut = ref Unsafe.Add(ref datref, refnode.shortcut);
                if (refnode.cnt == refshortcut.cnt)
                {
                    return (long)refshortcut.child ^ val;
                }
                var bit = (int)((val >> i) & 1);
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
        public long this[long index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return At(index); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set { }
        }
    }
    ////end
}