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
    class LIB_RadixHeap
    {
        ulong[][] heap;
        int[][] dat;
        int[] cnts;
        ulong last;
        ulong size;
        public int Count => (int)size;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_RadixHeap()
        {
            heap = new ulong[65][];
            dat = new int[65][];
            cnts = new int[65];
            ref ulong[] heapref = ref heap[0];
            ref int[] datref = ref dat[0];
            for (var i = 0; i < 65; ++i)
            {
                Unsafe.Add(ref heapref, i) = new ulong[8];
                Unsafe.Add(ref datref, i) = new int[8];
            }
            last = size = 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(long x, int value)
        {
            ++size;
            ulong key = (ulong)x;
            var b = 64 - (int)System.Runtime.Intrinsics.X86.Lzcnt.X64.LeadingZeroCount(key ^ last);
            ref ulong[] heapbref = ref heap[b];
            ref int[] datbref = ref dat[b];
            ref int cntsb = ref cnts[b];
            heapbref[cntsb] = key;
            datbref[cntsb] = value;
            if (heapbref.Length == ++cntsb)
            {
                var heaptmp = new ulong[cntsb << 1];
                var dattmp = new int[cntsb << 1];
                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref heaptmp[0]), ref Unsafe.As<ulong, byte>(ref heapbref[0]), (uint)(8 * cntsb));
                Unsafe.CopyBlock(ref Unsafe.As<int, byte>(ref dattmp[0]), ref Unsafe.As<int, byte>(ref datbref[0]), (uint)(4 * cntsb));
                heapbref = heaptmp;
                datbref = dattmp;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (ulong, int) Pop()
        {
            ref ulong[] heapref = ref heap[0];
            ref int[] datref = ref dat[0];
            ref int cntsref = ref cnts[0];
            if (cntsref == 0)
            {
                var i = 1;
                while (Unsafe.Add(ref cntsref, i) == 0) ++i;
                ref ulong[] heapi = ref Unsafe.Add(ref heapref, i);
                ref int[] dati = ref Unsafe.Add(ref datref, i);
                ref int cntsi = ref Unsafe.Add(ref cntsref, i);
                ref ulong heapiref = ref heapi[0];
                ref int datiref = ref dati[0];
                var newLast = heapiref;
                for (var j = 1; j < cntsi; j++)
                {
                    var v = Unsafe.Add(ref heapiref, j);
                    if (newLast > v) newLast = v;
                }
                for (var j = 0; j < cntsi; ++j)
                {
                    var item = Unsafe.Add(ref heapiref, j);
                    var b = 64 - (int)System.Runtime.Intrinsics.X86.Lzcnt.X64.LeadingZeroCount(item ^ newLast);
                    ref int cntsb = ref Unsafe.Add(ref cntsref, b);
                    ref ulong[] heapbref = ref Unsafe.Add(ref heapref, b);
                    ref int[] datbref = ref Unsafe.Add(ref datref, b);
                    heapbref[cntsb] = item;
                    datbref[cntsb] = Unsafe.Add(ref datiref, j);
                    if (heapbref.Length == ++cntsb)
                    {
                        var heaptmp = new ulong[cntsb << 1];
                        var dattmp = new int[cntsb << 1];
                        Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref heaptmp[0]), ref Unsafe.As<ulong, byte>(ref heapbref[0]), (uint)(8 * cntsb));
                        Unsafe.CopyBlock(ref Unsafe.As<int, byte>(ref dattmp[0]), ref Unsafe.As<int, byte>(ref datbref[0]), (uint)(4 * cntsb));
                        heapbref = heaptmp;
                        datbref = dattmp;
                    }
                }
                last = newLast;
                cntsi = 0;
            }
            --size;
            --cntsref;
            return (heapref[cntsref], datref[cntsref]);
        }
    }
    ////end
}