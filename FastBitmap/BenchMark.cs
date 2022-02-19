using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FastBitmap
{
    public class Benchmark
    {
        private static DateTime startDate = DateTime.MinValue;
        private static DateTime endDate = DateTime.MinValue;

        public static TimeSpan Span { get { return endDate.Subtract(startDate); } }

        public static void Start() { startDate = DateTime.Now; }

        public static void End() { endDate = DateTime.Now; }

        public static double GetSeconds()
        {
            if (endDate == DateTime.MinValue) return 0.0;
            else return Span.TotalSeconds;
        }
        [DllImport("msvcrt.dll")]
        private static extern int memcmp(IntPtr b1, IntPtr b2, long count);

        public static bool CompareMemCmp(PointBitmap b1, PointBitmap b2)
        {
            int height = b1.Height;
            int weight = b1.Width;
            b1.LockBits();
            b2.LockBits();

            try
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < weight; x++)
                    {
                        if (b1.GetPixel(x, y) != b2.GetPixel(x,y))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            finally
            {
                b1.UnlockBits();
                b2.UnlockBits();
            }
        }
    }

}
