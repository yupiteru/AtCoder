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
    class LIB_FastIO
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FastIO() { str = Console.OpenStandardInput(); }
        readonly Stream str;
        readonly byte[] buf = new byte[2048];
        int len, ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        byte read()
        {
            if (ptr >= len)
            {
                ptr = 0;
                if ((len = str.Read(buf, 0, 2048)) <= 0)
                {
                    return 0;
                }
            }
            return buf[ptr++];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        char Char()
        {
            byte b = 0;
            do b = read();
            while (b < 33 || 126 < b);
            return (char)b;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        virtual public string Scan()
        {
            var sb = new StringBuilder();
            for (var b = Char(); b >= 33 && b <= 126; b = (char)read())
                sb.Append(b);
            return sb.ToString();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        virtual public long Long()
        {
            long ret = 0; byte b = 0; var ng = false;
            do b = read();
            while (b != '-' && (b < '0' || '9' < b));
            if (b == '-') { ng = true; b = read(); }
            for (; true; b = read())
            {
                if (b < '0' || '9' < b)
                    return ng ? -ret : ret;
                else ret = (ret << 3) + (ret << 1) + b - '0';
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        virtual public double Double() { return double.Parse(Scan(), CultureInfo.InvariantCulture); }
    }
    class LIB_FastIODebug : LIB_FastIO
    {
        Queue<string> param = new Queue<string>();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        string NextString() { if (param.Count == 0) foreach (var item in Console.ReadLine().Split(' ')) param.Enqueue(item); return param.Dequeue(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FastIODebug() { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string Scan() => NextString();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override long Long() => long.Parse(NextString());
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override double Double() => double.Parse(NextString());
    }
    ////end
}