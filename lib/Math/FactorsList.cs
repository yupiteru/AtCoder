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
    class LIB_FactorsList
    {
        int[] minFactor;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FactorsList(int n)
        {
            minFactor = new int[n + 1];
            minFactor[1] = 1;
            for (var i = 2; i <= n; i++)
            {
                if (minFactor[i] == 0)
                {
                    minFactor[i] = i;
                    for (var j = i * 2; j <= n; j += i)
                    {
                        if (minFactor[j] == 0) minFactor[j] = i;
                    }
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Dictionary<int, int> Factors(int n)
        {
            var res = new Dictionary<int, int>();
            while (n > 1)
            {
                if (res.ContainsKey(minFactor[n])) res[minFactor[n]]++;
                else res[minFactor[n]] = 1;
                n /= minFactor[n];
            }
            return res;
        }
    }
    ////end
}