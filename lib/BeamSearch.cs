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
    class LIB_BeamSearch
    {
        struct Node
        {
            public int parent;
            public int child;
            public int prev;
            public int next;
            public (int l, int r) patch;
            public LIB_OperatorBase ope;
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

        void Remove(int nodeIdx)
        {
            // ノードを削除します
            // ノードの削除によって親ノードの子が無くなった場合は、親ノードも削除します（再帰的に）
            waitingReUse.PushBack(nodeIdx);
            ref var node = ref nodeList[nodeIdx];
            node.ope.Unuse();
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

        public string[] Run(HeuristicStateInternal state, int width, int maxTurn)
        {
            return Run(state, width, -1, maxTurn, true);
        }
        public string[] Run(HeuristicStateInternal state, int initialWidth, int totalMillis, int maxTurn, bool fixHaba = false)
        {
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

            // 確定済みの操作を保存するためのリスト
            var answer = new List<string>();

            // 状態の初期化
            // 継承先のクラスで実装する
            state.Initialize();

            // 初期状態を表す Node を作成
            root = NewNode();
            HeuristicStateInternal.Batch();

            var width = initialWidth;
            ref var nodeListRef = ref nodeList[0];
            var startTime = DateTime.Now;
            var checkpointTime = startTime;
            var usedHash = new HashSet<long>();
            var nextQueue = new LIB_PriorityQueue(); // このキューは最小要素を取り出す＝スコアの最大化になる（途中要らないものを Pop すると最小のものから削除されるので）
            for (var i = 0; i < maxTurn; ++i)
            {
                // ハッシュ履歴が多くなると遅くなるので、3ターンごとにクリアする
                // 頻度は問題によって変えるかも
                if (i % 99 == 0) usedHash.Clear();

                Console.Error.WriteLine($"turn: {i} width:{width} score: {nextQueue.Peek.Key}");

                var turnStartTime = DateTime.Now;
                // elapsed - 経過時間
                // lastTime - 残り時間
                var elapsed = (turnStartTime - startTime).TotalMilliseconds;
                var lastTime = totalMillis - elapsed;
                if (totalMillis >= 0 && lastTime < 0) break;
                if (!fixHaba && (i & 15) == 0 && i > 0)
                {
                    // 残り時間に応じて幅を調整します
                    // 一回の調整幅は 0.9?1.1 倍まで
                    var keisu = lastTime * 16 / ((maxTurn - i) * (turnStartTime - checkpointTime).TotalMilliseconds);
                    width = (int)(width * Max(Min(keisu, 1.1), 0.9));
                    width = Max(width, 10); // 最小幅は 10（問題によって変えるかも）
                    checkpointTime = turnStartTime;
                }

                // キューを空にします
                // このキューは、1ターンの処理の途中で幅を超える要素を削除するために使います
                // 木には生きているノードしかおらず、毎ターン木の root から走査するので、
                // キューは問答無用で空にする
                nextQueue.Reset();

                var nodeIdx = root; // 今見ているノードのインデックス
                while (true)
                {
                    // 一番左の子ノードに移動できるだけ移動する
                    while (Unsafe.Add(ref nodeListRef, nodeIdx).child != 0)
                    {
                        nodeIdx = Unsafe.Add(ref nodeListRef, nodeIdx).child;
                        HeuristicStateInternal.Apply(Unsafe.Add(ref nodeListRef, nodeIdx).patch);
                    }
                    // ここで nodeIdx は一番左の葉ノードになっている

                    // needRemoveIdx は、子が一つも無くなったらノードを削除するために使われます
                    // 以下 foreach の中で、子の要素が全て usedHash に含まれている場合に子が一つも無くなる
                    var needRemoveIdx = nodeIdx;

                    // beforeNodeIdx は兄弟ノードをつなぐために使われます
                    var beforeNodeIdx = 0;
                    //Debug();

                    // ListupActions で可能な操作を列挙し、操作ごとに子を生やします
                    // ハッシュが usedHash に含まれている（過去と同一の盤面）なら、その操作はスキップします
                    foreach (var ope in state.ListupActions())
                    {
                        // DoAction で操作（順遷移）を行う
                        var score = state.DoAction(ope, i);
                        if (usedHash.Contains(score.Item2))
                        {
                            HeuristicStateInternal.Rollback(HeuristicStateInternal.Batch());
                            continue;
                        }
                        usedHash.Add(score.Item2);
                        needRemoveIdx = 0; // 有効な子がいたので、親を削除しないようにする
                        var newNodeIdx = NewNode();
                        ref var node = ref Unsafe.Add(ref nodeListRef, newNodeIdx);
                        node.child = 0;
                        node.prev = 0;
                        node.next = 0;
                        node.parent = nodeIdx;
                        node.patch = HeuristicStateInternal.Batch();
                        node.ope = ope;
                        if (beforeNodeIdx != 0)
                        {
                            // 兄ノードがある場合
                            node.prev = beforeNodeIdx;
                            Unsafe.Add(ref nodeListRef, beforeNodeIdx).next = newNodeIdx;
                        }
                        else
                        {
                            // 兄ノードがいない（newNodeIdx が一番左の子）なら、親とつなぐ
                            Unsafe.Add(ref nodeListRef, nodeIdx).child = newNodeIdx;
                        }

                        beforeNodeIdx = newNodeIdx;
                        nextQueue.Push(score.Item1, newNodeIdx);

                        HeuristicStateInternal.Rollback(node.patch);
                    }

                    // なんかのケースで 0 番が汚染されていたことがあったので、ここでリセット
                    Unsafe.Add(ref nodeListRef, 0).prev = 0;
                    Unsafe.Add(ref nodeListRef, 0).child = 0;

                    // 有効な要素数を width に制限する
                    while (nextQueue.Count > width) Remove(nextQueue.Pop().Value);

                    // 木上を移動します
                    HeuristicStateInternal.Rollback(Unsafe.Add(ref nodeListRef, nodeIdx).patch);
                    // next == 0、つまり右の兄弟がいない場合は親に移動します
                    while (Unsafe.Add(ref nodeListRef, nodeIdx).next == 0 && Unsafe.Add(ref nodeListRef, nodeIdx).parent != 0)
                    {
                        nodeIdx = Unsafe.Add(ref nodeListRef, nodeIdx).parent;
                        HeuristicStateInternal.Rollback(Unsafe.Add(ref nodeListRef, nodeIdx).patch);
                    }

                    // 親がいない（root に戻った）場合は終了
                    if (Unsafe.Add(ref nodeListRef, nodeIdx).parent == 0) break;

                    // 右の兄弟に移動します
                    nodeIdx = Unsafe.Add(ref nodeListRef, nodeIdx).next;
                    HeuristicStateInternal.Apply(Unsafe.Add(ref nodeListRef, nodeIdx).patch);

                    // needRemoveIdx > 0 なら、それは子が一つもないノードなので、ここで削除
                    if (needRemoveIdx > 0) Remove(needRemoveIdx);
                }

                // root の子がただ一つなら、それを root に昇格させる
                // 旧 root は確定操作として answer に追加
                while (Unsafe.Add(ref nodeListRef, root).child != 0 && Unsafe.Add(ref nodeListRef, Unsafe.Add(ref nodeListRef, root).child).next == 0)
                {
                    waitingReUse.PushBack(root);
                    root = Unsafe.Add(ref nodeListRef, root).child;
                    answer.Add(Unsafe.Add(ref nodeListRef, root).ope.GetOperateString());
                    Unsafe.Add(ref nodeListRef, root).ope.Unuse();
                    HeuristicStateInternal.Apply(Unsafe.Add(ref nodeListRef, root).patch);
                    Unsafe.Add(ref nodeListRef, root).parent = 0;
                    Unsafe.Add(ref nodeListRef, root).patch = (0, 0);
                }
            }

            // nextQueue から最後に取れる要素＝最大スコアの要素を取り出します
            var maxNode = 0;
            var maxv = 0L;
            while (nextQueue.Count > 0)
            {
                var pop = nextQueue.Pop();
                maxNode = pop.Value;
                maxv = pop.Key;
            }
            //Console.Error.WriteLine($"lastScore: {maxv}");

            // 最大スコアの要素から親を辿っていき、操作の履歴を answer に追加します
            var backwardNodeList = new List<int>();
            while (maxNode != root)
            {
                backwardNodeList.Add(maxNode);
                maxNode = nodeList[maxNode].parent;
            }
            backwardNodeList.Reverse();
            foreach (var item in backwardNodeList)
            {
                answer.Add(nodeList[item].ope.GetOperateString());
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