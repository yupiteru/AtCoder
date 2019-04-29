using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC051C
    {
        static class Console_
        {
            private static Queue<string> param = new Queue<string>();
            public static int NextInt() => int.Parse(NextString());
            public static string NextString()
            {
                if (param.Count == 0) foreach (var item in Console.ReadLine().Split(' ')) param.Enqueue(item);
                return param.Dequeue();
            }
        }
        static public void Main(string[] args)
        {
            var sx = Console_.NextInt();
            var sy = Console_.NextInt();
            var tx = Console_.NextInt();
            var ty = Console_.NextInt();

            var fixMoveUp = new string(Enumerable.Repeat('U', ty - sy).ToArray());
            var fixMoveRight = new string(Enumerable.Repeat('R', tx - sx).ToArray());
            var fixMoveDown = new string(Enumerable.Repeat('D', ty - sy).ToArray());
            var fixMoveLeft = new string(Enumerable.Repeat('L', tx - sx).ToArray());
            Console.Write(fixMoveUp);
            Console.Write(fixMoveRight);
            Console.Write(fixMoveDown);
            Console.Write(fixMoveLeft);
            Console.Write("LU");
            Console.Write(fixMoveUp);
            Console.Write(fixMoveRight);
            Console.Write("RD");
            Console.Write("RD");
            Console.Write(fixMoveDown);
            Console.Write(fixMoveLeft);
            Console.Write("LU");
        }
    }
}
