
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
    class LIB_String
    {
        public static int[] ZAlgorithm(long[] s)
        {
            var ret = new int[s.Length];
            ret[0] = s.Length;
            int i = 1, j = 0;
            while (i < s.Length)
            {
                while (i + j < s.Length && s[j] == s[i + j]) ++j;
                ret[i] = j;
                if (j == 0) { ++i; continue; }
                var k = 1;
                while (i + k < s.Length && k + ret[k] < j) { ret[i + k] = ret[k]; ++k; }
                i += k; j -= k;
            }
            return ret;
        }
    }
    ////end
}