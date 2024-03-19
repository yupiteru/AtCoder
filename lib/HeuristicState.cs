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
    abstract class LIB_OperatorBase
    {
        public int Priority;
        public abstract string GetOperateString();
        public void Unuse()
        {
            HeuristicStateInternal.unusedOperatorPool.PushBack(this);
        }
    }

    abstract class HeuristicStateInternal
    {
        const int HISTORY_INDEX_MASK = (1 << 25) - 1;
        static (int index, int xorval)[] history = new (int, int)[1 << 25];
        static int[] memory = new int[0];
        static int historyCount = 0;
        static int beforeHistoryCount = 0;
        static public LIB_Deque<LIB_OperatorBase> unusedOperatorPool = new LIB_Deque<LIB_OperatorBase>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public (int l, int r) Batch()
        {
            var ret = (beforeHistoryCount, historyCount);
            beforeHistoryCount = historyCount;
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public void Apply((int l, int r) ope)
        {
            for (var i = ope.l; i != ope.r; i = (i + 1) & HISTORY_INDEX_MASK)
            {
                ref var hist = ref history[i];
                memory[hist.index] ^= hist.xorval;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public void Rollback((int l, int r) ope)
        {
            var i = ope.r;
            while (i != ope.l)
            {
                i = (i - 1) & HISTORY_INDEX_MASK;
                ref var hist = ref history[i];
                memory[hist.index] ^= hist.xorval;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Set(int index, int val)
        {
            if (memory[index] == val) return;
            history[historyCount++] = (index, memory[index] ^ val);
            historyCount &= HISTORY_INDEX_MASK;
            memory[index] = val;
        }

        protected class IntArray
        {
            int len1 = -1;
            int len2 = -1;
            int len3 = -1;
            int len4 = -1;
            int dataOffset = 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IntArray(int len1)
            {
                dataOffset = memory.Length;
                memory = memory.Concat(new int[len1]).ToArray();
                this.len1 = len1;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IntArray(int len1, int len2)
            {
                dataOffset = memory.Length;
                memory = memory.Concat(new int[len1 * len2]).ToArray();
                this.len1 = len1;
                this.len2 = len2;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IntArray(int len1, int len2, int len3)
            {
                dataOffset = memory.Length;
                memory = memory.Concat(new int[len1 * len2 * len3]).ToArray();
                this.len1 = len1;
                this.len2 = len2;
                this.len3 = len3;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IntArray(int len1, int len2, int len3, int len4)
            {
                dataOffset = memory.Length;
                memory = memory.Concat(new int[len1 * len2 * len3 * len4]).ToArray();
                this.len1 = len1;
                this.len2 = len2;
                this.len3 = len3;
                this.len4 = len4;
            }
            public int this[int index]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return memory[dataOffset + index]; }
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set { Set(dataOffset + index, value); }
            }
            public int this[int index1, int index2]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return memory[dataOffset + index1 * len2 + index2]; }
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set { Set(dataOffset + index1 * len2 + index2, value); }
            }
            public int this[int index1, int index2, int index3]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return memory[dataOffset + index1 * len2 * len3 + index2 * len3 + index3]; }
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set { Set(dataOffset + index1 * len2 * len3 + index2 * len3 + index3, value); }
            }
            public int this[int index1, int index2, int index3, int index4]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return memory[dataOffset + index1 * len2 * len3 * len4 + index2 * len3 * len4 + index3 * len4 + index4]; }
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set { Set(dataOffset + index1 * len2 * len3 * len4 + index2 * len3 * len4 + index3 * len4 + index4, value); }
            }
        }
        protected class LongArray
        {
            int len1 = -1;
            int len2 = -1;
            int len3 = -1;
            int len4 = -1;
            int dataOffset = 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public LongArray(int len1)
            {
                dataOffset = memory.Length;
                memory = memory.Concat(new int[len1 * 2]).ToArray();
                this.len1 = len1;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public LongArray(int len1, int len2)
            {
                dataOffset = memory.Length;
                memory = memory.Concat(new int[len1 * len2 * 2]).ToArray();
                this.len1 = len1;
                this.len2 = len2;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public LongArray(int len1, int len2, int len3)
            {
                dataOffset = memory.Length;
                memory = memory.Concat(new int[len1 * len2 * len3 * 2]).ToArray();
                this.len1 = len1;
                this.len2 = len2;
                this.len3 = len3;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public LongArray(int len1, int len2, int len3, int len4)
            {
                dataOffset = memory.Length;
                memory = memory.Concat(new int[len1 * len2 * len3 * len4 * 2]).ToArray();
                this.len1 = len1;
                this.len2 = len2;
                this.len3 = len3;
                this.len4 = len4;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static long GetAndMerge(int idx) => ((long)memory[idx] << 32) | (long)memory[idx + 1];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void SplitAndSet(int idx, long val)
            {
                memory[idx] = (int)(val >> 32);
                memory[idx + 1] = (int)val;
            }
            public long this[int index]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return GetAndMerge(dataOffset + index); }
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set { SplitAndSet(dataOffset + index, value); }
            }
            public long this[int index1, int index2]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return GetAndMerge(dataOffset + index1 * len2 + index2); }
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set { SplitAndSet(dataOffset + index1 * len2 + index2, value); }
            }
            public long this[int index1, int index2, int index3]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return GetAndMerge(dataOffset + index1 * len2 * len3 + index2 * len3 + index3); }
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set { SplitAndSet(dataOffset + index1 * len2 * len3 + index2 * len3 + index3, value); }
            }
            public long this[int index1, int index2, int index3, int index4]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return GetAndMerge(dataOffset + index1 * len2 * len3 * len4 + index2 * len3 * len4 + index3 * len4 + index4); }
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set { SplitAndSet(dataOffset + index1 * len2 * len3 * len4 + index2 * len3 * len4 + index3 * len4 + index4, value); }
            }
        }
        public abstract void Debug();
        public abstract void Initialize();
        public abstract LIB_OperatorBase[] ListupActions();
        public abstract (long score, long hash) DoAction(LIB_OperatorBase ope, int turn);
    }

    abstract class LIB_HeuristicStateBase<TOperator> : HeuristicStateInternal where TOperator : LIB_OperatorBase, new()
    {
        public TOperator CreateOperator()
        {
            if (unusedOperatorPool.Count > 0)
            {
                var ret = unusedOperatorPool.PopBack();
                return (TOperator)ret;
            }
            return new TOperator();
        }
        protected abstract (long score, long hash) DoAction(TOperator ope, int turn);
        public override (long score, long hash) DoAction(LIB_OperatorBase ope, int turn) => DoAction((TOperator)ope, turn);
    }
    ////end
}