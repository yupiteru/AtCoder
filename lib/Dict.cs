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
using System.Runtime.InteropServices.Marshalling;

namespace Library
{
    ////start
    static partial class LIB_Static
    {
        public static LIB_HashSet<T> ToHashSet<T>(this IEnumerable<T> source) where T : IEquatable<T>
        {
            var ret = new LIB_HashSet<T>();
            foreach (var item in source) ret.Add(item);
            return ret;
        }
        public static LIB_Dictionary<TKey, TValue> ToDictionary<T, TKey, TValue>(this IEnumerable<T> source, Func<T, TKey> keySelector, Func<T, TValue> elementSelector) where TKey : IEquatable<TKey>
        {
            var ret = new LIB_Dictionary<TKey, TValue>();
            foreach (var item in source) ret[keySelector(item)] = elementSelector(item);
            return ret;
        }
    }
    class LIB_Dictionary<TKey, TValue> : IDictionary<TKey, TValue>, IEquatable<LIB_Dictionary<TKey, TValue>> where TKey : IEquatable<TKey>
    {
        struct Entry
        {
            public bool used;
            public int hashCode;
            public TKey Key;
            public TValue Value;
            public int Next;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(LIB_Dictionary<TKey, TValue> x)
        {
            if (totalHash != x.totalHash) return false;
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
            set => Add(key, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int Hash(TKey key)
        {
            unchecked
            {
                {
                    if (key is long h && h > int.MaxValue)
                    {
                        h = (~h) + (h << 18);
                        h = h ^ (h >> 31);
                        h = h * 21;
                        h = h ^ (h >> 11);
                        h = h + (h << 6);
                        h = h ^ (h >> 22);
                        return (int)h;
                    }
                }
                {
                    if (key is ulong h && h > uint.MaxValue)
                    {
                        h = (~h) + (h << 18);
                        h = h ^ (h >> 31);
                        h = h * 21;
                        h = h ^ (h >> 11);
                        h = h + (h << 6);
                        h = h ^ (h >> 22);
                        return (int)h;
                    }
                }
                return key.GetHashCode();
            }
        }
        bool[] kouhoAdded;
        int[] bck;
        Entry[] entries;
        int mask;
        int totalHash;
        int freeCount;
        int freeList;
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
            freeList = -1;
            kouhoAdded = new bool[0];
            bck = new int[0];
            entries = new Entry[0];
            elemKouho = new LIB_Deque<int>();
            if (defval != null) this.defval = defval;
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
            else return;
            var oldEntries = entries;
            var oldKouhoAdded = kouhoAdded;
            entries = new Entry[nextCnt];
            kouhoAdded = new bool[nextCnt];
            bck = new int[nextCnt];
            mask = nextCnt - 1;
            if (nowlen == 0) return;
            Array.Copy(oldEntries, entries, nowlen);
            Array.Copy(oldKouhoAdded, kouhoAdded, nowlen);
            ref var oldEntriesref = ref oldEntries[0];
            ref var kouhoAddedref = ref kouhoAdded[0];
            ref var entriesref = ref entries[0];
            var firstItem = elemKouho.PopFront();
            var pos = firstItem;
            while (true)
            {
                ref var entry = ref Unsafe.Add(ref oldEntriesref, pos);
                if (entry.used)
                {
                    var h = entry.hashCode;

                    ref var bckref = ref bck[h & mask];
                    Unsafe.Add(ref entriesref, pos).Next = bckref - 1;
                    bckref = pos + 1;
                    elemKouho.PushBack(pos);
                }
                else Unsafe.Add(ref kouhoAddedref, pos) = false;
                if (firstItem == elemKouho.Front) break;
                pos = elemKouho.PopFront();
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public void Add(TKey key, TValue val)
        {
            Reserve(Count + 1);

            var h = Hash(key);
            ref var entriesref = ref entries[0];
            ref var bckref = ref bck[h & mask];
            var i = bckref - 1;
            while (i >= 0)
            {
                ref var entry = ref Unsafe.Add(ref entriesref, i);
                if (entry.hashCode == h && EqualityComparer<TKey>.Default.Equals(entry.Key, key))
                {
                    totalHash ^= entry.Value.GetHashCode() ^ val.GetHashCode();
                    entry.Value = val;
                    return;
                }
                i = entry.Next;
            }

            if (freeCount > 0)
            {
                i = freeList;
                freeList = Unsafe.Add(ref entriesref, freeList).Next;
                --freeCount;
            }
            else
            {
                i = Count;
            }

            {
                ref var entry = ref Unsafe.Add(ref entriesref, i);
                entry.used = true;
                entry.hashCode = h;
                entry.Next = bckref - 1;
                entry.Key = key;
                entry.Value = val;
                bckref = i + 1;
                if (!kouhoAdded[i])
                {
                    kouhoAdded[i] = true;
                    elemKouho.PushBack(i);
                }
                totalHash ^= key.GetHashCode() ^ val.GetHashCode();
                ++Count;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(TKey key)
        {
            var h = Hash(key);
            ref var entriesref = ref entries[0];
            ref var bckref = ref bck[h & mask];
            var i = bckref - 1;
            var last = -1;
            while (i >= 0)
            {
                ref var entry = ref Unsafe.Add(ref entriesref, i);
                if (entry.hashCode == h && EqualityComparer<TKey>.Default.Equals(entry.Key, key))
                {
                    if (last < 0)
                    {
                        bckref = entry.Next + 1;
                    }
                    else
                    {
                        Unsafe.Add(ref entriesref, last).Next = entry.Next;
                    }
                    totalHash ^= entry.Key.GetHashCode() ^ entry.Value.GetHashCode();
                    --Count;
                    entry.used = false;
                    entry.Next = freeList;

                    freeList = i;
                    ++freeCount;
                    return true;
                }
                last = i;
                i = entry.Next;
            }

            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(TKey key)
        {
            if (Count == 0) return false;
            var h = Hash(key);
            ref var entriesref = ref entries[0];
            var i = bck[h & mask] - 1;
            while (i >= 0)
            {
                ref var entry = ref Unsafe.Add(ref entriesref, i);
                if (entry.hashCode == h && EqualityComparer<TKey>.Default.Equals(entry.Key, key))
                {
                    return true;
                }
                i = entry.Next;
            }
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue GetOrInsert(TKey key, Func<TKey, TValue> val = null)
        {
            Reserve(Count + 1);

            var h = Hash(key);
            ref var entriesref = ref entries[0];
            ref var bckref = ref bck[h & mask];
            var i = bckref - 1;
            while (i >= 0)
            {
                ref var entry = ref Unsafe.Add(ref entriesref, i);
                if (entry.hashCode == h && EqualityComparer<TKey>.Default.Equals(entry.Key, key))
                {
                    return entry.Value;
                }
                i = entry.Next;
            }

            if (freeCount > 0)
            {
                i = freeList;
                freeList = Unsafe.Add(ref entriesref, freeList).Next;
                --freeCount;
            }
            else
            {
                i = Count;
            }

            {
                ref var entry = ref entries[i];
                entry.used = true;
                entry.hashCode = h;
                entry.Next = bckref - 1;
                entry.Key = key;
                entry.Value = val == null ? defval(key) : val(key);
                bckref = i + 1;
                if (!kouhoAdded[i])
                {
                    kouhoAdded[i] = true;
                    elemKouho.PushBack(i);
                }
                totalHash ^= key.GetHashCode() ^ entry.Value.GetHashCode();
                ++Count;

                return entry.Value;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue GetAndRemove(TKey key)
        {
            var h = Hash(key);
            ref var entriesref = ref entries[0];
            ref var bckref = ref bck[h & mask];
            var i = bckref - 1;
            var last = -1;
            while (i >= 0)
            {
                ref var entry = ref Unsafe.Add(ref entriesref, i);
                if (entry.hashCode == h && EqualityComparer<TKey>.Default.Equals(entry.Key, key))
                {
                    if (last < 0)
                    {
                        bckref = entry.Next + 1;
                    }
                    else
                    {
                        Unsafe.Add(ref entriesref, last).Next = entry.Next;
                    }
                    totalHash ^= entry.Key.GetHashCode() ^ entry.Value.GetHashCode();
                    --Count;
                    entry.used = false;
                    entry.Next = freeList;

                    freeList = i;
                    ++freeCount;
                    return entry.Value;
                }
                last = i;
                i = entry.Next;
            }

            return default;
        }

        public class KeyCollection : ICollection<TKey>, IReadOnlyCollection<TKey>
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
                foreach (var kv in dict) array[arrayIndex++] = kv.Key;
            }
            public bool Remove(TKey item) => throw new NotSupportedException();
            public IEnumerator<TKey> GetEnumerator() => dict.GetEnumeratorKey();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public KeyCollection Keys => new KeyCollection(this);

        public ValueCollection Values => new ValueCollection(this);

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        public bool IsReadOnly => false;

        public class ValueCollection : ICollection<TValue>, IReadOnlyCollection<TValue>
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
                foreach (var kv in dict) if (kv.Value.Equals(item)) return true;
                return false;
            }
            public void CopyTo(TValue[] array, int arrayIndex)
            {
                foreach (var kv in dict) array[arrayIndex++] = kv.Value;
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
                    if (dict.entries[idx].used)
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

            public KeyValuePair<TKey, TValue> Current => new KeyValuePair<TKey, TValue>(dict.entries[frontObj].Key, dict.entries[frontObj].Value);
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
                    if (dict.entries[idx].used)
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

            public TKey Current => dict.entries[frontObj].Key;
            object IEnumerator.Current => Current;

            public void Dispose() { }
        }

        public EnumeratorValue GetEnumeratorValue() => new EnumeratorValue(this);

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (ContainsKey(key))
            {
                value = this[key];
                return true;
            }
            value = default;
            return false;
        }

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        public void Clear()
        {
            Count = 0;
            totalHash = 0;
            freeCount = 0;
            freeList = -1;
            kouhoAdded = new bool[0];
            bck = new int[0];
            entries = new Entry[0];
            elemKouho.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (ContainsKey(item.Key)) return this[item.Key].Equals(item.Value);
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var kv in this) array[arrayIndex++] = kv;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

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
                    if (dict.entries[idx].used)
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

            public TValue Current => dict.entries[frontObj].Value;
            object IEnumerator.Current => Current;

            public void Dispose() { }
        }
    }
    class LIB_HashSet<TKey> : ICollection<TKey>, IEquatable<LIB_HashSet<TKey>> where TKey : IEquatable<TKey>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(LIB_HashSet<TKey> x)
        {
            if (GetHashCode() != x.GetHashCode()) return false;
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

        public bool IsReadOnly => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TKey key) => dict.Add(key, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(TKey key) => dict.ContainsKey(key);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(TKey key) => dict.Remove(key);

        IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => dict.GetEnumeratorKey();
        IEnumerator IEnumerable.GetEnumerator() => dict.GetEnumeratorKey();

        public void Clear() => dict.Clear();

        public void CopyTo(TKey[] array, int arrayIndex)
        {
            foreach (var item in this) array[arrayIndex++] = item;
        }
    }
    ////end
}