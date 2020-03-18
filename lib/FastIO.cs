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
        public LIB_FastIO() { str = Console.OpenStandardInput(); }
        readonly Stream str;
        readonly byte[] buf = new byte[1024];
        int len, ptr;
        public bool isEof = false;
        public bool IsEndOfStream { get { return isEof; } }
        byte read()
        {
            if (isEof) throw new EndOfStreamException();
            if (ptr >= len)
            {
                ptr = 0;
                if ((len = str.Read(buf, 0, 1024)) <= 0)
                {
                    isEof = true;
                    return 0;
                }
            }
            return buf[ptr++];
        }
        char Char()
        {
            byte b = 0;
            do b = read();
            while (b < 33 || 126 < b);
            return (char)b;
        }
        virtual public string Scan()
        {
            var sb = new StringBuilder();
            for (var b = Char(); b >= 33 && b <= 126; b = (char)read())
                sb.Append(b);
            return sb.ToString();
        }
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
                else ret = ret * 10 + b - '0';
            }
        }
        virtual public double Double() { return double.Parse(Scan(), CultureInfo.InvariantCulture); }
    }
    class LIB_FastIODebug : LIB_FastIO
    {
        Queue<string> param = new Queue<string>();
        string NextString() { if (param.Count == 0) foreach (var item in Console.ReadLine().Split(' ')) param.Enqueue(item); return param.Dequeue(); }
        public LIB_FastIODebug() { }
        public override string Scan() => NextString();
        public override long Long() => long.Parse(NextString());
        public override double Double() => double.Parse(NextString());
    }
    ////end
}