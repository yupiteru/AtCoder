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
    class LIB_Dict<K, V> : Dictionary<K, V>
    {
        Func<K, V> d;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_Dict(Func<K, V> _d) { d = _d; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_Dict() : this(_ => default(V)) { }
        new public V this[K i]
        {
            get
            {
                return TryGetValue(i, out var v) ? v : base[i] = d(i);
            }
            set
            {
                base[i] = value;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            unchecked
            {
                var ret = 23;
                for (var e = GetEnumerator(); e.MoveNext();) ret = (ret * 37) ^ (e.Current.Key.GetHashCode() << 5 + e.Current.Key.GetHashCode()) ^ e.Current.Value.GetHashCode();
                return ret;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            var o = (LIB_Dict<K, V>)obj;
            foreach (var kv in o)
            {
                if (!ContainsKey(kv.Key) || !this[kv.Key].Equals(kv.Value))
                    return false;
            }
            return o.Count == Count;
        }
    }
    ////end
}