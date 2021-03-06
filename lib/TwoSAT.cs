
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
    class LIB_TwoSAT : LIB_StronglyConnectedComponents
    {
        // use LIB_StronglyConnectedComponents
        public LIB_Bitset Answer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            private set;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_TwoSAT(long n) : base(n << 1)
        {
            Answer = new LIB_Bitset(n);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddClause(long i, bool f, long j, bool g)
        {
            base.AddPath((i << 1) + (f ? 0 : 1), (j << 1) + (g ? 1 : 0));
            base.AddPath((j << 1) + (g ? 0 : 1), (i << 1) + (f ? 1 : 0));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Satisfiable()
        {
            var id = SCCIDs().Item2;
            var idlen = id.Length;
            if (idlen == 0) return true;
            ref int idref = ref id[0];
            for (var i = 0; i < idlen; i += 2)
            {
                var idi = Unsafe.Add(ref idref, i);
                var idi1 = Unsafe.Add(ref idref, i + 1);
                if (idi == idi1) return false;
                Answer[i >> 1] = idi < idi1;
            }
            return true;
        }
        new void AddPath(long from, long to) { }
        new int[][] SCC() { throw new Exception(); }
    }
    ////end
}