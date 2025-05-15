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
    // use LIB_HeuristicStateBase
    class LIB_ChokudaiSearch
    {
        struct Node
        {
            public int parent;
            public int childCount;
            public (int l, int r) patch;
            public LIB_OperatorBase ope;
            public bool deleted;
            public int depth;
        }

        LIB_Deque<int> waitingReUse = new LIB_Deque<int>();
        Node[] nodeList = new Node[1000000];
        int nodeCount = 0;
        int root;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int NewNode()
        {
            // waitingReUse に削除されたノードが格納されており、これを使い回す
            // nodeList のできるだけ近い位置のノードを使うことでキャッシュヒット率を上げる意図があります（上がるのかは不明）
            if (waitingReUse.Count == 0) return ++nodeCount;
            return waitingReUse.PopBack();
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public string[] Run(HeuristicStateDiffInternal state, int totalMillis, int maxTurn)
        {
            var startTime = DateTime.Now;
            // ###=======古いコメント！=======###
            // 状態を木構造で持ちます
            // Node 構造体で1つの要素を表現
            // Node の実体はリストで管理して、親や子などの情報はインデックス番号で保持します
            // 0番目の要素は番兵です（nullチェック回避のため）
            //
            // Node は以下の情報を持ちます
            // parent - 親の Node のインデックス
            // child - 一番左の子のインデックス
            // next - 右の兄弟のインデックス
            // prev - 左の兄弟のインデックス
            // patch - この Node で行った操作の履歴
            // action - この Node で行った操作（最終的に出力される文字列）
            //
            // Node は操作の履歴と親兄弟の情報のみ保持して、状態自体は継承先のクラスが保持する方針
            // ###=======古いコメントここまで！=======###

            // 状態の初期化
            // 継承先のクラスで実装する
            state.Initialize();

            // 初期状態を表す Node を作成
            root = NewNode();
            HeuristicStateDiffInternal.Batch();

            const int MAX_NODE_COUNT_PER_TURN = 200;
            const int MAX_LOOP_PER_TURN = 1;
            var targetNodeIndecies = new List<int>();
            var nextTargetNodeIndecies = new List<int>();
            targetNodeIndecies.Add(root);
            var nodeIdx = root;
            var nextDepth = 0;
            ref var nodeListRef = ref nodeList[0];
            var testGlobalLoopCount = 0;
            var nextQueue = Enumerable.Repeat(0, maxTurn).Select(_ => new LIB_IntervalQueue<(long score, int nodeIdx)>()).ToArray();
            Func<int, bool, string[]> calcAnswer = (maxNode, doOperate) =>
            {
                while (nodeList[root].depth < nodeList[nodeIdx].depth)
                {
                    HeuristicStateDiffInternal.Rollback(nodeList[nodeIdx].patch);
                    nodeIdx = nodeList[nodeIdx].parent;
                }

                var ret = new List<string>();
                var backwardNodeList = new List<int>();
                var tmpmaxNode = maxNode;
                while (tmpmaxNode != root)
                {
                    backwardNodeList.Add(tmpmaxNode);
                    tmpmaxNode = nodeList[tmpmaxNode].parent;
                }
                backwardNodeList.Reverse();
                foreach (var item in backwardNodeList)
                {
                    if (doOperate) HeuristicStateDiffInternal.Apply(nodeList[item].patch);
                    ret.Add(nodeList[item].ope.GetOperateString());
                }
                return ret.ToArray();
            };
            var testTurnTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalMilliseconds < totalMillis)
            {
                nextTargetNodeIndecies.Clear();
                foreach (var firstTargetNodeIdx in targetNodeIndecies)
                {
                    if (nodeList[firstTargetNodeIdx].deleted) continue;
                    if ((DateTime.Now - startTime).TotalMilliseconds >= totalMillis) break;

                    var targetNodeIdx = firstTargetNodeIdx;
                    var forwardNodeList = new List<int>();
                    while (nodeList[nodeIdx].depth < nodeList[targetNodeIdx].depth)
                    {
                        forwardNodeList.Add(targetNodeIdx);
                        targetNodeIdx = nodeList[targetNodeIdx].parent;
                    }
                    while (nodeList[targetNodeIdx].depth < nodeList[nodeIdx].depth)
                    {
                        HeuristicStateDiffInternal.Rollback(nodeList[nodeIdx].patch);
                        nodeIdx = nodeList[nodeIdx].parent;
                    }
                    while (targetNodeIdx != nodeIdx)
                    {
                        forwardNodeList.Add(targetNodeIdx);
                        targetNodeIdx = nodeList[targetNodeIdx].parent;
                        HeuristicStateDiffInternal.Rollback(nodeList[nodeIdx].patch);
                        nodeIdx = nodeList[nodeIdx].parent;
                    }
                    for (var i = forwardNodeList.Count - 1; i >= 0; --i)
                    {
                        HeuristicStateDiffInternal.Apply(nodeList[nodeIdx = forwardNodeList[i]].patch);
                    }

                    // ListupActions で可能な操作を列挙し、操作ごとに子を生やします
                    // ハッシュが usedHash に含まれている（過去と同一の盤面）なら、その操作はスキップします
                    foreach (var ope in state.ListupActions(nextDepth))
                    {
                        if ((DateTime.Now - startTime).TotalMilliseconds >= totalMillis) break;

                        // DoAction で操作（順遷移）を行う
                        var score = state.DoAction(ope, nextDepth);

                        var newNodeIdx = NewNode();
                        nodeList[newNodeIdx].deleted = false;
                        ref var node = ref Unsafe.Add(ref nodeListRef, newNodeIdx);
                        node.parent = nodeIdx;
                        node.depth = nodeList[nodeIdx].depth + 1;
                        node.patch = HeuristicStateDiffInternal.Batch();
                        node.ope = ope;
                        node.childCount = 0;
                        ++nodeList[nodeIdx].childCount;

                        nextQueue[nextDepth].Push((score.score, newNodeIdx));

                        HeuristicStateDiffInternal.Rollback(node.patch);
                    }

                    while (nextQueue[nextDepth].Count > MAX_NODE_COUNT_PER_TURN)
                    {
                        var deleteIdx = nextQueue[nextDepth].PopMin().nodeIdx;

                        while (true)
                        {
                            if (deleteIdx == nodeIdx)
                            {
                                HeuristicStateDiffInternal.Rollback(nodeList[nodeIdx].patch);
                                nodeIdx = nodeList[nodeIdx].parent;
                            }

                            waitingReUse.PushBack(deleteIdx);
                            ref var node = ref nodeList[deleteIdx];
                            node.ope.Unuse();
                            node.deleted = true;
                            HeuristicStateDiffInternal.DeleteHistory(node.patch);
                            if (--nodeList[node.parent].childCount == 0)
                            {
                                deleteIdx = node.parent;
                            }
                            else break;
                        }
                    }
                }

                Func<int, int> forward = depth =>
                {
                    if (depth >= maxTurn - 2) return 0;
                    return depth + 1;
                };
                var zroedCount = 0;
                if (nextDepth == maxTurn - 1)
                {
                    nextDepth = 0;
                    ++testGlobalLoopCount;
                    Console.Error.WriteLine($"testGlobalLoopCount: {testGlobalLoopCount} elapsed: {(-(testTurnTime - (testTurnTime = DateTime.Now))).TotalMilliseconds}");
                }
                while (nextQueue[nextDepth].Count == 0)
                {
                    ++zroedCount;
                    nextDepth = forward(nextDepth);
                    if (zroedCount >= maxTurn) break;
                }
                if (nextQueue[nextDepth].Count == 0) break;
                for (var i = 0; i < MAX_LOOP_PER_TURN; ++i)
                {
                    if (nextQueue[nextDepth].Count == 0) break;
                    nextTargetNodeIndecies.Add(nextQueue[nextDepth].PopMax().nodeIdx);
                }
                (targetNodeIndecies, nextTargetNodeIndecies) = (nextTargetNodeIndecies, targetNodeIndecies);
                ++nextDepth;
            }

            var maxNode = nextQueue[maxTurn - 1].Max.nodeIdx;

            return calcAnswer(maxNode, true).ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_ChokudaiSearch()
        {
        }
    }
    ////end
}