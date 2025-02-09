using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Math;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Collections;

namespace Library
{
    ////start
    static partial class LIB_Static
    {
        public static LIB_HashSet<T> LIB_ToHashSet<T>(this IEnumerable<T> source) where T : IEquatable<T>
        {
            var ret = new LIB_HashSet<T>();
            foreach (var item in source) ret.Add(item);
            return ret;
        }
        public static LIB_Dictionary<TKey, TValue> LIB_ToDictionary<T, TKey, TValue>(this IEnumerable<T> source, Func<T, TKey> keySelector, Func<T, TValue> elementSelector) where TKey : IEquatable<TKey>
        {
            var ret = new LIB_Dictionary<TKey, TValue>();
            foreach (var item in source) ret[keySelector(item)] = elementSelector(item);
            return ret;
        }
    }

    class LIB_Dictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEquatable<LIB_Dictionary<TKey, TValue>> where TKey : IEquatable<TKey>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(LIB_Dictionary<TKey, TValue> x)
        {
            if (Count != x.Count) return false;
            foreach (var kv in this)
            {
                if (!x.ContainsKey(kv.Key) || !x[kv.Key].Equals(kv.Value)) return false;
            }
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object x) => x == null ? false : Equals((LIB_Dictionary<TKey, TValue>)x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => totalHash;

        public TValue this[TKey key]
        {
            get => GetOrInsert(key);
            set => AddOrUpdate(key, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int Hash(ulong h)
        {
            unchecked
            {
                var key = h << 32 | h;
                key = (~key) + (key << 18);
                key = key ^ (key >> 31);
                key = key * 21;
                key = key ^ (key >> 11);
                key = key + (key << 6);
                key = key ^ (key >> 22);
                return (int)key;
            }
        }
        int[] st;
        bool[] kouhoAdded;
        KeyValuePair<TKey, TValue>[] bck;
        int mask;
        int prode;
        int totalHash;
        Func<TKey, TValue> defval = _ => default;
        public int Count
        {
            get;
            private set;
        }
        LIB_Deque<int> elemKouho;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_Dictionary(Func<TKey, TValue> defval = null)
        {
            prode = -1;
            st = new int[0];
            kouhoAdded = new bool[0];
            bck = new KeyValuePair<TKey, TValue>[0];
            elemKouho = new LIB_Deque<int>();
            if (defval != null) this.defval = defval;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int FindEmpty(TKey key)
        {
            var h = Hash((ulong)key.GetHashCode());
            ref int stref = ref st[0];
            for (var delta = 0; ; ++delta)
            {
                var i = (h + delta) & mask;
                if (Unsafe.Add(ref stref, i) != 2)
                {
                    if (prode < delta) prode = delta;
                    return i;
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int FindFilled(TKey key)
        {
            if (Count == 0) return -1;
            var h = Hash((ulong)key.GetHashCode());
            ref int stref = ref st[0];
            ref KeyValuePair<TKey, TValue> bckref = ref bck[0];
            for (var delta = 0; delta <= prode; ++delta)
            {
                var i = (h + delta) & mask;
                var sti = Unsafe.Add(ref stref, i);
                if (sti == 2)
                {
                    if (Unsafe.Add(ref bckref, i).Key.Equals(key)) return i;
                }
                else if (sti == 0) return -1;
            }
            return -1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int FindOrAllocate(TKey key)
        {
            var h = Hash((ulong)key.GetHashCode());
            var hole = -1;
            var delta = 0;
            ref int stref = ref st[0];
            ref KeyValuePair<TKey, TValue> bckref = ref bck[0];
            for (; delta <= prode; ++delta)
            {
                var i = (h + delta) & mask;
                var sti = Unsafe.Add(ref stref, i);
                if (sti == 2)
                {
                    if (Unsafe.Add(ref bckref, i).Key.Equals(key)) return i;
                }
                else if (sti == 0) return i;
                else
                {
                    if (hole == -1) hole = i;
                }
            }
            if (hole != -1) return hole;
            for (; ; ++delta)
            {
                var i = (h + delta) & mask;
                if (Unsafe.Add(ref stref, i) != 2)
                {
                    prode = delta;
                    return i;
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Reserve(int nextCnt)
        {
            var requiredCnt = nextCnt + (nextCnt >> 1) + 1;
            var nowlen = bck.Length;
            if (requiredCnt > nowlen)
            {
                nextCnt = 4;
                while (nextCnt < requiredCnt) nextCnt <<= 1;
            }
            else if (nextCnt <= nowlen >> 2) nextCnt = Max(4, nowlen >> 1);
            else return;
            var oldSt = new int[nextCnt];
            var oldKouhoAdded = new bool[nextCnt];
            var oldBck = new KeyValuePair<TKey, TValue>[nextCnt];
            { var t = oldSt; oldSt = st; st = t; }
            { var t = oldKouhoAdded; oldKouhoAdded = kouhoAdded; kouhoAdded = t; }
            { var t = oldBck; oldBck = bck; bck = t; }
            mask = nextCnt - 1;
            Count = 0;
            prode = 0;
            elemKouho.Clear();
            if (nowlen == 0) return;
            ref int stref = ref st[0];
            ref bool kouhoAddedref = ref kouhoAdded[0];
            ref KeyValuePair<TKey, TValue> bckref = ref bck[0];
            ref int oldstref = ref oldSt[0];
            ref bool oldkouhoAddedref = ref oldKouhoAdded[0];
            ref KeyValuePair<TKey, TValue> oldbckref = ref oldBck[0];
            for (var pos = 0; pos < nowlen; ++pos)
            {
                if (Unsafe.Add(ref oldstref, pos) == 2)
                {
                    ref KeyValuePair<TKey, TValue> oldbckpos = ref Unsafe.Add(ref oldbckref, pos);
                    var i = FindEmpty(oldbckpos.Key);
                    Unsafe.Add(ref stref, i) = 2;
                    Unsafe.Add(ref kouhoAddedref, i) = true;
                    Unsafe.Add(ref bckref, i) = oldbckpos;
                    elemKouho.PushBack(i);
                    ++Count;
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddOrUpdate(TKey key, TValue val)
        {
            Reserve(Count + 1);
            var i = FindOrAllocate(key);
            if (st[i] != 2)
            {
                st[i] = 2;
                bck[i] = new KeyValuePair<TKey, TValue>(key, val);
                if (!kouhoAdded[i])
                {
                    kouhoAdded[i] = true;
                    elemKouho.PushBack(i);
                }
                totalHash ^= key.GetHashCode();
                ++Count;
            }
            else bck[i] = new KeyValuePair<TKey, TValue>(key, val);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(TKey key)
        {
            var i = FindFilled(key);
            if (i == -1) return false;
            st[i] = 1;
            totalHash ^= key.GetHashCode();
            --Count;
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(TKey key) => FindFilled(key) != -1;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue GetOrInsert(TKey key, Func<TKey, TValue> val = null)
        {
            Reserve(Count + 1);
            var i = FindOrAllocate(key);
            if (st[i] != 2)
            {
                st[i] = 2;
                bck[i] = new KeyValuePair<TKey, TValue>(key, val == null ? defval(key) : val(key));
                if (!kouhoAdded[i])
                {
                    kouhoAdded[i] = true;
                    elemKouho.PushBack(i);
                }
                totalHash ^= key.GetHashCode();
                ++Count;
            }
            return bck[i].Value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue GetAndErase(TKey key)
        {
            var i = FindFilled(key);
            st[i] = 1;
            totalHash ^= key.GetHashCode();
            --Count;
            return bck[i].Value;
        }

        public KeyCollection Keys => new KeyCollection(this);
        public class KeyCollection : IEnumerable<TKey>, ICollection<TKey>, IReadOnlyCollection<TKey>
        {
            private LIB_Dictionary<TKey, TValue> dict;
            public KeyCollection(LIB_Dictionary<TKey, TValue> dict)
            {
                this.dict = dict;
            }
            public int Count => dict.Count;
            public bool IsReadOnly => true;
            public void Add(TKey item) => throw new NotSupportedException();
            public void Clear() => throw new NotSupportedException();
            public bool Contains(TKey item) => dict.ContainsKey(item);
            public void CopyTo(TKey[] array, int arrayIndex)
            {
                foreach (var kv in dict)
                {
                    array[arrayIndex++] = kv.Key;
                }
            }
            public bool Remove(TKey item) => throw new NotSupportedException();
            public IEnumerator<TKey> GetEnumerator() => dict.GetEnumeratorKey();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public ValueCollection Values => new ValueCollection(this);
        public class ValueCollection : IEnumerable<TValue>, ICollection<TValue>, IReadOnlyCollection<TValue>
        {
            private LIB_Dictionary<TKey, TValue> dict;
            public ValueCollection(LIB_Dictionary<TKey, TValue> dict)
            {
                this.dict = dict;
            }
            public int Count => dict.Count;
            public bool IsReadOnly => true;
            public void Add(TValue item) => throw new NotSupportedException();
            public void Clear() => throw new NotSupportedException();
            public bool Contains(TValue item)
            {
                foreach (var kv in dict)
                {
                    if (kv.Value.Equals(item)) return true;
                }
                return false;
            }
            public void CopyTo(TValue[] array, int arrayIndex)
            {
                foreach (var kv in dict)
                {
                    array[arrayIndex++] = kv.Value;
                }
            }
            public bool Remove(TValue item) => throw new NotSupportedException();
            public IEnumerator<TValue> GetEnumerator() => dict.GetEnumeratorValue();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public Enumerator GetEnumerator() => new Enumerator(this);
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly LIB_Dictionary<TKey, TValue> dict;
            private int frontObj;
            private int firstObj;
            private bool first;

            internal Enumerator(LIB_Dictionary<TKey, TValue> dict)
            {
                this.dict = dict;
                first = true;
            }

            public bool MoveNext()
            {
                while (dict.elemKouho.Count > 0)
                {
                    if (!first && dict.elemKouho.Front == firstObj) return false;
                    var idx = dict.elemKouho.PopFront();
                    if (dict.st[idx] == 2)
                    {
                        dict.elemKouho.PushBack(idx);
                        frontObj = idx;
                        if (first)
                        {
                            firstObj = frontObj;
                            first = false;
                        }
                        return true;
                    }
                    else dict.kouhoAdded[idx] = false;
                }
                return false;
            }

            public void Reset()
            {
                first = true;
            }

            public KeyValuePair<TKey, TValue> Current => dict.bck[frontObj];
            object IEnumerator.Current => Current;

            public void Dispose() { }
        }

        public EnumeratorKey GetEnumeratorKey() => new EnumeratorKey(this);
        public struct EnumeratorKey : IEnumerator<TKey>
        {
            private readonly LIB_Dictionary<TKey, TValue> dict;
            private int frontObj;
            private int firstObj;
            private bool first;

            internal EnumeratorKey(LIB_Dictionary<TKey, TValue> dict)
            {
                this.dict = dict;
                first = true;
            }

            public bool MoveNext()
            {
                while (dict.elemKouho.Count > 0)
                {
                    if (!first && dict.elemKouho.Front == firstObj) return false;
                    var idx = dict.elemKouho.PopFront();
                    if (dict.st[idx] == 2)
                    {
                        dict.elemKouho.PushBack(idx);
                        frontObj = idx;
                        if (first)
                        {
                            firstObj = frontObj;
                            first = false;
                        }
                        return true;
                    }
                    else dict.kouhoAdded[idx] = false;
                }
                return false;
            }

            public void Reset()
            {
                first = true;
            }

            public TKey Current => dict.bck[frontObj].Key;
            object IEnumerator.Current => Current;

            public void Dispose() { }
        }

        public EnumeratorValue GetEnumeratorValue() => new EnumeratorValue(this);
        public struct EnumeratorValue : IEnumerator<TValue>
        {
            private readonly LIB_Dictionary<TKey, TValue> dict;
            private int frontObj;
            private int firstObj;
            private bool first;

            internal EnumeratorValue(LIB_Dictionary<TKey, TValue> dict)
            {
                this.dict = dict;
                first = true;
            }

            public bool MoveNext()
            {
                while (dict.elemKouho.Count > 0)
                {
                    if (!first && dict.elemKouho.Front == firstObj) return false;
                    var idx = dict.elemKouho.PopFront();
                    if (dict.st[idx] == 2)
                    {
                        dict.elemKouho.PushBack(idx);
                        frontObj = idx;
                        if (first)
                        {
                            firstObj = frontObj;
                            first = false;
                        }
                        return true;
                    }
                    else dict.kouhoAdded[idx] = false;
                }
                return false;
            }

            public void Reset()
            {
                first = true;
            }

            public TValue Current => dict.bck[frontObj].Value;
            object IEnumerator.Current => Current;

            public void Dispose() { }
        }
    }
    class LIB_HashSet<TKey> : IEnumerable<TKey>, IEquatable<LIB_HashSet<TKey>> where TKey : IEquatable<TKey>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(LIB_HashSet<TKey> x)
        {
            if (Count != x.Count) return false;
            foreach (var v in this)
            {
                if (!x.Contains(v)) return false;
            }
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object x) => x == null ? false : Equals((LIB_HashSet<TKey>)x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => dict.GetHashCode();

        LIB_Dictionary<TKey, byte> dict;
        public LIB_HashSet()
        {
            dict = new LIB_Dictionary<TKey, byte>();
        }
        public int Count
        {
            get { return dict.Count; }
            private set { }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TKey key) => dict.AddOrUpdate(key, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(TKey key) => dict.ContainsKey(key);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(TKey key) => dict.Remove(key);

        IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => dict.GetEnumeratorKey();
        IEnumerator IEnumerable.GetEnumerator() => dict.GetEnumeratorKey();
    }
    ////end
}