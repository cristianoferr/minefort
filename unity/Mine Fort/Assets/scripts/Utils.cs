using System;
using System.Diagnostics;
using UnityEngine;

namespace Rimworld
{
    public class Utils
    {
        static System.Random rnd = new System.Random();
        internal static int Random(float p1, float p2)
        {
            return rnd.Next((int)p1, (int)p2);
        }

        public static void LogError(string p)
        {
            Console.WriteLine("ERROR: " + p);
            //  Debug.WriteLine("ERROR: " + p);
            UnityEngine.Debug.LogError("ERROR: " + p);
        }

        public static void Log(string p)
        {
            Console.WriteLine("[LOG] " + p);
            UnityEngine.Debug.Log("[LOG] " + p);
        }

        //z is height in a 2.5D environment
        public static Vector3 TwoDToIso(float x, float y,float z=0,float priority=0)
        {
            x = x * 2f;
            y = y * 2f;
            Vector3 pos = new Vector3(x - y, (x + y) / 2+z, x+y-priority);
            return pos;
        }


        public static Vector3 IsoTo2D(float x, float y)
        {
            x = x / 2f;
            y = y / 2f;
            Vector3 pos = new Vector3((2*y+x)/2,(2*y-x)/2, 0);
            return pos;
        }

        internal static Vector3 TwoDToIso(Vector3 currFramePosition)
        {
            return TwoDToIso(currFramePosition.x, currFramePosition.y, currFramePosition.z);
        }


        /*
         function isoTo2D(pt:Point):Point{
      var tempPt:Point = new Point(0, 0);
      tempPt.x = (2 * pt.y + pt.x) / 2;
      tempPt.y = (2 * pt.y - pt.x) / 2;
      return(tempPt);
    }
             function twoDToIso(pt:Point):Point{
      var tempPt:Point = new Point(0,0);
      tempPt.x = pt.x - pt.y;
      tempPt.y = (pt.x + pt.y) / 2;
      return(tempPt);
    }
             */
    }
}
