using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Math;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Library;
using System.Diagnostics.CodeAnalysis;

namespace Program
{
    public static class ProblemA
    {
        static bool SAIKI = false;
        static public int numberOfRandomCases = 0;
        static public void MakeTestCase(List<string> _input, List<string> _output, ref Func<string[], bool> _outputChecker)
        {
        }
        abstract class LIB_MCTS<Operation, State>
            where Operation : class
            where State : class
        {
            public enum Result
            {
                Lose = 0, Draw = 1, Win = 2
            }
            public LIB_MCTS()
            {
            }
            /// <summary>
            /// </summary>
            /// <param name="destState"></param>
            /// <param name="operation"></param>
            /// <returns> rollback を返すこと</returns>
            abstract public Action<State> Action(State destState, Operation operation);
            abstract public Result Rollout(State srcState);
            abstract public List<Operation> Listup(State srcState);

            class Node
            {
                public Node parent;
                public Operation ope;
                public int selected;
                public int wins;
                public Node[] children;
                public Action<State> rollback;
                public Node(Node parent, Operation ope)
                {
                    this.parent = parent;
                    this.ope = ope;
                }
            }

            const int SIKII = 4;
            Node root = null;
            State state;

            public void Initialize(State initState)
            {
                root = new Node(null, null);
                state = initState;
            }

            public void Select(Operation ope)
            {
                var error = true;
                foreach (var item in root.children)
                {
                    if (item.ope.GetHashCode() == ope.GetHashCode())
                    {
                        error = false;
                        root = item;
                        break;
                    }
                }
                if (error)
                {
                    throw new Exception();
                }
                Action(state, ope);
            }

            public Operation Run(DateTime limit)
            {
                var node = root;
                if (node.children == null) node.children = Listup(state).Select(e => new Node(node, e)).ToArray();
                if (node.children.Length == 0) return null;
                var rolloutCount = 0;
                var me = false;
                while (DateTime.Now < limit)
                {
                    me = !me;

                    // selection
                    Node maxNode = null;
                    var maxUCB = double.MinValue;
                    foreach (var item in node.children)
                    {
                        if (item.selected == 0)
                        {
                            maxNode = item;
                            break;
                        }
                        var ucb = 0.0;
                        if (me) ucb = ((double)item.wins / (item.selected * 2)) + Sqrt(2 * Log(node.selected) / item.selected);
                        else ucb = (1.0 - (double)item.wins / (item.selected * 2)) + Sqrt(2 * Log(node.selected) / item.selected);
                        if (maxUCB.Chmax(ucb))
                        {
                            maxNode = item;
                        }
                    }

                    node = maxNode;
                    node.rollback = Action(state, node.ope);

                    // expansion
                    if (node.selected >= SIKII)
                    {
                        if (node.children == null)
                        {
                            node.children = Listup(state).Select(e => new Node(node, e)).ToArray();
                        }
                        if (node.children.Length != 0) continue;
                    }

                    me = false;

                    // evaluation
                    var result = Rollout(state);
                    ++rolloutCount;

                    // backup
                    while (node != root)
                    {
                        ++node.selected;
                        node.wins += (int)result;
                        node.rollback(state);
                        node = node.parent;
                    }
                    ++node.selected;
                    node.wins += (int)result;
                }

                while (node != root)
                {
                    node.rollback(state);
                    node = node.parent;
                }

                Console.Error.WriteLine($"Rollout count: {rolloutCount}");

                // play
                var maxProb = -1;
                Operation ret = null;
                foreach (var item in node.children)
                {
                    Console.Error.WriteLine($"{item.ope}: {(double)item.wins / (item.selected * 2)} ({item.selected} selected)");
                    if (maxProb.Chmax(item.selected)) ret = item.ope;
                }
                return ret;
            }
        }

        class OthelloState
        {
            public ulong boradW;
            public ulong boradB;
            public int N;
            public bool nextIsPlayer;
            public bool playerIsWhite;
            public OthelloState Copy()
            {
                var ret = new OthelloState();
                ret.boradW = boradW;
                ret.boradB = boradB;
                ret.N = N;
                ret.nextIsPlayer = nextIsPlayer;
                ret.playerIsWhite = playerIsWhite;
                return ret;
            }
        }
        class OthelloOperation
        {
            public int row;
            public int col;
            public bool putCellIsBlack;
            public bool isPass;
            public override string ToString()
            {
                return $"({row}, {col})";
            }
            public override int GetHashCode()
            {
                var hash = putCellIsBlack ? 1 : 0;
                hash += isPass ? 2 : 0;
                hash += row * 10;
                hash += col * 100;
                return hash;
            }
        }
        class Othello : LIB_MCTS<OthelloOperation, OthelloState>
        {
            public override Action<OthelloState> Action(OthelloState destState, OthelloOperation operation)
            {
                if (operation.isPass)
                {
                    destState.nextIsPlayer = !destState.nextIsPlayer;
                    return state =>
                    {
                        state.nextIsPlayer = !state.nextIsPlayer;
                    };
                }
                var fliped = new List<(int row, int col)>();
                var i = operation.row;
                var j = operation.col;
                Func<int, int, int, int, bool> fun = null;
                fun = (r, c, dr, dc) =>
                {
                    if (r < 0 || r >= destState.N || c < 0 || c >= destState.N) return false;
                    if (operation.putCellIsBlack && (destState.boradB & (1UL << (r * destState.N + c))) != 0) return true;
                    if (!operation.putCellIsBlack && (destState.boradW & (1UL << (r * destState.N + c))) != 0) return true;
                    if ((destState.boradB & (1UL << (r * destState.N + c))) == 0 && (destState.boradW & (1UL << (r * destState.N + c))) == 0) return false;
                    if (fun(r + dr, c + dc, dr, dc))
                    {
                        destState.boradW &= ~(1UL << (r * destState.N + c));
                        destState.boradB &= ~(1UL << (r * destState.N + c));
                        if (operation.putCellIsBlack) destState.boradB |= (1UL << (r * destState.N + c));
                        else destState.boradW |= (1UL << (r * destState.N + c));
                        fliped.Add((r, c));
                        return true;
                    }
                    return false;
                };
                fun(i + 1, j, 1, 0);
                fun(i - 1, j, -1, 0);
                fun(i, j + 1, 0, 1);
                fun(i, j - 1, 0, -1);
                fun(i + 1, j + 1, 1, 1);
                fun(i + 1, j - 1, 1, -1);
                fun(i - 1, j + 1, -1, 1);
                fun(i - 1, j - 1, -1, -1);
                if (operation.putCellIsBlack) destState.boradB |= (1UL << (i * destState.N + j));
                else destState.boradW |= (1UL << (i * destState.N + j));
                destState.nextIsPlayer = !destState.nextIsPlayer;
                return state =>
                {
                    foreach (var item in fliped)
                    {
                        destState.boradW &= ~(1UL << (item.row * destState.N + item.col));
                        destState.boradB &= ~(1UL << (item.row * destState.N + item.col));
                        if (operation.putCellIsBlack) destState.boradW |= (1UL << (item.row * destState.N + item.col));
                        else destState.boradB |= (1UL << (item.row * destState.N + item.col));
                    }
                    if (operation.putCellIsBlack) destState.boradB &= ~(1UL << (i * destState.N + j));
                    else destState.boradW &= ~(1UL << (i * destState.N + j));
                    destState.nextIsPlayer = !destState.nextIsPlayer;
                };
            }

            public override List<OthelloOperation> Listup(OthelloState srcState)
            {
                var ret = new List<OthelloOperation>();
                var nextCellIsWhite = false;
                if (srcState.playerIsWhite)
                {
                    if (srcState.nextIsPlayer)
                    {
                        nextCellIsWhite = true;
                    }
                }
                else
                {
                    if (!srcState.nextIsPlayer)
                    {
                        nextCellIsWhite = true;
                    }
                }
                Func<bool, int, int, bool> check = (targetIsWhite, i, j) =>
                {
                    if ((srcState.boradB & (1UL << (i * srcState.N + j))) != 0 || (srcState.boradW & (1UL << (i * srcState.N + j))) != 0) return false;
                    Func<int, int, int, int, bool, bool> fun = null;
                    fun = (r, c, dr, dc, flag) =>
                    {
                        if (r < 0 || r >= srcState.N || c < 0 || c >= srcState.N) return false;
                        if (targetIsWhite && (srcState.boradW & (1UL << (r * srcState.N + c))) != 0) return flag;
                        if (!targetIsWhite && (srcState.boradB & (1UL << (r * srcState.N + c))) != 0) return flag;
                        if ((srcState.boradB & (1UL << (r * srcState.N + c))) == 0 && (srcState.boradW & (1UL << (r * srcState.N + c))) == 0) return false;
                        flag = true;
                        return fun(r + dr, c + dc, dr, dc, flag);
                    };
                    if (fun(i + 1, j, 1, 0, false)) return true;
                    if (fun(i - 1, j, -1, 0, false)) return true;
                    if (fun(i, j + 1, 0, 1, false)) return true;
                    if (fun(i, j - 1, 0, -1, false)) return true;
                    if (fun(i + 1, j + 1, 1, 1, false)) return true;
                    if (fun(i + 1, j - 1, 1, -1, false)) return true;
                    if (fun(i - 1, j + 1, -1, 1, false)) return true;
                    if (fun(i - 1, j - 1, -1, -1, false)) return true;
                    return false;
                };
                var gameend = true;
                for (var i = 0; i < srcState.N; ++i)
                {
                    for (var j = 0; j < srcState.N; ++j)
                    {
                        if ((srcState.boradB & (1UL << (i * srcState.N + j))) == 0 && (srcState.boradW & (1UL << (i * srcState.N + j))) == 0)
                        {
                            if (check(nextCellIsWhite, i, j))
                            {
                                gameend = false;
                                var ope = new OthelloOperation();
                                ope.row = i;
                                ope.col = j;
                                ope.putCellIsBlack = !nextCellIsWhite;
                                ret.Add(ope);
                            }
                            else if (gameend && check(!nextCellIsWhite, i, j))
                            {
                                gameend = false;
                            }
                        }
                    }
                }
                if (gameend) return ret;
                if (ret.Count == 0)
                {
                    var ope = new OthelloOperation();
                    ope.isPass = true;
                    ret.Add(ope);
                }
                return ret;
            }

            public override Result Rollout(OthelloState srcState)
            {
                var state = srcState.Copy();
                while (true)
                {
                    var opes = Listup(state);
                    if (opes.Count == 0) break;
                    OthelloOperation ope = null;
                    {
                        foreach (var item in opes)
                        {
                            if (item.row == 0 && item.col == 0 ||
                                item.row == 0 && item.col == state.N - 1 ||
                                item.row == state.N - 1 && item.col == 0 ||
                                item.row == state.N - 1 && item.col == state.N - 1)
                            {
                                ope = item;
                                break;
                            }
                        }
                    }
                    if (ope == null)
                    {
                        ope = opes[(int)(xorshift % opes.Count)];
                    }
                    Action(state, ope);
                }
                var whiteCount = LIB_BitUtil.PopCount(state.boradW);
                var blackCount = LIB_BitUtil.PopCount(state.boradB);

                if (whiteCount == blackCount) return Result.Draw;
                var whiteIsWin = whiteCount > blackCount;
                if (whiteIsWin)
                {
                    if (state.playerIsWhite) return Result.Lose;
                    else return Result.Win;
                }
                else
                {
                    if (state.playerIsWhite) return Result.Win;
                    else return Result.Lose;
                }
            }
        }

        static void PrintState(OthelloState state)
        {
            Console.Write("**");
            for (var i = 0; i < state.N; ++i)
            {
                Console.Write($" {i}");
            }
            Console.WriteLine("**");
            for (var i = 0; i < state.N; ++i)
            {
                Console.Write($" {i}");
                for (var j = 0; j < state.N; ++j)
                {
                    if ((state.boradW & (1UL << (i * state.N + j))) != 0) Console.Write("ii");
                    else if ((state.boradB & (1UL << (i * state.N + j))) != 0) Console.Write("**");
                    else Console.Write("  ");
                }
                Console.WriteLine("**");
            }
            Console.Write("**");
            for (var i = 0; i < state.N; ++i)
            {
                Console.Write("**");
            }
            Console.WriteLine("**");
            Console.WriteLine();
            Console.Error.Flush();
            Console.Out.Flush();
        }
        static public void Solve()
        {
            var othello = new Othello();
            var state = new OthelloState();
            state.N = 8;
            state.boradW |= 1UL << (4 * state.N + 4);
            state.boradW |= 1UL << (3 * state.N + 3);
            state.boradB |= 1UL << (3 * state.N + 4);
            state.boradB |= 1UL << (4 * state.N + 3);
            state.playerIsWhite = true;
            state.nextIsPlayer = false;

            othello.Initialize(state);

            while (true)
            {
                PrintState(state);
                if (state.nextIsPlayer)
                {
                    var line = Console.ReadLine().Split(" ").Select(int.Parse).ToArray();
                    var ope = new OthelloOperation();
                    ope.row = line[0];
                    ope.col = line[1];
                    ope.putCellIsBlack = !state.playerIsWhite;
                    othello.Select(ope);
                }
                else
                {
                    var mctsAct = othello.Run(DateTime.Now.AddMilliseconds(5000));
                    if (mctsAct == null) break;
                    othello.Select(mctsAct);
                }
                if (othello.Listup(state).Count == 0) break;
            }
            PrintState(state);
            var whiteCount = LIB_BitUtil.PopCount(state.boradW);
            var blackCount = LIB_BitUtil.PopCount(state.boradB);
            Console.WriteLine($"white: {whiteCount}  black: {blackCount}");
            Console.Error.Flush();
            Console.Out.Flush();
            Console.ReadLine();
        }
        class Printer : StreamWriter
        {
            public override IFormatProvider FormatProvider { get { return CultureInfo.InvariantCulture; } }
            public Printer(Stream stream) : base(stream, new UTF8Encoding(false, true)) { base.AutoFlush = false; }
            public Printer(Stream stream, Encoding encoding) : base(stream, encoding) { base.AutoFlush = false; }
        }
        static LIB_FastIO fastio = new LIB_FastIODebug();
        static public void Main(string[] args) { if (args.Length == 0) { fastio = new LIB_FastIO(); Console.SetOut(new Printer(Console.OpenStandardOutput())); } if (SAIKI) { var t = new Thread(Solve, 134217728); t.Start(); t.Join(); } else Solve(); Console.Out.Flush(); }
        static long NN => fastio.Long();
        static double ND => fastio.Double();
        static string NS => fastio.Scan();
        static long[] NNList(long N) => Repeat(0, N).Select(_ => NN).ToArray();
        static double[] NDList(long N) => Repeat(0, N).Select(_ => ND).ToArray();
        static string[] NSList(long N) => Repeat(0, N).Select(_ => NS).ToArray();
        static long Count<T>(this IEnumerable<T> x, Func<T, bool> pred) => Enumerable.Count(x, pred);
        static IEnumerable<T> Repeat<T>(T v, long n) => Enumerable.Repeat<T>(v, (int)n);
        static IEnumerable<int> Range(long s, long c) => Enumerable.Range((int)s, (int)c);
        static IOrderedEnumerable<T> OrderByRand<T>(this IEnumerable<T> x) => Enumerable.OrderBy(x, _ => xorshift);
        static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> x) => Enumerable.OrderBy(x.OrderByRand(), e => e);
        static IOrderedEnumerable<T1> OrderBy<T1, T2>(this IEnumerable<T1> x, Func<T1, T2> selector) => Enumerable.OrderBy(x.OrderByRand(), selector);
        static IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> x) => Enumerable.OrderByDescending(x.OrderByRand(), e => e);
        static IOrderedEnumerable<T1> OrderByDescending<T1, T2>(this IEnumerable<T1> x, Func<T1, T2> selector) => Enumerable.OrderByDescending(x.OrderByRand(), selector);
        static IOrderedEnumerable<string> OrderBy(this IEnumerable<string> x) => x.OrderByRand().OrderBy(e => e, StringComparer.OrdinalIgnoreCase);
        static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> x, Func<T, string> selector) => x.OrderByRand().OrderBy(selector, StringComparer.OrdinalIgnoreCase);
        static IOrderedEnumerable<string> OrderByDescending(this IEnumerable<string> x) => x.OrderByRand().OrderByDescending(e => e, StringComparer.OrdinalIgnoreCase);
        static IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> x, Func<T, string> selector) => x.OrderByRand().OrderByDescending(selector, StringComparer.OrdinalIgnoreCase);
        static string Join<T>(this IEnumerable<T> x, string separator = "") => string.Join(separator, x);
        static uint xorshift { get { _xsi.MoveNext(); return _xsi.Current; } }
        static IEnumerator<uint> _xsi = _xsc();
        static IEnumerator<uint> _xsc() { uint x = 123456789, y = 362436069, z = 521288629, w = (uint)(DateTime.Now.Ticks & 0xffffffff); while (true) { var t = x ^ (x << 11); x = y; y = z; z = w; w = (w ^ (w >> 19)) ^ (t ^ (t >> 8)); yield return w; } }
        static bool Chmax<T>(this ref T lhs, T rhs) where T : struct, IComparable<T> { if (lhs.CompareTo(rhs) < 0) { lhs = rhs; return true; } return false; }
        static bool Chmin<T>(this ref T lhs, T rhs) where T : struct, IComparable<T> { if (lhs.CompareTo(rhs) > 0) { lhs = rhs; return true; } return false; }
        static void Fill<T>(this T[] array, T value) => array.AsSpan().Fill(value);
        static void Fill<T>(this T[,] array, T value) => MemoryMarshal.CreateSpan(ref array[0, 0], array.Length).Fill(value);
        static void Fill<T>(this T[,,] array, T value) => MemoryMarshal.CreateSpan(ref array[0, 0, 0], array.Length).Fill(value);
        static void Fill<T>(this T[,,,] array, T value) => MemoryMarshal.CreateSpan(ref array[0, 0, 0, 0], array.Length).Fill(value);
    }
}
