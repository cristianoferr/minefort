using System;
using System.Diagnostics;

namespace Rimworld
{
    public class Utils
    {
        static Random rnd = new Random();
        internal static int Random(float p1, float p2)
        {
            return rnd.Next((int)p1, (int)p2);
        }

        public static void LogError(string p)
        {
            Console.WriteLine("ERROR: " + p);
            Debug.WriteLine("ERROR: " + p);
        }

        public static void Log(string p)
        {
            Console.WriteLine("[LOG] " + p);
            Debug.WriteLine("[LOG] " + p);
        }
    }
}
