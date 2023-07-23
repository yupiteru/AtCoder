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
    abstract class LIB_BeamSearch
    {
        protected class IntArray
        {
            static int historyCount = 0;
            static int beforeHistoryCount = 0;
            static byte targetCount = 0;
            static int[][] targetList = new int[255][];
            const int HISTORY_INDEX_MASK = (1 << 25) - 1;
            static (byte targetIdx, int index, int xorval)[] history = new (byte, int, int)[1 << 25];

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
                    targetList[hist.targetIdx][hist.index] ^= hist.xorval;
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
                    targetList[hist.targetIdx][hist.index] ^= hist.xorval;
                }
            }

            int len1 = -1;
            int len2 = -1;
            int len3 = -1;
            int len4 = -1;
            byte thisTargetIdx = 0;
            int[] ary;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IntArray(int len1)
            {
                targetList[thisTargetIdx = targetCount++] = ary = new int[len1];
                this.len1 = len1;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IntArray(int len1, int len2)
            {
                targetList[thisTargetIdx = targetCount++] = ary = new int[len1 * len2];
                this.len1 = len1;
                this.len2 = len2;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IntArray(int len1, int len2, int len3)
            {
                targetList[thisTargetIdx = targetCount++] = ary = new int[len1 * len2 * len3];
                this.len1 = len1;
                this.len2 = len2;
                this.len3 = len3;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IntArray(int len1, int len2, int len3, int len4)
            {
                targetList[thisTargetIdx = targetCount++] = ary = new int[len1 * len2 * len3 * len4];
                this.len1 = len1;
                this.len2 = len2;
                this.len3 = len3;
                this.len4 = len4;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void Set(int index, int val)
            {
                if (ary[index] == val) return;
                history[historyCount++] = (thisTargetIdx, index, ary[index] ^ val);
                historyCount &= HISTORY_INDEX_MASK;
                ary[index] = val;
            }

            public int this[int index]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return ary[index]; }
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set { Set(index, value); }
            }
            public int this[int index1, int index2]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return ary[index1 * len2 + index2]; }
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set { Set(index1 * len2 + index2, value); }
            }
            public int this[int index1, int index2, int index3]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return ary[index1 * len2 * len3 + index2 * len3 + index3]; }
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set { Set(index1 * len2 * len3 + index2 * len3 + index3, value); }
            }
            public int this[int index1, int index2, int index3, int index4]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return ary[index1 * len2 * len3 * len4 + index2 * len3 * len4 + index3 * len4 + index4]; }
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set { Set(index1 * len2 * len3 * len4 + index2 * len3 * len4 + index3 * len4 + index4, value); }
            }
        }

        struct Node
        {
            public int parent;
            public int child;
            public int prev;
            public int next;
            public (int l, int r) patch;
            public string action;
        }

        LIB_Deque<int> waitingReUse = new LIB_Deque<int>();
        Node[] nodeList = new Node[1000000];
        int nodeCount = 0;
        int root;

        protected abstract void Initialize();
        protected abstract string[] ListupActions();
        protected abstract long DoAction(string act, int turn);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int NewNode()
        {
            if (waitingReUse.Count == 0) return ++nodeCount;
            return waitingReUse.PopBack();
        }

        void Remove(int nodeIdx)
        {
            waitingReUse.PushBack(nodeIdx);
            ref var node = ref nodeList[nodeIdx];
            if (node.prev == 0 && node.next == 0)
            {
                Remove(node.parent);
            }
            else if (node.prev == 0)
            {
                nodeList[node.parent].child = node.next;
                nodeList[node.next].prev = 0;
            }
            else if (node.next == 0)
            {
                nodeList[node.prev].next = 0;
            }
            else
            {
                nodeList[node.prev].next = node.next;
                nodeList[node.next].prev = node.prev;
            }
        }

        public string[] Run(int width, int maxTurn)
        {
            return Run(width, -1, maxTurn);
        }
        public string[] Run(int initialWidth, int totalMillis, int maxTurn)
        {
            var nextQueue = new LIB_PriorityQueue();
            var answer = new List<string>();

            Initialize();
            root = NewNode();
            IntArray.Batch();

            var width = initialWidth;
            ref var nodeListRef = ref nodeList[0];
            var startTime = DateTime.Now;
            var checkpointTime = startTime;
            for (var i = 0; i < maxTurn; ++i)
            {
                nextQueue.Reset();
                if (totalMillis >= 0 && (i & 15) == 0 && i > 0)
                {
                    var turnStartTime = DateTime.Now;
                    var elapsed = (turnStartTime - startTime).TotalMilliseconds;
                    var lastTime = totalMillis - elapsed;
                    var keisu = lastTime * 16 / ((maxTurn - i) * (turnStartTime - checkpointTime).TotalMilliseconds);
                    width = (int)(width * Max(Min(keisu, 1.1), 0.9));
                    checkpointTime = turnStartTime;
                }
                var nodeIdx = root;
                while (true)
                {
                    while (Unsafe.Add(ref nodeListRef, nodeIdx).child != 0)
                    {
                        nodeIdx = Unsafe.Add(ref nodeListRef, nodeIdx).child;
                        IntArray.Apply(Unsafe.Add(ref nodeListRef, nodeIdx).patch);
                    }
                    var beforeNodeIdx = 0;
                    foreach (var act in ListupActions())
                    {
                        var score = DoAction(act, i);
                        var newNodeIdx = NewNode();
                        ref var node = ref Unsafe.Add(ref nodeListRef, newNodeIdx);
                        node.child = 0;
                        node.prev = 0;
                        node.next = 0;
                        node.parent = nodeIdx;
                        node.patch = IntArray.Batch();
                        node.action = act;
                        if (beforeNodeIdx != 0)
                        {
                            node.prev = beforeNodeIdx;
                            Unsafe.Add(ref nodeListRef, beforeNodeIdx).next = newNodeIdx;
                        }
                        if (Unsafe.Add(ref nodeListRef, nodeIdx).child == 0) Unsafe.Add(ref nodeListRef, nodeIdx).child = newNodeIdx;
                        beforeNodeIdx = newNodeIdx;
                        nextQueue.Push(score, newNodeIdx);

                        IntArray.Rollback(node.patch);
                    }
                    IntArray.Rollback(Unsafe.Add(ref nodeListRef, nodeIdx).patch);
                    while (Unsafe.Add(ref nodeListRef, nodeIdx).next == 0 && Unsafe.Add(ref nodeListRef, nodeIdx).parent != 0)
                    {
                        nodeIdx = Unsafe.Add(ref nodeListRef, nodeIdx).parent;
                        IntArray.Rollback(Unsafe.Add(ref nodeListRef, nodeIdx).patch);
                    }
                    while (nextQueue.Count > width) Remove(nextQueue.Pop().Value);
                    if (Unsafe.Add(ref nodeListRef, nodeIdx).parent == 0) break;
                    nodeIdx = Unsafe.Add(ref nodeListRef, nodeIdx).next;
                    IntArray.Apply(Unsafe.Add(ref nodeListRef, nodeIdx).patch);
                }
                while (Unsafe.Add(ref nodeListRef, Unsafe.Add(ref nodeListRef, root).child).next == 0)
                {
                    waitingReUse.PushBack(root);
                    root = Unsafe.Add(ref nodeListRef, root).child;
                    answer.Add(Unsafe.Add(ref nodeListRef, root).action);
                    IntArray.Apply(Unsafe.Add(ref nodeListRef, root).patch);
                    Unsafe.Add(ref nodeListRef, root).parent = 0;
                    Unsafe.Add(ref nodeListRef, root).patch = (0, 0);
                }
            }

            var maxNode = 0;
            while (nextQueue.Count > 0) maxNode = nextQueue.Pop().Value;

            var forwardNodeList = new List<int>();
            while (maxNode != root)
            {
                forwardNodeList.Add(maxNode);
                maxNode = nodeList[maxNode].parent;
            }
            forwardNodeList.Reverse();
            foreach (var item in forwardNodeList)
            {
                IntArray.Apply(nodeList[item].patch);
                answer.Add(nodeList[item].action);
            }

            return answer.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_BeamSearch()
        {
        }
    }
    ////end
}