
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
        struct Node
        {
            public int zero;
            public int one;
            public uint cnt;
        }
        int root;
        static int datcnt;
        static Stack<int> pool;
        static Node[] dat;
        public int Count
        {
            get;
            private set;
        }
        static LIB_BinaryTrie()
        {
            dat = new Node[100000000];
            pool = new Stack<int>();
            datcnt = 1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int NewNode()
        {
            if (pool.Any())
            {
                var ret = pool.Pop();
                dat[ret].zero = dat[ret].one = 0;
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
                if ((val & (1L << i)) != 0)
                {
                    if (Unsafe.Add(ref datref, node).one == 0)
                    {
                        node = Unsafe.Add(ref datref, node).one = NewNode();
                        lastadd = true;
                    }
                    else node = Unsafe.Add(ref datref, node).one;
                }
                else
                {
                    if (Unsafe.Add(ref datref, node).zero == 0)
                    {
                        node = Unsafe.Add(ref datref, node).zero = NewNode();
                        lastadd = true;
                    }
                    else node = Unsafe.Add(ref datref, node).zero;
                }
                list[i] = node;
            }
            if (lastadd)
            {
                foreach (var item in list) ++Unsafe.Add(ref datref, item).cnt;
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
            var list = new (int node, int parent, bool one)[bitlen];
            var i = bitlen - 1;
            for (; i >= 0; --i)
            {
                if ((val & (1L << i)) != 0)
                {
                    if (Unsafe.Add(ref datref, node).one != 0)
                    {
                        var p = node;
                        node = Unsafe.Add(ref datref, node).one;
                        list[i] = (node, p, true);
                    }
                    else break;
                }
                else
                {
                    if (Unsafe.Add(ref datref, node).zero != 0)
                    {
                        var p = node;
                        node = Unsafe.Add(ref datref, node).zero;
                        list[i] = (node, p, false);
                    }
                    else break;
                }
            }
            if (i == -1)
            {
                foreach (var item in list)
                {
                    if (--Unsafe.Add(ref datref, item.node).cnt == 0)
                    {
                        if (item.one) Unsafe.Add(ref datref, item.parent).one = 0;
                        else Unsafe.Add(ref datref, item.parent).zero = 0;
                        pool.Push(item.node);
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
            var ret = 0L;
            var node = root;
            ref var datref = ref dat[0];
            for (var i = bitlen - 1; i >= 0; --i)
            {
                ret <<= 1;
                if ((val & (1L << i)) != 0)
                {
                    if (Unsafe.Add(ref datref, node).one != 0)
                    {
                        node = Unsafe.Add(ref datref, node).one;
                    }
                    else
                    {
                        node = Unsafe.Add(ref datref, node).zero;
                        ret |= 1;
                    }
                }
                else
                {
                    if (Unsafe.Add(ref datref, node).zero != 0)
                    {
                        node = Unsafe.Add(ref datref, node).zero;
                    }
                    else
                    {
                        node = Unsafe.Add(ref datref, node).one;
                        ret |= 1;
                    }
                }
            }
            return ret;
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